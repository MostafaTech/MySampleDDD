using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;
using Consumer.DomainModels;

namespace UnitTests
{
    public class DomainServiceTests
    {
        [Fact]
        public async Task RegisterCustomer()
        {
            var customerId = Guid.NewGuid();
            var customerInitialStocks = new Dictionary<string, int>()
            {
                { "AAPL", 100 }
            };

            var customerRepository = new Fakes.FakeCustomerRepository();
            var service = new Consumer.DomainServices.DomainService(
                Mock.Of<ILogger<Consumer.DomainServices.DomainService>>(),
                Mock.Of<Common.Domain.IDomainEventPublisher>(),
                customerRepository, null);

            await service.RegisterCustomer(customerId, "CUST001", customerInitialStocks);

            var customer = await customerRepository.Load(customerId);
            Assert.True(customer != null && customer.State.Transactions.Count == 1);
        }

        [Fact]
        public async Task PutSuccessfullOrder()
        {
            var orderId = Guid.NewGuid();
            var sellerCustomer = new Customer(Guid.NewGuid(), "CUST001");
            sellerCustomer.AddTransaction(Guid.NewGuid(), "AAPL", 100, 0);

            var customerRepository = new Mock<Consumer.Repositories.ICustomerRepository>();
            customerRepository.Setup(x => x.Load(sellerCustomer.State.Id)).Returns(Task.FromResult(sellerCustomer));

            var orderRepository = new Fakes.FakeOrderRepository();

            var service = new Consumer.DomainServices.DomainService(
                Mock.Of<ILogger<Consumer.DomainServices.DomainService>>(),
                Mock.Of<Common.Domain.IDomainEventPublisher>(),
                customerRepository.Object, orderRepository);
            await service.PutOrder(orderId, sellerCustomer.State.Id, "AAPL", 75, 5);

            var order = await orderRepository.Load(orderId);
            Assert.True(order != null);
        }

        [Fact]
        public async Task PutInvalidOrder()
        {
            var orderId = Guid.NewGuid();
            var sellerCustomer = new Customer(Guid.NewGuid(), "CUST001");
            sellerCustomer.AddTransaction(Guid.NewGuid(), "AAPL", 100, 0);

            var customerRepository = new Mock<Consumer.Repositories.ICustomerRepository>();
            customerRepository.Setup(x => x.Load(sellerCustomer.State.Id)).Returns(Task.FromResult(sellerCustomer));

            var orderRepository = new Fakes.FakeOrderRepository();

            var service = new Consumer.DomainServices.DomainService(
                Mock.Of<ILogger<Consumer.DomainServices.DomainService>>(),
                Mock.Of<Common.Domain.IDomainEventPublisher>(),
                customerRepository.Object, orderRepository);

            Assert.Throws<ApplicationException>(() =>
                service.PutOrder(orderId, sellerCustomer.State.Id, "AAPL", 120, 5).GetAwaiter().GetResult());
        }

        [Fact]
        public async Task FillOrder()
        {
            var sellerCustomer = new Customer(Guid.NewGuid(), "CUST001");
            sellerCustomer.AddTransaction(Guid.NewGuid(), "AAPL", 100, 0);
            var buyerCustomer = new Customer(Guid.NewGuid(), "CUST002");
            var order = new Order(Guid.NewGuid(), sellerCustomer, "AAPL", 75, 5);

            var customerRepository = new Mock<Consumer.Repositories.ICustomerRepository>();
            customerRepository.Setup(x => x.Load(sellerCustomer.State.Id)).Returns(Task.FromResult(sellerCustomer));
            customerRepository.Setup(x => x.Load(buyerCustomer.State.Id)).Returns(Task.FromResult(buyerCustomer));

            var orderRepository = new Fakes.FakeOrderRepository();
            orderRepository.Store.Add(order.State.Id, order.State);

            var service = new Consumer.DomainServices.DomainService(
                Mock.Of<ILogger<Consumer.DomainServices.DomainService>>(), 
                Mock.Of<Common.Domain.IDomainEventPublisher>(), 
                customerRepository.Object,
                orderRepository);

            await service.FillOrder(order.State.Id, buyerCustomer.State.Id);

            Assert.True(order.State.Filled);
        }
    }
}
