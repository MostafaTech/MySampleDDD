using System;
using System.Collections.Generic;
using Common.Domain;

namespace Consumer.DomainEvents
{
    public class OrderFilled : IDomainEvent
    {
        public Guid Id { get; set; }
        public Guid SellerCustomerId { get; set; }
        public Guid BuyerCustomerId { get; set; }
        public string Stock { get; set; }
        public int Units { get; set; }
        public double Price { get; set; }
    }
}
