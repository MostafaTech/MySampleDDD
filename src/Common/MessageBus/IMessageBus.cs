using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Common.MessageBus
{
    public interface IMessageBus
    {
        void Send(object data);
        Task StartConsume(Type[] handlerTypes, Type[] middlewares = null, CancellationToken stoppingToken = default);
    }
}
