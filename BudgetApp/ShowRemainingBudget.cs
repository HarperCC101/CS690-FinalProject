// ShowRemainingBudget.cs
using System;
using System.Collections.Generic;

namespace BudgetTrackerApp {
    public static class ShowRemainingBudget {
        // Displays remaining category budget
        public static void Show(ConsoleUI ui) {
            Console.Clear();

             // Check for categories
            if (ui.NoCategories()) {
                return;
            }

            // Display last added expense
            if (!string.IsNullOrEmpty(ui.lastAddedExpense)) {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(ui.lastAddedExpense + '\n');
                Console.ResetColor();
                ui.lastAddedExpense = "";
            }

            Console.WriteLine("Budget Status\n");

            // Load categories with limits and spent amounts
            ui.dataManager.LoadCategoriesFromFile();
            Dictionary<string, (double limit, double spent)> currentCategories = ui.dataManager.GetAllCategories();

            // Display each category status
            foreach (var categoryPair in currentCategories) {
                string category = categoryPair.Key;
                double limit = categoryPair.Value.limit;
                double spent = categoryPair.Value.spent;
                double remaining = limit - spent;

                // Display color appropriate warnings
                if (spent > limit) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{category}: Remaining = {remaining:C} ");
                    ui.DisplayProgressBar(spent, limit, indent: "  ");
                    Console.WriteLine($"WARNING: {category} budget exceeded!");
                } else if (spent == limit) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{category}: Remaining = {remaining:C} ");
                    ui.DisplayProgressBar(spent, limit, indent: "  ");
                    Console.WriteLine($"WARNING: {category} budget is met!");
                } else if (spent >= limit * 0.9) {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{category}: Remaining = {remaining:C} ");
                    ui.DisplayProgressBar(spent, limit, indent: "  ");
                    Console.WriteLine($"WARNING: {category} budget is almost met!");
                } else {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"{category}: Remaining = {remaining:C} ");
                    ui.DisplayProgressBar(spent, limit, indent: "  ");
                }
                Console.ResetColor();
                Console.WriteLine();
            }

            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }
    }
}
