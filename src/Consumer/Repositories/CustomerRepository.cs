using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Common.Persistence;
using Consumer.DomainModels;

namespace Consumer.Repositories
{
    public class CustomerRepository : DomainRepository<Customer, Entities.Customer>, ICustomerRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public CustomerRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override Task<Customer> Load(Guid id)
        {
            var data = _unitOfWork.FindById<Entities.Customer>(id);
            return Task.FromResult(new Customer(data));
        }

        public override async Task Save(Customer entry)
        {
            await base.Save(entry);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task Apply(DomainEvents.CustomerRegistered evt, Customer entry)
        {
            await _unitOfWork.AddAsync(new Entities.Customer
            {
                Id = evt.Id,
                Number = evt.Number
            });
        }

        public async Task Apply(DomainEvents.CustomerTransactionCreated evt, Customer entry)
        {
            await _unitOfWork.AddAsync(new Entities.Transaction
            {
                Id = evt.Id,
                CustomerId = evt.CustomerId,
                CreatedDate = evt.CreatedDate,
                Stock = evt.Stock,
                Units = evt.Units,
                Price = evt.Price
            });
        }

        public Task<Entities.Transaction[]> GetTransactions(Guid customerId)
        {
            var data = _unitOfWork.FindAll<Entities.Transaction>()
                .Where(x => x.CustomerId == customerId).ToArray();
            return Task.FromResult(data);
        }
    }
}
