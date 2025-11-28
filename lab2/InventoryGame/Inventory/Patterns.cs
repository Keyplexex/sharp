using System;
using System.Collections.Generic;
using System.Linq;

namespace InventoryGame.Inventory
{
    public enum ItemType { Weapon, Armor, Potion, QuestItem }

    public interface IItem
    {
        Guid Id { get; }
        string Name { get; }
        string Description { get; }
        ItemType Type { get; }
        IItemState State { get; set; } 
        IUseBehavior UseBehavior { get; } 
        IItem Clone();
        IReadOnlyDictionary<string, double> Attributes { get; }
    }

    public interface IItemState //S
    {
        string Name { get; }
        void Equip(IItem item, PlayerInventory inventory);
        void Unequip(IItem item, PlayerInventory inventory);
        void Consume(IItem item, PlayerInventory inventory);
    }

    public class InInventoryState : IItemState
    {
        public string Name => "InInventory";

        public void Equip(IItem item, PlayerInventory inventory) =>
            inventory.InternalEquip(item);

        public void Unequip(IItem item, PlayerInventory inventory) =>
            throw new InvalidOperationException("Cannot unequip item that is not equipped.");

        public void Consume(IItem item, PlayerInventory inventory) =>
            inventory.InternalConsume(item);
    }

    public class EquippedState : IItemState
    {
        public string Name => "Equipped";

        public void Equip(IItem item, PlayerInventory inventory) =>
            throw new InvalidOperationException("Item already equipped.");

        public void Unequip(IItem item, PlayerInventory inventory) =>
            inventory.InternalUnequip(item);

        public void Consume(IItem item, PlayerInventory inventory) =>
            throw new InvalidOperationException("Cannot consume equipped item.");
    }

    public class ConsumedState : IItemState
    {
        public string Name => "Consumed";

        public void Equip(IItem item, PlayerInventory inventory) =>
            throw new InvalidOperationException("Cannot equip consumed item.");

        public void Unequip(IItem item, PlayerInventory inventory) =>
            throw new InvalidOperationException("Cannot unequip consumed item.");

        public void Consume(IItem item, PlayerInventory inventory) =>
            throw new InvalidOperationException("Item already consumed.");
    }

    public interface IUseBehavior // S
    {
        UseResult Use(IItem item, PlayerInventory inventory);
    }

    public class UseResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";

        public static UseResult Ok(string msg = "") => new() { Success = true, Message = msg };
        public static UseResult Fail(string msg) => new() { Success = false, Message = msg };
    }

    public class EquipBehavior : IUseBehavior
    {
        public UseResult Use(IItem item, PlayerInventory inventory)
        {
            try
            {
                item.State.Equip(item, inventory);
                return UseResult.Ok($"Equipped {item.Name}");
            }
            catch (Exception ex)
            {
                return UseResult.Fail(ex.Message);
            }
        }
    }

    public class ConsumeBehavior : IUseBehavior
    {
        public UseResult Use(IItem item, PlayerInventory inventory)
        {
            try
            {
                item.State.Consume(item, inventory);
                return UseResult.Ok($"Consumed {item.Name}");
            }
            catch (Exception ex)
            {
                return UseResult.Fail(ex.Message);
            }
        }
    }

    public class NoOpBehavior : IUseBehavior
    {
        public UseResult Use(IItem item, PlayerInventory inventory) =>
            UseResult.Fail("This item cannot be used.");
    }

    public interface IItemBuilder
    {
        IItemBuilder SetName(string name);
        IItemBuilder SetDescription(string desc);
        IItemBuilder SetAttribute(string key, double value);
        IItem Build();
    }

    public class GenericItemBuilder : IItemBuilder
    {
        private string _name = "Unnamed";
        private string _desc = "";
        private readonly ItemType _type;
        private readonly Dictionary<string, double> _attrs = new();

        public GenericItemBuilder(ItemType type) => _type = type;

        public IItemBuilder SetName(string name)
        {
            _name = name;
            return this;
        }

        public IItemBuilder SetDescription(string desc)
        {
            _desc = desc;
            return this;
        }

        public IItemBuilder SetAttribute(string key, double value)
        {
            _attrs[key] = value;
            return this;
        }

        public IItem Build()
        {
            return _type switch
            {
                ItemType.Weapon => new Weapon(
                    _name, _desc,
                    _attrs.TryGetValue("Damage", out var d) ? d : 1.0,
                    _attrs.TryGetValue("Weight", out var w) ? w : 1.0),

                ItemType.Armor => new Armor(
                    _name, _desc,
                    _attrs.TryGetValue("Defense", out var a) ? a : 1.0,
                    _attrs.TryGetValue("Weight", out var w2) ? w2 : 1.0),

                ItemType.Potion => new Potion(
                    _name, _desc,
                    _attrs.TryGetValue("Heal", out var h) ? h : 10.0),

                ItemType.QuestItem => new QuestItem(_name, _desc),

                _ => throw new InvalidOperationException("Unknown item type")
            };
        }
    }

    public interface IItemFactory
    {
        IItem CreateBasicWeapon();
        IItem CreateBasicArmor();
        IItem CreateHealingPotion();
        IItem CreateQuestItem(string name);
    }

    public class MedievalItemFactory : IItemFactory
    {
        public IItem CreateBasicWeapon() =>
            new Weapon("Iron Sword", "Basic iron sword.", 10, 5);

        public IItem CreateBasicArmor() =>
            new Armor("Leather Armor", "Light armor.", 5, 8);

        public IItem CreateHealingPotion() =>
            new Potion("Minor Healing", "Restores a bit of HP.", 25);

        public IItem CreateQuestItem(string name) =>
            new QuestItem(name, "Quest item.");
    }
}
