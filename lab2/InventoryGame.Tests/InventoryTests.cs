using System;
using System.Linq;
using InventoryGame.Inventory;
using Xunit;

namespace InventoryGame.Tests
{
    public class InventoryTests
    {
        [Fact]
        public void AddAndFindItem_Works()
        {
            var inv = new PlayerInventory();
            var sword = new Weapon("Sword", "Sharp blade", 12, 5);
            inv.AddItem(sword);

            Assert.Contains(sword, inv.Items);

            var found = inv.FindByName("Sword");
            Assert.Equal(sword, found);
        }

        [Fact]
        public void EquipBehavior_EquipsAndUnequips()
        {
            var inv = new PlayerInventory();
            var armor = new Armor("Plate", "Heavy armor", 20, 30);
            inv.AddItem(armor);

            var result = inv.UseItem(armor);
            Assert.True(result.Success);
            Assert.Contains(armor, inv.Equipped);
            Assert.Equal("Equipped", armor.State.Name);

            inv.UnequipItem(armor);
            Assert.DoesNotContain(armor, inv.Equipped);
            Assert.Contains(armor, inv.Items);
            Assert.Equal("InInventory", armor.State.Name);
        }

        [Fact]
        public void ConsumeBehavior_RemovesFromInventory()
        {
            var inv = new PlayerInventory();
            var potion = new Potion("Heal", "Restores HP", 50);
            inv.AddItem(potion);

            var result = inv.UseItem(potion);
            Assert.True(result.Success);
            Assert.DoesNotContain(potion, inv.Items);
            Assert.Equal("Consumed", potion.State.Name);
        }

        [Fact]
        public void CombineWeapons_CreatesStrongerWeapon()
        {
            var inv = new PlayerInventory();
            var w1 = new Weapon("Short", "S", 5, 3);
            var w2 = new Weapon("Blade", "B", 7, 4);
            inv.AddItem(w1);
            inv.AddItem(w2);

            var combined = inv.Combine(w1, w2);

            Assert.Contains(combined, inv.Items);
            Assert.DoesNotContain(w1, inv.Items);
            Assert.DoesNotContain(w2, inv.Items);

            Assert.True(combined.Attributes.TryGetValue("Damage", out var dmg));
            Assert.True(dmg > 10); // 13.2
        }

        [Fact]
        public void UpgradeItem_IncreasesAttribute()
        {
            var inv = new PlayerInventory();
            var sword = new Weapon("Sword", "S", 8, 5);
            inv.AddItem(sword);

            var upgraded = inv.Upgrade(sword, "Damage", 2.5);

            Assert.Contains(upgraded, inv.Items);
            Assert.DoesNotContain(sword, inv.Items);

            Assert.True(upgraded.Attributes.TryGetValue("Damage", out var dmg));
            Assert.Equal(10.5, dmg, 6);
        }

        [Fact]
        public void AbstractFactory_CreatesProperTypes()
        {
            IItemFactory factory = new MedievalItemFactory();

            var w = factory.CreateBasicWeapon();
            var a = factory.CreateBasicArmor();
            var p = factory.CreateHealingPotion();
            var q = factory.CreateQuestItem("Relic");

            Assert.Equal(ItemType.Weapon, w.Type);
            Assert.Equal(ItemType.Armor, a.Type);
            Assert.Equal(ItemType.Potion, p.Type);
            Assert.Equal(ItemType.QuestItem, q.Type);
        }

        [Fact]
        public void StateTransitions_WorkCorrectly()
        {
            var inv = new PlayerInventory();
            var sword = new Weapon("Sword", "S", 6, 4);
            inv.AddItem(sword);

            Assert.Equal("InInventory", sword.State.Name);

            var result = inv.UseItem(sword);
            Assert.True(result.Success);
            Assert.Equal("Equipped", sword.State.Name);

            inv.UnequipItem(sword);
            Assert.Equal("InInventory", sword.State.Name);
        }
    }
}
