using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Consumer.DomainServices
{
    public class DomainService : IDomainService
    {
        private readonly ILogger<DomainService> _logger;
        private readonly Common.Domain.IDomainEventPublisher _domainEventPublisher;
        private readonly Repositories.ICustomerRepository _customerRepository;
        private readonly Repositories.IOrderRepository _orderRepository;
        public DomainService(ILogger<DomainService> logger,
            Common.Domain.IDomainEventPublisher domainEventPublisher,
            Repositories.ICustomerRepository customerRepository,
            Repositories.IOrderRepository orderRepository)
        {
            _logger = logger;
            _domainEventPublisher = domainEventPublisher;
            _customerRepository = customerRepository;
            _orderRepository = orderRepository;
        }

        public async Task RegisterCustomer(Guid id, string number, IDictionary<string, int> initialStocks)
        {
            var customer = new DomainModels.Customer(id, number);
            if (initialStocks != null)
                foreach (var i in initialStocks)
                    customer.AddTransaction(Guid.NewGuid(), i.Key, i.Value, 0);

            await _customerRepository.Save(customer);
            await _domainEventPublisher.PublishEvents(customer.Events);

            _logger.LogInformation("{CustomerNumber} Registered as a Customer", customer.State.Number);
        }

        public async Task PutOrder(Guid id, Guid customerId, string stock, int units, double price)
        {
            var sellerCustomer = await _customerRepository.Load(customerId);

            var order = new DomainModels.Order(id, sellerCustomer, stock, units, price);

            await _orderRepository.Save(order);
            await _domainEventPublisher.PublishEvents(order.Events);

            _logger.LogInformation("{CustomerNumber} put an order with {Stock}({Units} * ${Price})",
                sellerCustomer.State.Number, order.State.Stock, order.State.Units, order.State.Price);
        }

        public async Task FillOrder(Guid orderId, Guid buyerCustomerId)
        {
            var order = await _orderRepository.Load(orderId);
            var sellerCustomer = await _customerRepository.Load(order.State.SellerCustomerId);
            var buyerCustomer = await _customerRepository.Load(buyerCustomerId);

            order.FillOrder(sellerCustomer, buyerCustomer);

            await _orderRepository.Save(order);
            await _customerRepository.Save(sellerCustomer);
            await _customerRepository.Save(buyerCustomer);

            await _domainEventPublisher.PublishEvents(order.Events);
            await _domainEventPublisher.PublishEvents(sellerCustomer.Events);
            await _domainEventPublisher.PublishEvents(buyerCustomer.Events);

            _logger.LogInformation(
                "{BuyerCustomer} filled order of {SellerCustomer} with {Stock}({Units} * ${Price})",
                buyerCustomer.State.Number, sellerCustomer.State.Number, order.State.Stock, order.State.Units, order.State.Price);
        }
    }
}
