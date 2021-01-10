using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Common.MessageBus
{
    public interface IMessageHandler<TMessage>
    {
        Task Handle(TMessage message);
    }
}
