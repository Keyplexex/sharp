using System;

namespace FoodDeliverySystem
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "--test")
            {
                RunTests();
                return;
            }
            
            Console.WriteLine("=== Food Delivery System ===");
        }
        
        static void RunTests()
        {
            Console.WriteLine("=== Running Unit Tests ===");
            
            Console.WriteLine("To run xUnit tests, use: dotnet test");
        }
    }
}