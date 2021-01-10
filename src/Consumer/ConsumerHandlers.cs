using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Common.Messages;

namespace Consumer
{
    class ConsumerHandlers :
        Common.MessageBus.IMessageHandler<CustomerRegister>,
        Common.MessageBus.IMessageHandler<OrderPut>,
        Common.MessageBus.IMessageHandler<OrderFill>
    {
        private readonly ILogger<ConsumerHandlers> _logger;
        private readonly DomainServices.IDomainService _domainService;
        public ConsumerHandlers(ILogger<ConsumerHandlers> logger,
            DomainServices.IDomainService domainService)
        {
            _logger = logger;
            _domainService = domainService;
        }

        public async Task Handle(CustomerRegister message)
        {
            await _domainService.RegisterCustomer(message.Id, message.Number, message.InitialStocks);
        }

        public async Task Handle(OrderPut message)
        {
            await _domainService.PutOrder(message.Id, message.CustomerId,
                message.Stock, message.Units, message.Price);
        }

        public async Task Handle(OrderFill message)
        {
            await _domainService.FillOrder(message.OrderId, message.BuyerCustomerId);
        }
    }
}
