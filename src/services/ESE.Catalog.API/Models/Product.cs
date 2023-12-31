﻿using ESE.Core.DomainObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESE.Catalog.API.Models
{
    public class Product : Entity, IAggregateRoot
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Image { get; set; }
        public int Stock { get; set; }
        public bool Available(int quantity)
        {
            return Active && Stock >= quantity;
        }
        public void DecrementStock(int quantity)
        {
            if (Stock >= quantity)
              Stock -= quantity;
        }
    }    
}
