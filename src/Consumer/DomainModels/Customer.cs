using System;
using System.Linq;
using System.Collections.Generic;
using Common.Domain;

namespace Consumer.DomainModels
{
    public class Customer : AggregateRoot<Entities.Customer>
    {
        public Customer(Entities.Customer state)
        {
            State = state;
        }

        public Customer(Guid id, string number)
        {
            if (id == null || id == Guid.Empty)
                throw new ArgumentException("Customer Id should be set");
            if (string.IsNullOrWhiteSpace(number))
                throw new ArgumentException("Customer Number should be set");

            Publish(new DomainEvents.CustomerRegistered { Id = id, Number = number });
        }

        public void AddTransaction(Guid id, string stock, int units, double price)
        {
            if (id == null || id == Guid.Empty)
                throw new ArgumentException("Transaction Id should be set");
            if (string.IsNullOrWhiteSpace(stock))
                throw new ArgumentException("Transaction Stock should be set");
            if (units == 0)
                throw new ArgumentException("Transaction Units should not be zero");
            if (price < 0)
                throw new ArgumentException("Transaction Price should be greater or equal to zero");

            Publish(new DomainEvents.CustomerTransactionCreated
            {
                Id = id,
                CustomerId = State.Id,
                CreatedDate = DateTime.Now,
                Stock = stock,
                Units = units,
                Price = price
            });
        }

        public void Apply(DomainEvents.CustomerRegistered evt)
        {
            State = new Entities.Customer
            {
                Id = evt.Id,
                Number = evt.Number
            };
        }

        public void Apply(DomainEvents.CustomerTransactionCreated evt)
        {
            State.Transactions.Add(new Entities.Transaction
            {
                Id = evt.Id,
                CustomerId = evt.CustomerId,
                CreatedDate = evt.CreatedDate,
                Stock = evt.Stock,
                Units = evt.Units,
                Price = evt.Price,
            });
        }

        public IReadOnlyDictionary<string, int> CalculateStocks()
        {
            var stocks = State.Transactions.Select(x => x.Stock).Distinct();
            return stocks.ToDictionary(k => k, v =>
                State.Transactions.Where(t => t.Stock == v).Select(t => t.Units).DefaultIfEmpty().Sum());
        }
        public int CalculateStockUnits(string stock)
        {
            var stocks = State.Transactions.Select(x => x.Stock).Distinct();
            return State.Transactions
                .Where(x => x.Stock == stock)
                .Select(t => t.Units).DefaultIfEmpty().Sum();
        }
    }
}
