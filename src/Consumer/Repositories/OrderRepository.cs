using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Common.Persistence;
using Consumer.DomainModels;

namespace Consumer.Repositories
{
    public class OrderRepository : DomainRepository<Order, Entities.Order>, IOrderRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override Task<Order> Load(Guid id)
        {
            var data = _unitOfWork.FindById<Entities.Order>(id);
            return Task.FromResult(new Order(data));
        }

        public override async Task Save(Order entry)
        {
            await base.Save(entry);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task Apply(DomainEvents.OrderCreated evt, Order order)
        {
            //await _unitOfWork.AddAsync(order);
            await _unitOfWork.AddAsync(new Entities.Order
            {
                Id = evt.Id,
                SellerCustomerId = evt.SellerCustomerId,
                Stock = evt.Stock,
                Units = evt.Units,
                Price = evt.Price,
                Filled = false
            });
        }

        public async Task Apply(DomainEvents.OrderFilled evt, Order order)
        {
            await _unitOfWork.UpdateAsync(order.State);
        }
    }
}
