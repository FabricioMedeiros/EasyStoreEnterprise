using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ESE.WebApp.MVC.Models
{
    public class AddressViewModel
    {
        [Required]
        [DisplayName("Logradouro")]
        public string Street { get; private set; }
        [Required]
        [DisplayName("Número")]
        public string Number { get; private set; }
        [Required]
        [DisplayName("Complemento")]
        public string Complement { get; private set; }
        [Required]
        [DisplayName("Bairro")]
        public string Neighborhood { get; private set; }
        [Required]
        [DisplayName("Cidade")]
        public string City { get; private set; }
        [Required]
        [DisplayName("Estado")]
        public string State { get; private set; }
        [Required]
        [DisplayName("CEP")]
        public string ZipCode { get; private set; }
        public override string ToString()
        {
            return $"{Street}, {Number} {Complement} - {Neighborhood} - {City} - {State}";
        }
    }

}
