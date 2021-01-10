using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Consumer
{
    class ConsumerWorker : BackgroundService
    {
        private readonly ILogger<ConsumerWorker> _logger;
        private readonly Common.MessageBus.IMessageBus _messageBus;
        private readonly Common.Persistence.IUnitOfWork _unitOfWork;

        public ConsumerWorker(ILogger<ConsumerWorker> logger,
            Common.MessageBus.IMessageBus messageBus)
        {
            _logger = logger;
            _messageBus = messageBus;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var handlerTypes = new Type[] {
                typeof(ConsumerHandlers)
            };
            var middlewares = new Type[] {
                typeof(Middlewares.UnitOfWorkMiddleware)
            };
            return _messageBus.StartConsume(handlerTypes, middlewares, stoppingToken);
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            // delete all data before starting application
            //_unitOfWork.PurgeData("dbo.[Orders]", "dbo.[Transactions]", "dbo.[Customers]");

            _logger.LogDebug("Consumer Worker started");
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Consumer Worker stopped");
            return base.StopAsync(cancellationToken);
        }
    }
}
