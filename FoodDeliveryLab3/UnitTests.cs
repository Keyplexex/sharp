using Xunit;
using System;
using System.Collections.Generic;

namespace FoodDeliverySystem.Tests
{
    public static class TestData
    {
        public static MenuItem Pizza => new MenuItem(1, "Pizza", 10.0m);
        public static MenuItem Burger => new MenuItem(2, "Burger", 5.0m);
        public static MenuItem Salad => new MenuItem(3, "Salad", 6.5m);
    }

    public class OrderTests
    {
        [Fact]
        public void Order_CalculateSubtotal_ReturnsCorrectSum()
        {
            var items = new List<MenuItem> { TestData.Pizza, TestData.Burger };
            var order = new Order(items, "Test", new StandardPricing());

            var subtotal = order.CalculateSubtotal();

            Assert.Equal(15.0m, subtotal);
        }

        [Fact]
        public void Order_CalculateTotal_WithStandardPricing_AddsDeliveryFee()
        {
            var items = new List<MenuItem> { TestData.Pizza };
            var order = new Order(items, "Test", new StandardPricing());

            var total = order.CalculateTotal();

            Assert.Equal(15.0m, total);
        }

        [Fact]
        public void Order_CalculateTotal_WithExpressPricing_AddsExpressFee()
        {
            var items = new List<MenuItem> { TestData.Pizza };
            var order = new Order(items, "Test", new ExpressPricing());

            var total = order.CalculateTotal();

            Assert.Equal(20.0m, total); // 10 + 10
        }

        [Fact]
        public void Order_GetDescription_ContainsCustomerName()
        {
            var order = new Order(new List<MenuItem> { TestData.Pizza }, "John Doe", new StandardPricing());

            var description = order.GetDescription();

            Assert.Contains("John Doe", description);
        }

        [Fact]
        public void Order_GetDescription_ContainsItems()
        {
            var order = new Order(new List<MenuItem> { TestData.Pizza, TestData.Salad }, "Test", new StandardPricing());

            var description = order.GetDescription();

            Assert.Contains("Pizza", description);
            Assert.Contains("Salad", description);
            Assert.Contains("$10", description);
            Assert.Contains("$6.5", description);
        }

        [Fact]
        public void Order_InitialStatus_IsCreated()
        {
            var order = new Order(new List<MenuItem> { TestData.Pizza }, "Test", new StandardPricing());

            Assert.Equal(OrderStatus.Created, order.GetStatus());
        }

        [Fact]
        public void Order_GetItems_ReturnsCopyOfItems()
        {
            var items = new List<MenuItem> { TestData.Pizza };
            var order = new Order(items, "Test", new StandardPricing());

            var returnedItems = order.GetItems();

            Assert.NotSame(items, returnedItems); //копирование
            Assert.Equal(items.Count, returnedItems.Count);
        }
    }

    public class PricingStrategyTests
    {
        [Fact]
        public void StandardPricing_CalculateTotal_Adds5ToSubtotal()
        {
            var strategy = new StandardPricing();

            var result = strategy.CalculateTotal(20.0m);

            Assert.Equal(25.0m, result);
        }

        [Fact]
        public void ExpressPricing_CalculateTotal_Adds10ToSubtotal()
        {
            var strategy = new ExpressPricing();

            var result = strategy.CalculateTotal(20.0m);

            Assert.Equal(30.0m, result); // 20 + 10
        }

        [Fact]
        public void StandardPricing_GetDescription_ReturnsCorrectString()
        {
            var strategy = new StandardPricing();

            var description = strategy.GetDescription();

            Assert.Contains("Standard", description);
            Assert.Contains("$5", description);
        }

        [Fact]
        public void ExpressPricing_GetDescription_ReturnsCorrectString()
        {
            var strategy = new ExpressPricing();

            var description = strategy.GetDescription();

            Assert.Contains("Express", description);
            Assert.Contains("$10", description);
        }
    }

    public class DecoratorTests
    {
        [Fact]
        public void GiftWrapDecorator_AddsWrapCostToTotal()
        {
            var order = new Order(new List<MenuItem> { TestData.Pizza }, "Test", new StandardPricing());
            var baseTotal = order.CalculateTotal(); // 10 + 5 = 15

            var decoratedOrder = new GiftWrapDecorator(order, 2.5m);
            var decoratedTotal = decoratedOrder.CalculateTotal();

            Assert.Equal(baseTotal + 2.5m, decoratedTotal);
        }

        [Fact]
        public void GiftWrapDecorator_GetDescription_ContainsWrapInfo()
        {
            var order = new Order(new List<MenuItem> { TestData.Pizza }, "Test", new StandardPricing());

            var decoratedOrder = new GiftWrapDecorator(order, 3.0m);
            var description = decoratedOrder.GetDescription();

            Assert.Contains("Gift wrap", description);
            Assert.Contains("$3", description);
        }

        [Fact]
        public void SpecialInstructionsDecorator_GetDescription_ContainsInstructions()
        {
            var order = new Order(new List<MenuItem> { TestData.Pizza }, "Test", new StandardPricing());

            var decoratedOrder = new SpecialInstructionsDecorator(order, "No onions, extra cheese");
            var description = decoratedOrder.GetDescription();

            Assert.Contains("No onions, extra cheese", description);
        }

        [Fact]
        public void MultipleDecorators_CombineCorrectly()
        {
            var order = new Order(new List<MenuItem> { TestData.Pizza }, "Test", new StandardPricing());

            IOrder decorated = new GiftWrapDecorator(order, 2.5m);
            decorated = new SpecialInstructionsDecorator(decorated, "Extra sauce");
            var total = decorated.CalculateTotal();
            var description = decorated.GetDescription();

            Assert.True(total > order.CalculateTotal());
            Assert.Contains("Gift wrap", description);
            Assert.Contains("Extra sauce", description);
        }

        [Fact]
        public void Decorator_PreservesCustomerName()
        {
            var order = new Order(new List<MenuItem> { TestData.Pizza }, "John", new StandardPricing());

            var decorated = new GiftWrapDecorator(order);

            Assert.Equal("John", decorated.CustomerName);
            Assert.Equal(order.Id, decorated.Id);
        }
    }

    public class FactoryTests
    {
        [Fact]
        public void StandardOrderFactory_CreatesOrderWithStandardPricing()
        {
            IOrderFactory factory = new StandardOrderFactory();

            var order = factory.CreateOrder(new List<MenuItem> { TestData.Pizza }, "Test");

            Assert.NotNull(order);
            Assert.Equal("Test", order.CustomerName);
            Assert.Equal(15.0m, order.CalculateTotal()); // 10 + 5
        }

        [Fact]
        public void ExpressOrderFactory_CreatesOrderWithExpressPricing()
        {
            IOrderFactory factory = new ExpressOrderFactory();

            var order = factory.CreateOrder(new List<MenuItem> { TestData.Pizza }, "Test");

            Assert.Equal(20.0m, order.CalculateTotal()); // 10 + 10
        }

        [Fact]
        public void FactoryCreatedOrder_HasCorrectProperties()
        {
            IOrderFactory factory = new StandardOrderFactory();

            var order = factory.CreateOrder(new List<MenuItem> { TestData.Pizza, TestData.Burger }, "John");

            Assert.Equal("John", order.CustomerName);
            Assert.Equal(2, order.GetItems().Count);
            Assert.Equal(20.0m, order.CalculateTotal()); // 10 + 5 + 5 доставка
        }
    }

    public class StateTests
    {
        [Fact]
        public void Order_InitialStatus_IsCreated()
        {
            var order = new Order(new List<MenuItem> { TestData.Pizza }, "Test", new StandardPricing());

            Assert.Equal(OrderStatus.Created, order.GetStatus());
        }

        [Fact]
        public void Order_Process_CallsStateProcessMethod()
        {
            var order = new Order(new List<MenuItem> { TestData.Pizza }, "Test", new StandardPricing());

            var exception = Record.Exception(() => order.Process());

            Assert.Null(exception);
        }

        [Fact]
        public void Order_Cancel_CallsStateCancelMethod()
        {
            var order = new Order(new List<MenuItem> { TestData.Pizza }, "Test", new StandardPricing());

            var exception = Record.Exception(() => order.Cancel());

            Assert.Null(exception); 
        }
    }

    public class IntegrationTests
    {
        [Fact]
        public void CompleteOrderFlow_WorksCorrectly()
        {
            var items = new List<MenuItem> { TestData.Pizza, TestData.Burger };

            IOrderFactory factory = new StandardOrderFactory();
            var order = factory.CreateOrder(items, "Customer");

            IOrder decoratedOrder = new GiftWrapDecorator(order, 2.5m);
            decoratedOrder = new SpecialInstructionsDecorator(decoratedOrder, "Test instructions");

            var initialStatus = decoratedOrder.GetStatus();

            decoratedOrder.Process();
            decoratedOrder.Cancel();

            Assert.Equal(OrderStatus.Created, initialStatus);
            Assert.True(decoratedOrder.CalculateTotal() > 0);
            Assert.Contains("Customer", decoratedOrder.GetDescription());
            Assert.Contains("Test instructions", decoratedOrder.GetDescription());
        }

    }
}