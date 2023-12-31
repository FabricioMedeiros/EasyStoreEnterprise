﻿using ESE.Bff.Shopping.Models;
using ESE.Bff.Shopping.Services;
using ESE.WebAPI.Core.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ESE.Bff.Shopping.Controllers
{
    [Authorize]
    public class OrderController : MainController
    {
        private readonly ICatalogService _catalogService;
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        private readonly IClientService _clientService;

        public OrderController(ICatalogService catalogService, ICartService cartService, IOrderService orderService, IClientService clientService)
        {
            _catalogService = catalogService;
            _cartService = cartService;
            _orderService = orderService;
            _clientService = clientService;
        }

        [HttpPost]
        [Route("shopping/order")]
        public async Task<IActionResult> AddOrder(OrderDTO order)
        {
            var cart = await _cartService.GetCart();
            var products = await _catalogService.GetItems(cart.Items.Select(p => p.ProductId));
            var address = await _clientService.GetAddress();

            if (!await CheckCartProducts(cart, products)) return CustomResponse();

            SetDataOrder(cart, address, order);

            return CustomResponse(await _orderService.FinalizeOrder(order));
        }

        [HttpGet("shopping/order/last")]
        public async Task<IActionResult> LastOrder()
        {
            var order = await _orderService.GetLastOrder();
            if (order is null)
            {
                AddProcessingError("Pedido não encontrado!");
                return CustomResponse();
            }

            return CustomResponse(order);
        }

        [HttpGet("shopping/order/list-client")]
        public async Task<IActionResult> ListByClient()
        {
            var order = await _orderService.GetListByClientId();

            return order == null ? NotFound() : CustomResponse(order);
        }

        private async Task<bool> CheckCartProducts(CartDTO cart, IEnumerable<ItemProductDTO> products)
        {
            if (cart.Items.Count != products.Count())
            {
                var unavailableItems = cart.Items.Select(c => c.ProductId).Except(products.Select(p => p.Id)).ToList();

                foreach (var itemId in unavailableItems)
                {
                    var itemCart = cart.Items.FirstOrDefault(c => c.ProductId == itemId);
                    AddProcessingError($"O item {itemCart.Name} não está mais disponível no catálogo, o remova do carrinho para prosseguir com a compra");
                }

                return false;
            }

            foreach (var itemCart in cart.Items)
            {
                var productCatalog = products.FirstOrDefault(p => p.Id == itemCart.ProductId);

                if (productCatalog.Price != itemCart.Price)
                {
                    var msgErro = $"O produto {itemCart.Name} mudou de valor (de: " +
                                  $"{string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", itemCart.Price)} para: " +
                                  $"{string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", productCatalog.Price)}) desde que foi adicionado ao carrinho.";

                    AddProcessingError(msgErro);

                    var responseRemove = await _cartService.RemoveItemCart(itemCart.ProductId);

                    if (ResponseHasErrors(responseRemove))
                    {
                        AddProcessingError($"Não foi possível remover automaticamente o produto {itemCart.Name} do seu carrinho, _" +
                                                   "remova e adicione novamente caso ainda deseje comprar este item");
                        return false;
                    }

                    itemCart.Price = productCatalog.Price;
                    var responseAdd = await _cartService.AddItemCart(itemCart);

                    if (ResponseHasErrors(responseAdd))
                    {
                        AddProcessingError($"Não foi possível atualizar automaticamente o produto {itemCart.Name} do seu carrinho, _" +
                                                   "adicione novamente caso ainda deseje comprar este item");
                        return false;
                    }

                    ClearProcessingError();
                    AddProcessingError(msgErro + " Atualizamos o valor em seu carrinho, realize a conferência do pedido e se preferir remova o produto");

                    return false;
                }
            }

            return true;
        }
        private void SetDataOrder(CartDTO cart, AddressDTO address, OrderDTO order)
        {
            order.VoucherCode = cart.Voucher?.Code;
            order.VoucherUsed = cart.VoucherUsed;
            order.TotalPrice = cart.TotalPrice;
            order.Discount = cart.Discount;
            order.OrderItems = cart.Items;
            order.Address = address;
        }
    }
}
