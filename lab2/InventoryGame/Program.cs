using System;
using InventoryGame.Inventory;

namespace InventoryGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Inventory demo (console app).");

            IItemFactory factory = new MedievalItemFactory();
            var inventory = new PlayerInventory();

            var sword = factory.CreateBasicWeapon();
            var armor = factory.CreateBasicArmor();
            var potion = factory.CreateHealingPotion();
            var quest = factory.CreateQuestItem("Amulet of Dawn");

            inventory.AddItem(sword);
            inventory.AddItem(armor);
            inventory.AddItem(potion);
            inventory.AddItem(quest);

            Console.WriteLine("Using items...");
            Console.WriteLine(inventory.UseItem(sword).Message);
            Console.WriteLine(inventory.UseItem(armor).Message);
            Console.WriteLine(inventory.UseItem(potion).Message);
            Console.WriteLine(inventory.UseItem(quest).Message);

            // Попробуем комбинировать меч и зелье
            var potion2 = factory.CreateHealingPotion();
            inventory.AddItem(potion2);
            var enhancedSword = inventory.Combine(sword, potion2);
            Console.WriteLine($"New weapon: {enhancedSword.Name}, Damage={enhancedSword.Attributes["Damage"]}");

            Console.WriteLine();
            Console.WriteLine(inventory.Describe());

            Console.WriteLine();
            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }
    }
}
