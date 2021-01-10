using System;
using System.Collections.Generic;

namespace Common.Messages
{
    public class OrderPut : MessageBus.IMessage
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string Stock { get; set; }
        public int Units { get; set; }
        public double Price { get; set; }

        public override string ToString() => $"{nameof(OrderPut)}({Id}, {CustomerId}, {Stock}, {Units}, {Price})";
    }
}
