using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Consumer.DomainModels;

namespace UnitTests.Fakes
{
    class FakeOrderRepository : Consumer.Repositories.IOrderRepository
    {
        public Dictionary<Guid, Consumer.Entities.Order> Store { get; private set; } =
            new Dictionary<Guid, Consumer.Entities.Order>();

        public Task<Order> Load(Guid id)
        {
            var result = Store[id];
            return Task.FromResult(new Order(result));
        }

        public Task Save(Order entry)
        {
            if (Store.ContainsKey(entry.State.Id) == false)
                Store.Add(entry.State.Id, entry.State);
            return Task.CompletedTask;
        }
    }
}
