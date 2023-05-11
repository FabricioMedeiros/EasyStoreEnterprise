using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ESE.WebApp.MVC.Models
{
    public class OrderTransactionViewModel
    {
        #region Pedido

        public decimal TotalPrice { get; set; }
        public decimal Discount { get; set; }
        public string VoucherCode { get; set; }
        public bool VoucherUsed { get; set; }

        public List<ItemCartViewModel> Items { get; set; } = new List<ItemCartViewModel>();

        #endregion

        #region Endereco

        public AddressViewModel Address { get; set; }

        #endregion

    }
}
