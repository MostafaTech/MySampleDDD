using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Producer
{
    public class ProducerWorker : BackgroundService
    {
        private readonly ILogger<ProducerWorker> _logger;
        private readonly Common.MessageBus.IMessageBus _messageBus;

        public ProducerWorker(ILogger<ProducerWorker> logger,
            Common.MessageBus.IMessageBus messageBus)
        {
            _logger = logger;
            _messageBus = messageBus;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // give time to establish connection to the message bus
            await Task.Delay(2000, stoppingToken);

            foreach (var msg in produceMessages())
            {
                if (stoppingToken.IsCancellationRequested)
                    break;

                _messageBus.Send(msg);
                _logger.LogInformation("SENT: {Message}", msg);

                await Task.Delay(2500, stoppingToken);
            }
        }

        private IEnumerable<Common.MessageBus.IMessage> produceMessages()
        {
            for (var i = 0; ; i++)
            {
                var customers = new Guid[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
                var stocks = new string[] { "AAPL", "MSFT", "GOOG" };
                var orders = new Guid[] { Guid.NewGuid(), Guid.NewGuid() };

                // register all customers
                for (var ic = 0; ic < customers.Length; ic++)
                {
                    yield return new Common.Messages.CustomerRegister
                    {
                        Id = customers[ic],
                        Number = "CUST" + (i * customers.Length + ic + 1).ToString().PadLeft(3, '0'),
                        InitialStocks = new Dictionary<string, int>()
                        {
                            { stocks[ic], 10 }
                        }
                    };
                }

                yield return new Common.Messages.OrderPut
                {
                    Id = orders[0],
                    CustomerId = customers[0],
                    Stock = stocks[0],
                    Units = 2,
                    Price = 5.0
                };

                yield return new Common.Messages.OrderPut
                {
                    Id = orders[1],
                    CustomerId = customers[0],
                    Stock = stocks[0],
                    Units = 3,
                    Price = 5.0
                };

                yield return new Common.Messages.OrderFill
                {
                    OrderId = orders[0],
                    BuyerCustomerId = customers[1],
                };

                yield return new Common.Messages.OrderFill
                {
                    OrderId = orders[1],
                    BuyerCustomerId = customers[2],
                };
            }
        }
    }
}
