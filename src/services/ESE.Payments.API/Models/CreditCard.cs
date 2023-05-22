using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESE.Payments.API.Models
{
    public class CreditCard
    {
        public string NameCard { get; set; }
        public string NumberCard { get; set; }
        public string MonthYearExpiry { get; set; }
        public string CVV { get; set; }

        public CreditCard(string nameCard, string numberCard, string monthYearExpiry, string cVV)
        {
            NameCard = nameCard;
            NumberCard = numberCard;
            MonthYearExpiry = monthYearExpiry;
            CVV = cVV;
        }
        protected CreditCard() { }
    }
}
