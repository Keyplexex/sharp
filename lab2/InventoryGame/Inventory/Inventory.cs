using System;
using System.Collections.Generic;
using System.Linq;

namespace InventoryGame.Inventory
{
    public class PlayerInventory
    {
        private readonly List<IItem> _items = new();
        private readonly List<IItem> _equipped = new();

        public IEnumerable<IItem> Items => _items.AsReadOnly();
        public IEnumerable<IItem> Equipped => _equipped.AsReadOnly();

        public void AddItem(IItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            item.State = new InInventoryState();
            _items.Add(item);
        }

        public void RemoveItem(IItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            _equipped.Remove(item);
            _items.Remove(item);
        }

        public UseResult UseItem(IItem item)
        {
            if (!_items.Contains(item))
                return UseResult.Fail("Item not in inventory.");

            return item.UseBehavior.Use(item, this);
        }

        public void InternalEquip(IItem item)
        {
            if (!_items.Contains(item))
                throw new InvalidOperationException("Item not in inventory.");
            if (_equipped.Contains(item))
                throw new InvalidOperationException("Item already equipped.");

            _equipped.Add(item);
            item.State = new EquippedState();
        }

        public void InternalUnequip(IItem item)
        {
            if (!_equipped.Contains(item))
                throw new InvalidOperationException("Item is not equipped.");
            _equipped.Remove(item);
            item.State = new InInventoryState();
        }

        public void InternalConsume(IItem item)
        {
            if (!_items.Contains(item))
                throw new InvalidOperationException("Item not in inventory.");

            _items.Remove(item);
            item.State = new ConsumedState();
        }

        public void UnequipItem(IItem item)
        {
            item.State.Unequip(item, this);
        }

        public IItem? FindByName(string name) =>
            _items.FirstOrDefault(i => i.Name == name);

        public IEnumerable<IItem> FindByType(ItemType type) =>
            _items.Where(i => i.Type == type);


        public IItem Combine(IItem baseItem, IItem material)
        {
            if (!_items.Contains(baseItem) || !_items.Contains(material))
                throw new InvalidOperationException("Both items must be in inventory.");

            IItem result;

            if (baseItem.Type == ItemType.Weapon && material.Type == ItemType.Weapon)
            {
                double d1 = baseItem.Attributes.TryGetValue("Damage", out var dv1) ? dv1 : 1;
                double d2 = material.Attributes.TryGetValue("Damage", out var dv2) ? dv2 : 0;
                double w1 = baseItem.Attributes.TryGetValue("Weight", out var wv1) ? wv1 : 1;
                double w2 = material.Attributes.TryGetValue("Weight", out var wv2) ? wv2 : 0;

                result = new Weapon(
                    baseItem.Name + "+" + material.Name,
                    "Combined weapon",
                    (d1 + d2) * 1.1,
                    w1 + w2
                );
            }
            else if (baseItem.Type == ItemType.Armor && material.Type == ItemType.Armor)
            {
                double a1 = baseItem.Attributes.TryGetValue("Defense", out var av1) ? av1 : 1;
                double a2 = material.Attributes.TryGetValue("Defense", out var av2) ? av2 : 0;
                double w1 = baseItem.Attributes.TryGetValue("Weight", out var wv1) ? wv1 : 1;
                double w2 = material.Attributes.TryGetValue("Weight", out var wv2) ? wv2 : 0;

                result = new Armor(
                    baseItem.Name + "+" + material.Name,
                    "Combined armor",
                    (a1 + a2) * 1.1,
                    w1 + w2
                );
            }
            else if (material.Type == ItemType.Potion && baseItem.Type == ItemType.Weapon)
            {
                double d1 = baseItem.Attributes.TryGetValue("Damage", out var dv1) ? dv1 : 1;
                double heal = material.Attributes.TryGetValue("Heal", out var hv) ? hv : 0;
                double extra = heal / 10.0;

                double w1 = baseItem.Attributes.TryGetValue("Weight", out var wv1) ? wv1 : 1;

                result = new Weapon(
                    baseItem.Name + "+Enhanced",
                    baseItem.Description,
                    d1 + extra,
                    w1
                );
            }
            else
            {
                throw new InvalidOperationException("Unsupported combination.");
            }

            RemoveItem(baseItem);
            RemoveItem(material);
            AddItem(result);

            return result;
        }

        public IItem Upgrade(IItem item, string attribute, double increment)
        {
            if (!_items.Contains(item))
                throw new InvalidOperationException("Item not in inventory.");

            var clone = item.Clone();

            if (clone is ItemBase baseItem &&
                baseItem.Attributes is IDictionary<string, double> dict)
            {
                if (dict.ContainsKey(attribute))
                    dict[attribute] = dict[attribute] + increment;
                else
                    dict[attribute] = increment;
            }

            RemoveItem(item);
            AddItem(clone);

            return clone;
        }

        public string Describe()
        {
            var lines = new List<string> { "Inventory:" };
            foreach (var it in _items)
            {
                var attrs = string.Join(", ", it.Attributes.Select(kv => $"{kv.Key}:{kv.Value}"));
                lines.Add($" - {it.Name} ({it.Type}) [{it.State.Name}] | {attrs}");
            }

            lines.Add("Equipped:");
            foreach (var it in _equipped)
                lines.Add($" - {it.Name} ({it.Type})");

            return string.Join(Environment.NewLine, lines);
        }
    }
}
