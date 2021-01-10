using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Common.MessageBus;
using Common.Persistence;

namespace Consumer.Middlewares
{
    public class UnitOfWorkMiddleware : IMessageConsumerMiddleware
    {
        private readonly ILogger<UnitOfWorkMiddleware> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public UnitOfWorkMiddleware(ILogger<UnitOfWorkMiddleware> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public Task Before()
        {
            _unitOfWork.BeginTransaction();
            return Task.CompletedTask;
        }

        public Task After(Exception ex)
        {
            if (ex == null)
            {
                _unitOfWork.CommitTransaction();
            }
            else
            {
                _unitOfWork.RollbackTransaction();

                if (ex is ArgumentException)
                    _logger.LogError("ArgumentException: {ErrorMessage}", ex.Message);
                else if (ex is ApplicationException)
                    _logger.LogError("ApplicationException: {ErrorMessage}", ex.Message);
                else
                    _logger.LogError(ex, "Error thrown during a request: {ErrorMessage}", ex.Message);
            }
            return Task.CompletedTask;
        }
    }
}
