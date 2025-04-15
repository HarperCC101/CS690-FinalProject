// SetSpendingLimits.cs
using System;
using System.Collections.Generic;

namespace BudgetTrackerApp {
    public static class SetSpendingLimits {
        // Process setting spending limits
        public static void Process(ConsoleUI ui) {
            Console.Clear();

             // Check for categories
            if (ui.NoCategories()) {
                return;
            }

            Console.WriteLine("Set Spending Limits for Categories\n");

            // List category names and fetch data
            Dictionary<string, (double limit, double spent)> currentCategoryData = ui.dataManager.GetAllCategories();
            List<string> categoryList = new List<string>(currentCategoryData.Keys);
            int selectedIndex = 0;
            bool settingLimits = true; // Limit control flag
            Console.CursorVisible = true;

            // Loop until user selects category & sets limit (or goes back)
            while (settingLimits) {
                Console.Clear();
                Console.WriteLine("Set Spending Limits for Categories\n");
                Console.SetCursorPosition(0, 2);

                // Display categories and limits with spent amount
                for (int i = 0; i <= categoryList.Count; i++) {
                    if (i < categoryList.Count) {
                        string categoryName = categoryList[i];
                        (double limit, double spent) = currentCategoryData[categoryName];

                        if (i == selectedIndex) {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine($"> {categoryName}:");
                            Console.WriteLine($"  Current Limit: {limit:C}");
                            Console.WriteLine($"  Amount Spent: {spent:C}");
                            ui.DisplayProgressBar(spent, limit, indent: "  ");
                        } else {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine($"  {categoryName}:");
                            Console.WriteLine($"  Current Limit: {limit:C}");
                            Console.WriteLine($"  Amount Spent: {spent:C}");
                            ui.DisplayProgressBar(spent, limit, indent: "  ");
                        }
                        Console.WriteLine();
                    } else {
                        // Back logic
                        Console.ForegroundColor = (selectedIndex == categoryList.Count) ? ConsoleColor.Cyan : ConsoleColor.White;
                        Console.WriteLine($"{(selectedIndex == categoryList.Count ? "> " : "  ")}Back");
                        Console.ResetColor();
                    }
                }

                ConsoleKeyInfo key = Console.ReadKey(true);

                // Arrow key navigation
                if (key.Key == ConsoleKey.UpArrow) {
                    selectedIndex = (selectedIndex == 0) ? categoryList.Count : selectedIndex - 1;
                } else if (key.Key == ConsoleKey.DownArrow) {
                    selectedIndex = (selectedIndex == categoryList.Count) ? 0 : selectedIndex + 1;
                } else if (key.Key == ConsoleKey.Enter) {
                    // If category selected
                    if (selectedIndex < categoryList.Count) {
                        string selectedCategory = categoryList[selectedIndex];

                        // Check input amount
                        Console.Write($"Enter new limit for {selectedCategory}: ");
                        if (double.TryParse(Console.ReadLine(), out double newLimit)) {
                            if (newLimit > 0) {
                                ui.dataManager.SetSpendingLimit(selectedCategory, newLimit);
                                Console.WriteLine("Spending limits updated.");

                                // Real-time data reload (after update)
                                ui.dataManager.LoadCategoriesFromFile();
                                currentCategoryData = ui.dataManager.GetAllCategories();
                                categoryList = new List<string>(currentCategoryData.Keys);
                                selectedIndex = 0;
                            } else {
                                // Invalid amount
                                Console.WriteLine("Invalid input! Please enter a number.");
                                Console.WriteLine("Press Enter to continue...");
                                Console.ReadLine();
                            }
                        }
                    } else {
                        // If back selected
                        settingLimits = false;
                    }
                }
            }
            Console.CursorVisible = false;
        }
    }
}
