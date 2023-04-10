using ESE.Bff.Shopping.Communication;
using ESE.WebApp.MVC.Models;
using System;
using System.Threading.Tasks;

namespace ESE.WebApp.MVC.Services
{
    public interface ICartService
    {
        Task<CartViewModel> GetCart();
        Task<ResponseResult> AddItemCart(ItemProductViewModel product);
        Task<ResponseResult> UpdateItemCart(Guid productId, ItemProductViewModel product);
        Task<ResponseResult> RemoveItemCart(Guid productId);
    }
}
