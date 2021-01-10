using System;
using System.Collections.Generic;
using Common.Domain;

namespace Consumer.DomainModels
{
    public class Order : AggregateRoot<Entities.Order>
    {
        public Order(Entities.Order state)
            : base(state) { }

        public Order(Guid id, Customer sellerCustomer, string stock, int units, double price)
        {
            if (id == null || id == Guid.Empty)
                throw new ArgumentException("Order Id should be set");
            if (sellerCustomer == null)
                throw new ArgumentException("Seller Customer should be set");
            if (string.IsNullOrWhiteSpace(stock))
                throw new ArgumentException("Stock should be set");
            if (units <= 0)
                throw new ArgumentException("Units should be greater than zero");
            if (price <= 0)
                throw new ArgumentException("Price should be greater than zero");

            var sellerStockUnits = sellerCustomer.CalculateStockUnits(stock);
            if (sellerStockUnits < units)
                throw new ApplicationException($"{sellerCustomer.State.Number} has {sellerStockUnits} {stock} stocks which is not enought to put an order with {units} units");

            Publish(new DomainEvents.OrderCreated
            {
                Id = id,
                SellerCustomerId = sellerCustomer.State.Id,
                Stock = stock,
                Units = units,
                Price = price
            });
        }

        public void FillOrder(Customer sellerCustomer, Customer buyerCustomer)
        {
            if (sellerCustomer == null)
                throw new ArgumentException("Seller Customer should be set");
            if (buyerCustomer == null)
                throw new ArgumentException("Buyer Customer should be set");
            if (State.Filled == true)
                throw new ApplicationException("Filled Order cant be fill again");

            var sellerStockUnits = sellerCustomer.CalculateStockUnits(State.Stock);
            if (sellerStockUnits < State.Units)
                throw new ApplicationException($"{sellerCustomer.State.Number} has {sellerStockUnits} {State.Stock} stocks which is not enought to sell {State.Units} to {buyerCustomer.State.Number}");

            sellerCustomer.AddTransaction(Guid.NewGuid(), State.Stock, -State.Units, State.Price);
            buyerCustomer.AddTransaction(Guid.NewGuid(), State.Stock, State.Units, State.Price);

            Publish(new DomainEvents.OrderFilled
            {
                Id = State.Id,
                SellerCustomerId = sellerCustomer.State.Id,
                BuyerCustomerId = buyerCustomer.State.Id,
                Stock = State.Stock,
                Units = State.Units,
                Price = State.Price
            });
        }

        public void Apply(DomainEvents.OrderCreated evt)
        {
            State = new Entities.Order
            {
                Id = evt.Id,
                SellerCustomerId = evt.SellerCustomerId,
                Stock = evt.Stock,
                Units = evt.Units,
                Price = evt.Price,
                Filled = false
            };
        }

        public void Apply(DomainEvents.OrderFilled evt)
        {
            State.BuyerCustomerId = evt.BuyerCustomerId;
            State.Filled = true;
        }
    }
}
