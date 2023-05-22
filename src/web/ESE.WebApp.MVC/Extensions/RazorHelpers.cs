using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Microsoft.AspNetCore.Mvc.Razor;

namespace ESE.WebApp.MVC.Extensions
{
    public static class RazorHelpers
    {
        public static string HashEmailForGravatar(this RazorPage page, string email)
        {
            var md5Hasher = MD5.Create();
            var data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(email));
            var sBuilder = new StringBuilder();

            foreach (var t in data)
            {
                sBuilder.Append(t.ToString("x2"));
            }
            return sBuilder.ToString();
        }     

        public static string FormatCurrency(this RazorPage page, decimal price)
        {
            return price > 0 ? string.Format(Thread.CurrentThread.CurrentCulture, "{0:C}", price) : "Gratuito";
        }

        public static string StockMessage(this RazorPage page, int stock)
        {
            return stock > 0 ? $"Apenas {stock} em estoque!" : "Produto esgotado!";
        }

        public static string UnitsProduct(this RazorPage page, int quantity)
        {
            return quantity > 1 ? $"Unidades {quantity}" : $"Unidade {quantity}";
        }

        public static string SelectOptionsQuantity(this RazorPage page, int quantity, int value = 0)
        {
            var sb = new StringBuilder();
            for (var i = 1; i <= quantity; i++)
            {
                var selected = "";
                if (i == value) selected = "selected";
                sb.Append($"<option {selected} value='{i}'>{i}</option>");
            }

            return sb.ToString();
        }
        public static string UnitsByProductTotalPrice(this RazorPage page, int unidades, decimal valor)
        {
            return $"{unidades} x {page.FormatCurrency(valor)} = Total: {page.FormatCurrency(valor * unidades)}";
        }

        public static string ShowStatus(this RazorPage page, int status)
        {
            var statusMensage = "";
            var statusClass = "";

            switch (status)
            {
                case 1:
                    statusClass = "info";
                    statusMensage = "Em aprovação";
                    break;
                case 2:
                    statusClass = "primary";
                    statusMensage = "Aprovado";
                    break;
                case 3:
                    statusClass = "danger";
                    statusMensage = "Recusado";
                    break;
                case 4:
                    statusClass = "success";
                    statusMensage = "Entregue";
                    break;
                case 5:
                    statusClass = "warning";
                    statusMensage = "Cancelado";
                    break;

            }

            return $"<span class='badge badge-{statusClass}'>{statusMensage}</span>";
        }
    }
}