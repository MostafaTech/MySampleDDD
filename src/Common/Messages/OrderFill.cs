using System;
using System.Collections.Generic;

namespace Common.Messages
{
    public class OrderFill : MessageBus.IMessage
    {
        public Guid OrderId { get; set; }
        public Guid BuyerCustomerId { get; set; }

        public override string ToString() => $"{nameof(OrderFill)}({OrderId}, {BuyerCustomerId})";
    }
}
