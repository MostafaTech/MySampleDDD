using System;
using System.Collections.Generic;

namespace Consumer.Entities
{
    public class Customer
    {
        public Guid Id { get; set; }
        public string Number { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }

        public Customer()
        {
            Transactions = new List<Transaction>();
        }
    }
}
