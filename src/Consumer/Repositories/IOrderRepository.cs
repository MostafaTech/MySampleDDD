using System;
using System.Collections.Generic;
using Common.Persistence;
using Consumer.DomainModels;

namespace Consumer.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
    }
}
