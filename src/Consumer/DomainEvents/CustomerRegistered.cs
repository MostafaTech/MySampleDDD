using System;
using System.Collections.Generic;
using Common.Domain;

namespace Consumer.DomainEvents
{
    public class CustomerRegistered : IDomainEvent
    {
        public Guid Id { get; set; }
        public string Number { get; set; }
    }
}
