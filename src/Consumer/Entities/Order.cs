using System;
using System.Collections.Generic;

namespace Consumer.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid SellerCustomerId { get; set; }
        public Guid? BuyerCustomerId { get; set; }
        public string Stock { get; set; }
        public int Units { get; set; }
        public double Price { get; set; }
        public bool Filled { get; set; }

        public virtual Customer SellerCustomer { get; set; }
        public virtual Customer BuyerCustomer { get; set; }
    }
}
