using System;
using System.Linq;
using System.Collections.Generic;

namespace Common.Messages
{
    public class CustomerRegister : MessageBus.IMessage
    {
        public Guid Id { get; set; }
        public string Number { get; set; }
        public IDictionary<string, int> InitialStocks { get; set; }

        public override string ToString() => $"{nameof(CustomerRegister)}({Id}, {Number}, {InitialStocks.Keys.First()}, {InitialStocks[InitialStocks.Keys.First()]})";
    }
}
