using System;
using System.Collections.Generic;
using Common.Domain;

namespace Consumer.DomainEvents
{
    public class CustomerTransactionCreated : IDomainEvent
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Stock { get; set; }
        public int Units { get; set; }
        public double Price { get; set; }
    }
}
