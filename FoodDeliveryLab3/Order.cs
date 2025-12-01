using System;
using System.Collections.Generic;
using System.Linq;

namespace FoodDeliverySystem
{
    public class Order : IOrder
    {
        private static int _nextId = 1;
        private List<MenuItem> _items;
        private IPricingStrategy _pricingStrategy;
        private IOrderState _state;
        private List<IOrderObserver> _observers = new List<IOrderObserver>();
        
        public int Id { get; }
        public string CustomerName { get; }
        public DateTime OrderTime { get; }
        
        public Order(List<MenuItem> items, string customerName, IPricingStrategy pricingStrategy)
        {
            Id = _nextId++;
            CustomerName = customerName;
            OrderTime = DateTime.Now;
            _items = items ?? new List<MenuItem>();
            _pricingStrategy = pricingStrategy;
            _state = new CreatedState();
        }
        
        public void Process() => _state.Process(this);
        public void Cancel() => _state.Cancel(this);
        public OrderStatus GetStatus() => _state.Status;
        
        public void SetState(IOrderState state)
        {
            _state = state;
            NotifyObservers(); 
        }
        
        public void Attach(IOrderObserver observer) => _observers.Add(observer);
        public void Detach(IOrderObserver observer) => _observers.Remove(observer);
        
        private void NotifyObservers()
        {
            foreach (var observer in _observers)
            {
                observer.Update(this);
            }
        }
        
        public decimal CalculateSubtotal() => _items.Sum(item => item.Price);
        public decimal CalculateTotal() => _pricingStrategy.CalculateTotal(CalculateSubtotal());
        
        public string GetDescription()
        {
            var itemsDesc = string.Join("\n", _items.Select(i => $"- {i.Name}: ${i.Price}"));
            return $"Order #{Id} for {CustomerName}\n" +
                   $"Time: {OrderTime:HH:mm}\n" +
                   $"Status: {_state.Status}\n" +
                   $"Items:\n{itemsDesc}\n" +
                   $"Subtotal: ${CalculateSubtotal()}\n" +
                   $"Total: ${CalculateTotal()}";
        }
        
        public List<MenuItem> GetItems() => new List<MenuItem>(_items);
    }
    
    // DECORATOR - s
    public abstract class OrderDecorator : IOrder
    {
        protected IOrder _order;
        
        protected OrderDecorator(IOrder order) => _order = order;
        
        public virtual int Id => _order.Id;
        public virtual string CustomerName => _order.CustomerName;
        public virtual List<MenuItem> GetItems() => _order.GetItems();
        public virtual decimal CalculateTotal() => _order.CalculateTotal();
        public virtual OrderStatus GetStatus() => _order.GetStatus();
        public virtual void Process() => _order.Process();
        public virtual void Cancel() => _order.Cancel();
        
        public virtual void Attach(IOrderObserver observer) => _order.Attach(observer);
        public virtual void Detach(IOrderObserver observer) => _order.Detach(observer);
        
        public virtual string GetDescription() => _order.GetDescription();
    }
    
    public class GiftWrapDecorator : OrderDecorator
    {
        private readonly decimal _wrapCost;
        
        public GiftWrapDecorator(IOrder order, decimal wrapCost = 2.5m) : base(order)
            => _wrapCost = wrapCost;
        
        public override decimal CalculateTotal() => _order.CalculateTotal() + _wrapCost;
        public override string GetDescription() => 
            _order.GetDescription() + $"\n+ Gift wrap: ${_wrapCost}";
    }
    
    public class SpecialInstructionsDecorator : OrderDecorator
    {
        private readonly string _instructions;
        
        public SpecialInstructionsDecorator(IOrder order, string instructions) : base(order)
            => _instructions = instructions;
        
        public override string GetDescription() => 
            _order.GetDescription() + $"\nSpecial instructions: {_instructions}";
    }
}