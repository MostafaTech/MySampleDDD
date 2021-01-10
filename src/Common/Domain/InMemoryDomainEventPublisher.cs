using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Domain
{
    public class InMemoryDomainEventPublisher : IDomainEventPublisher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Type[] _handlerTypes;
        public InMemoryDomainEventPublisher(IServiceProvider serviceProvider, Type[] handlerTypes)
        {
            _serviceProvider = serviceProvider;
            _handlerTypes = handlerTypes;
        }

        public async Task PublishEvents(IEnumerable<IDomainEvent> events)
        {
            foreach (var evt in events)
            {
                foreach (var tHandler in _handlerTypes)
                {
                    var handleMethod = Helpers.ReflectionHelpers.GetMessageHandlerHandleMethod(tHandler, evt.GetType());
                    if (handleMethod != null)
                    {
                        var instance = _serviceProvider.GetService(tHandler);
                        if (instance != null)
                        {
                            await Helpers.ReflectionHelpers.InvokeMethodAsync(handleMethod, instance, new object[] { evt });
                        }
                    }
                }
            }
        }
    }
}
