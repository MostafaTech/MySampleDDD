using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Common.MessageBus
{
    public interface IMessageConsumerMiddleware
    {
        Task Before();
        Task After(Exception ex);
    }
}
