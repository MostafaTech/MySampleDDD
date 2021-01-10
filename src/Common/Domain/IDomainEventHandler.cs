using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Common.Domain
{
    public interface IDomainEventHandler<TEvent>
    {
        Task Handle(TEvent @event);
    }
}
