using System;


namespace ESE.Clients.API.Models
{
    public class Address
    {
        public Guid ClientId { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public string Complement { get; set; }
        public string Neighborhood { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }

        // EF Relation
        public Client Client { get; set; }

        public Address(string street, string number, string complement, string neighborhood, string city, string state, string zipcode)
        {
            Street = street;
            Number = number;
            Complement = complement;
            Neighborhood = neighborhood;
            City = city;
            State = state;
            ZipCode = zipcode;
        }
    }
}
