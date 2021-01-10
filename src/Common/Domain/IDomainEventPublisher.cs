using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Common.Domain
{
    public interface IDomainEventPublisher
    {
        Task PublishEvents(IEnumerable<IDomainEvent> events);
    }
}
