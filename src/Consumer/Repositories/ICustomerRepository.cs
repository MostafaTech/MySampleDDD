using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Common.Persistence;
using Consumer.DomainModels;

namespace Consumer.Repositories
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<Entities.Transaction[]> GetTransactions(Guid customerId);
    }
}
