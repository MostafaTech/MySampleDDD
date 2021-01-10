using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Common.Domain;
using Consumer.DomainEvents;

namespace Consumer
{
    class DomainEventHandlers :
        IDomainEventHandler<CustomerRegistered>,
        IDomainEventHandler<CustomerTransactionCreated>,
        IDomainEventHandler<OrderCreated>,
        IDomainEventHandler<OrderFilled>
    {
        private readonly ILogger<DomainEventHandlers> _logger;
        public DomainEventHandlers(ILogger<DomainEventHandlers> logger)
        {
            _logger = logger;
        }

        public Task Handle(CustomerRegistered @event)
        {
            return Task.CompletedTask;
        }

        public Task Handle(CustomerTransactionCreated @event)
        {
            return Task.CompletedTask;
        }

        public Task Handle(OrderCreated @event)
        {
            return Task.CompletedTask;
        }

        public Task Handle(OrderFilled @event)
        {
            return Task.CompletedTask;
        }
    }
}
