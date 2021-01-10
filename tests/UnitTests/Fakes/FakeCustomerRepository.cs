using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Consumer.DomainModels;

namespace UnitTests.Fakes
{
    class FakeCustomerRepository : Consumer.Repositories.ICustomerRepository
    {
        private readonly Dictionary<Guid, Consumer.Entities.Customer> _store =
                     new Dictionary<Guid, Consumer.Entities.Customer>();

        public Task<Consumer.Entities.Transaction[]> GetTransactions(Guid customerId)
        {
            var result = _store[customerId].Transactions;
            return Task.FromResult(result.ToArray());
        }

        public Task<Customer> Load(Guid id)
        {
            var result = _store[id];
            return Task.FromResult(new Customer(result));
        }

        public Task Save(Customer entry)
        {
            if (_store.ContainsKey(entry.State.Id) == false)
                _store.Add(entry.State.Id, entry.State);
            return Task.CompletedTask;
        }
    }
}
