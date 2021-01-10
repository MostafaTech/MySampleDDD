using System;
using System.Collections.Generic;

namespace Consumer.Entities
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Stock { get; set; }
        public int Units { get; set; }
        public double Price { get; set; }
    }
}
