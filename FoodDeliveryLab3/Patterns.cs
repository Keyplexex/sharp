using System;
using System.Collections.Generic;

namespace FoodDeliverySystem
{
    // STATE - l
    public interface IOrderState
    {
        OrderStatus Status { get; }
        void Process(IOrder order);
        void Cancel(IOrder order);
    }
    
    public class CreatedState : IOrderState
    {
        public OrderStatus Status => OrderStatus.Created;
        public void Process(IOrder order) => Console.WriteLine("Order accepted");
        public void Cancel(IOrder order) => Console.WriteLine("Order cancelled");
    }
    
    public class PreparingState : IOrderState
    {
        public OrderStatus Status => OrderStatus.Preparing;
        public void Process(IOrder order) => Console.WriteLine("Order is being prepared");
        public void Cancel(IOrder order) => Console.WriteLine("Order cancelled during preparation");
    }
    
    // STRATEGY - o
    public interface IPricingStrategy
    {
        decimal CalculateTotal(decimal subtotal);
        string GetDescription();
    }
    
    public class StandardPricing : IPricingStrategy
    {
        public decimal CalculateTotal(decimal subtotal) => subtotal + 5.0m;
        public string GetDescription() => "Standard delivery ($5)";
    }
    
    public class ExpressPricing : IPricingStrategy
    {
        public decimal CalculateTotal(decimal subtotal) => subtotal + 10.0m;
        public string GetDescription() => "Express delivery ($10)";
    }
    
    // FACTORY - d
    public interface IOrderFactory
    {
        IOrder CreateOrder(List<MenuItem> items, string customerName);
    }
    
    public class StandardOrderFactory : IOrderFactory
    {
        public IOrder CreateOrder(List<MenuItem> items, string customerName)
            => new Order(items, customerName, new StandardPricing());
    }
    
    public class ExpressOrderFactory : IOrderFactory
    {
        public IOrder CreateOrder(List<MenuItem> items, string customerName)
            => new Order(items, customerName, new ExpressPricing());
    }
    
    // OBSERVER
    public class CustomerNotifier : IOrderObserver
    {
        private readonly string _customerName;
        
        public CustomerNotifier(string customerName) => _customerName = customerName;
        
        public void Update(IOrder order)
        {
            Console.WriteLine($"Notification for {_customerName}: Order #{order.Id} status: {order.GetStatus()}");
        }
    }
}