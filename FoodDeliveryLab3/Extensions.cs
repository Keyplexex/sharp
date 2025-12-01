using System;
using System.Collections.Generic;

namespace FoodDeliverySystem
{
    
    public class OrderBuilder
    {
        private List<MenuItem> _items = new List<MenuItem>();
        private string? _customerName;  
        private IPricingStrategy _pricingStrategy = new StandardPricing();
        private string? _instructions; 
        private bool _addGiftWrap;
        private decimal _giftWrapCost = 2.5m;
        
        public OrderBuilder SetCustomer(string name)
        {
            _customerName = name ?? throw new ArgumentNullException(nameof(name));
            return this;
        }
        
        public OrderBuilder AddItem(MenuItem item)
        {
            _items.Add(item ?? throw new ArgumentNullException(nameof(item)));
            return this;
        }
        
        public OrderBuilder AddItems(params MenuItem[] items)
        {
            foreach (var item in items)
                AddItem(item);
            return this;
        }
        
        public OrderBuilder SetExpressDelivery()
        {
            _pricingStrategy = new ExpressPricing();
            return this;
        }
        
        public OrderBuilder SetStandardDelivery()
        {
            _pricingStrategy = new StandardPricing();
            return this;
        }
        
        public OrderBuilder AddInstructions(string instructions)
        {
            _instructions = instructions;
            return this;
        }
        
        public OrderBuilder AddGiftWrap(decimal cost = 2.5m)
        {
            _addGiftWrap = true;
            _giftWrapCost = cost;
            return this;
        }
        
        public IOrder Build()
        {
            if (string.IsNullOrWhiteSpace(_customerName))
                throw new InvalidOperationException("Не указано имя клиента");
            
            if (_items.Count == 0)
                throw new InvalidOperationException("Заказ не содержит товаров");
            
            IOrder order = new Order(_items, _customerName, _pricingStrategy);
            
            if (_addGiftWrap)
                order = new GiftWrapDecorator(order, _giftWrapCost);
            
            if (!string.IsNullOrEmpty(_instructions))
                order = new SpecialInstructionsDecorator(order, _instructions!);
            
            return order;
        }
    }
}