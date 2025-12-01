using System.Collections.Generic;

namespace FoodDeliverySystem
{
    public enum OrderStatus { Created, Preparing, ReadyForDelivery, OnTheWay, Delivered, Cancelled }
    
    public class MenuItem
    {
        public int Id { get; }
        public string Name { get; }
        public decimal Price { get; }
        
        public MenuItem(int id, string name, decimal price)
        {
            Id = id;
            Name = name;
            Price = price;
        }
    }
    
    public interface IOrder
    {
        int Id { get; }
        string CustomerName { get; }
        decimal CalculateTotal();
        string GetDescription();
        OrderStatus GetStatus();
        void Process();
        void Cancel();
        List<MenuItem> GetItems();
        void Attach(IOrderObserver observer);
        void Detach(IOrderObserver observer);
    }
    
    public interface IOrderObserver
    {
        void Update(IOrder order);
    }
}