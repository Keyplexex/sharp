using System;
using System.Collections.Generic;

namespace InventoryGame.Inventory
{
    public abstract class ItemBase : IItem
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name { get; set; } = "Unnamed";
        public string Description { get; set; } = "";
        public abstract ItemType Type { get; }
        public IItemState State { get; set; } = new InInventoryState();
        public IUseBehavior UseBehavior { get; set; } = new NoOpBehavior();

        protected readonly Dictionary<string, double> attributes = new(); // доступ в классах и наследниках + ссылка на словарь не может быть изменена
        public IReadOnlyDictionary<string, double> Attributes => attributes;

        public abstract IItem Clone();
    }

    public class Weapon : ItemBase
    {
        public Weapon(string name, string description, double damage, double weight)
        {
            Name = name;
            Description = description;
            attributes["Damage"] = damage;
            attributes["Weight"] = weight;
            UseBehavior = new EquipBehavior();
        }

        public override ItemType Type => ItemType.Weapon;

        public override IItem Clone()
        {
            return new Weapon(Name, Description,
                attributes["Damage"],
                attributes["Weight"])
            {
                State = new InInventoryState()
            };
        }
    }

    public class Armor : ItemBase
    {
        public Armor(string name, string description, double defense, double weight)
        {
            Name = name;
            Description = description;
            attributes["Defense"] = defense;
            attributes["Weight"] = weight;
            UseBehavior = new EquipBehavior();
        }

        public override ItemType Type => ItemType.Armor;

        public override IItem Clone()
        {
            return new Armor(Name, Description,
                attributes["Defense"],
                attributes["Weight"])
            {
                State = new InInventoryState()
            };
        }
    }

    public class Potion : ItemBase
    {
        public Potion(string name, string description, double healAmount)
        {
            Name = name;
            Description = description;
            attributes["Heal"] = healAmount;
            UseBehavior = new ConsumeBehavior();
        }

        public override ItemType Type => ItemType.Potion;

        public override IItem Clone()
        {
            return new Potion(Name, Description,
                attributes["Heal"])
            {
                State = new InInventoryState()
            };
        }
    }

    public class QuestItem : ItemBase
    {
        public QuestItem(string name, string description)
        {
            Name = name;
            Description = description;
            UseBehavior = new NoOpBehavior();
        }

        public override ItemType Type => ItemType.QuestItem;

        public override IItem Clone()
        {
            return new QuestItem(Name, Description)
            {
                State = new InInventoryState()
            };
        }
    }
}
