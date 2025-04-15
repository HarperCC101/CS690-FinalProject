// AddExpense.cs
using System;
using System.Collections.Generic;

namespace BudgetTrackerApp {
    public static class AddExpense {
        // Processes new expense additions
        public static void Process(ConsoleUI ui) {
            Console.Clear();

            // Check for categories
            if (ui.NoCategories()) { return; }

            Console.WriteLine("Add a New Expense");
            Console.WriteLine("Select a category:");

            // List categories (or go back)
            List<string> categoryList = ui.dataManager.GetCategoriesFromFile();
            categoryList.Add("Back");

            int selectedIndex = 0;
            bool addingExpense = true; // expense control flag
            Console.CursorVisible = true;

            // Loop until user selects category & enters amount (or goes back)
            while (addingExpense) {
                Console.SetCursorPosition(0, 2);

                // Display categories
                for (int i = 0; i < categoryList.Count; i++) {
                    if (i == selectedIndex) {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"> {categoryList[i]}");
                    } else {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine($"  {categoryList[i]}");
                    }
                }

                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                // Arrow key navigation
                if (keyInfo.Key == ConsoleKey.UpArrow) {
                    selectedIndex = (selectedIndex == 0) ? categoryList.Count - 1 : selectedIndex - 1;
                } else if (keyInfo.Key == ConsoleKey.DownArrow) {
                    selectedIndex = (selectedIndex == categoryList.Count - 1) ? 0 : selectedIndex + 1;
                } else if (keyInfo.Key == ConsoleKey.Enter) {
                    // If category selected
                    if (selectedIndex < categoryList.Count - 1) {
                        string selectedCategory = categoryList[selectedIndex];
                        Console.Write($"Enter the amount spent on {selectedCategory}: ");

                        // Check input amount
                        if (double.TryParse(Console.ReadLine(), out double amountSpent) && amountSpent > 0) {
                            ui.dataManager.UpdateData(selectedCategory, amountSpent);
                            ui.lastAddedExpense = $"Expense Added: {amountSpent:C} to {selectedCategory}";
                            addingExpense = false;
                            ui.ShowRemainingBudget();
                        } else {
                            // Invalid amount
                            Console.WriteLine("Invalid input. Press Enter to try again.");
                            Console.ReadLine();
                            Console.Clear();
                            Console.WriteLine("Add a New Expense");
                            Console.WriteLine("Select a category:");
                            Console.SetCursorPosition(0, 2 + selectedIndex);
                        }
                    } else {
                        // If back selected
                        Console.WriteLine("Returning to the main menu...");
                        addingExpense = false;
                    }
                }
            }
            Console.CursorVisible = false;
        }
    }
}
