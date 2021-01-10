using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Consumer.DomainServices
{
    public interface IDomainService
    {
        Task RegisterCustomer(Guid id, string number, IDictionary<string, int> initialStocks);

        Task PutOrder(Guid id, Guid customerId, string stock, int units, double price);

        Task FillOrder(Guid orderId, Guid buyerCustomerId);
    }
}
