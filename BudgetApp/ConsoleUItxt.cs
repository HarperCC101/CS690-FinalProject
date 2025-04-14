// ConsoleUItxt.cs
using System;
using System.Collections.Generic;

namespace BudgetTrackerApp {
    public class ConsoleUI {
        private DataManager dataManager; // Handles budget data
        private const string DefaultFilePath = "Budget.txt";
        private string lastAddedExpense = "";

        // Constructor: Initializes the ConsoleUI with DataManager instance
        public ConsoleUI() {
            dataManager = new DataManager(DefaultFilePath);
        }

        // Runs application loop
        public void Run() {
            Console.Clear();
            Console.WriteLine("Welcome to Aria's Budget Tracker!");
            System.Threading.Thread.Sleep(2500);
            dataManager.LoadCategoriesFromFile();

            bool isRunning = true;

            // Display main menu and handle user input
            while (isRunning) {
                ShowMenu(ref isRunning);
            }
        }

        private void ShowMenu(ref bool isRunning) {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Please choose an option:");

            string[] menuOptions = { "Add Expense", "View Remaining Budget", "Set Spending Limits", "Exit" };
            int selectedIndex = 0;

            bool menuRunning = true; // Menu flag
            Console.CursorVisible = true;

            while (menuRunning) {
                Console.SetCursorPosition(0, 0); // Reset cursor to top of menu
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Please choose an option:");

                // Diplay menu
                for (int i = 0; i < menuOptions.Length; i++) {
                    if (i == selectedIndex) {
                        Console.ForegroundColor = ConsoleColor.Cyan; // Highlight selected option
                        Console.WriteLine($"> {menuOptions[i]}");
                    } else {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine($"  {menuOptions[i]}");
                    }
                }

                ConsoleKeyInfo key = Console.ReadKey(true); // Read user input

                // Standard arrow key navigation
                if (key.Key == ConsoleKey.UpArrow) {
                    selectedIndex = (selectedIndex == 0) ? menuOptions.Length - 1 : selectedIndex - 1;
                } else if (key.Key == ConsoleKey.DownArrow) {
                    selectedIndex = (selectedIndex == menuOptions.Length - 1) ? 0 : selectedIndex + 1;
                } else if (key.Key == ConsoleKey.Enter) {
                    menuRunning = false;
                    HandleMenuSelection(selectedIndex, ref isRunning);
                }
            }
            Console.CursorVisible = false;
        }

        private void HandleMenuSelection(int choice, ref bool isRunning) {
            switch (choice) {
                case 0: AddExpense(); break;
                case 1: ShowRemainingBudget(); break;
                case 2: SetSpendingLimits(); break;
                case 3: Console.WriteLine("Goodbye!"); Environment.Exit(0); break;
                default: Console.WriteLine("Invalid option!"); Console.ReadKey(); break;
            }
        }

        private bool NoCategories() {
            List<string> categoryList = dataManager.GetCategoriesFromFile();
            if (categoryList.Count == 0) {
                Console.WriteLine("No categories found.");
                Console.ReadKey();
                return true; // Indicate that no categories were found.
            }
            return false; // Indicate that categories were found.
        }

        public void AddExpense() {
            Console.Clear();

            if (NoCategories()) { return; }

            Console.WriteLine("Add a New Expense");
            Console.WriteLine("Select a category:");

            List<string> categoryList = dataManager.GetCategoriesFromFile();
            categoryList.Add("Back");

            int selectedIndex = 0;
            bool addingExpense = true;
            Console.CursorVisible = true;

            while (addingExpense) {
                Console.SetCursorPosition(0, 2);

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

                if (keyInfo.Key == ConsoleKey.UpArrow) {
                    selectedIndex = (selectedIndex == 0) ? categoryList.Count - 1 : selectedIndex - 1;
                } else if (keyInfo.Key == ConsoleKey.DownArrow) {
                    selectedIndex = (selectedIndex == categoryList.Count - 1) ? 0 : selectedIndex + 1;
                } else if (keyInfo.Key == ConsoleKey.Enter) {
                    if (selectedIndex < categoryList.Count - 1) {
                        string selectedCategory = categoryList[selectedIndex];
                        Console.Write($"Enter the amount spent on {selectedCategory}: ");

                        if (double.TryParse(Console.ReadLine(), out double amountSpent) && amountSpent > 0) {
                            dataManager.UpdateData(selectedCategory, amountSpent);
                            lastAddedExpense = $"Expense Added: {amountSpent:C} to {selectedCategory}";
                            addingExpense = false;
                            ShowRemainingBudget();
                        } else {
                            Console.WriteLine("Invalid input. Press Enter to try again.");
                            Console.ReadLine();
                            Console.Clear();
                            Console.WriteLine("Add a New Expense");
                            Console.WriteLine("Select a category:");
                            Console.SetCursorPosition(0, 2 + selectedIndex);
                        }
                    } else {
                        Console.WriteLine("Returning to the main menu...");
                        addingExpense = false;
                    }
                }
            }
            Console.CursorVisible = false;
        }

        private void ShowRemainingBudget() {
            Console.Clear();

            if (NoCategories()) { // Check for categories
                return; // Exit if no categories are found
            }

            if (!string.IsNullOrEmpty(lastAddedExpense)) {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(lastAddedExpense + '\n');
                Console.ResetColor();
                lastAddedExpense = "";
            }

            Console.WriteLine("Budget Status\n");

            dataManager.LoadCategoriesFromFile();
            Dictionary<string, (double limit, double spent)> currentCategories = dataManager.GetAllCategories();

            foreach (var categoryPair in currentCategories) {
                string category = categoryPair.Key;
                double limit = categoryPair.Value.limit;
                double spent = categoryPair.Value.spent;
                double remaining = limit - spent;

                if (spent > limit) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{category}: Remaining = {remaining:C} ");
                    DisplayProgressBar(spent, limit, indent: "  ");
                    Console.WriteLine($"WARNING: {category} budget exceeded!");
                } else if (spent == limit) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{category}: Remaining = {remaining:C} ");
                    DisplayProgressBar(spent, limit, indent: "  ");
                    Console.WriteLine($"WARNING: {category} budget is met!");
                } else if (spent >= limit * 0.9) {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{category}: Remaining = {remaining:C} ");
                    DisplayProgressBar(spent, limit, indent: "  ");
                    Console.WriteLine($"WARNING: {category} budget is almost met!");
                } else {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"{category}: Remaining = {remaining:C} ");
                    DisplayProgressBar(spent, limit, indent: "  ");
                }
                Console.ResetColor();
                Console.WriteLine();
            }

            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }

        public void SetSpendingLimits() {
            Console.Clear();

            if (NoCategories()) { // Check for categories
                return; // Exit if no categories are found
            }

            Console.WriteLine("Set Spending Limits for Categories\n");

            Dictionary<string, (double limit, double spent)> currentCategoryData = dataManager.GetAllCategories();
            List<string> categoryList = new List<string>(currentCategoryData.Keys);
            int selectedIndex = 0;
            bool settingLimits = true;
            Console.CursorVisible = true;

            while (settingLimits) {
                Console.Clear();
                Console.WriteLine("Set Spending Limits for Categories\n");
                Console.SetCursorPosition(0, 2);

                for (int i = 0; i <= categoryList.Count; i++) {
                    if (i < categoryList.Count) {
                        string categoryName = categoryList[i];
                        (double limit, double spent) = currentCategoryData[categoryName];

                        if (i == selectedIndex) {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine($"> {categoryName}:");
                            Console.WriteLine($"  Current Limit: {limit:C}");
                            Console.WriteLine($"  Amount Spent: {spent:C}");
                            DisplayProgressBar(spent, limit, indent: "  ");
                        } else {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine($"  {categoryName}:");
                            Console.WriteLine($"  Current Limit: {limit:C}");
                            Console.WriteLine($"  Amount Spent: {spent:C}");
                            DisplayProgressBar(spent, limit, indent: "  ");
                        }
                        Console.WriteLine();
                    } else {
                        Console.ForegroundColor = (selectedIndex == categoryList.Count) ? ConsoleColor.Cyan : ConsoleColor.White;
                        Console.WriteLine($"{(selectedIndex == categoryList.Count ? "> " : "  ")}Back");
                        Console.ResetColor();
                    }
                }

                ConsoleKeyInfo key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.UpArrow) {
                    selectedIndex = (selectedIndex == 0) ? categoryList.Count : selectedIndex - 1;
                } else if (key.Key == ConsoleKey.DownArrow) {
                    selectedIndex = (selectedIndex == categoryList.Count) ? 0 : selectedIndex + 1;
                } else if (key.Key == ConsoleKey.Enter) {
                    if (selectedIndex < categoryList.Count) {
                        string selectedCategory = categoryList[selectedIndex];
                        Console.Write($"Enter new limit for {selectedCategory}: ");
                        if (double.TryParse(Console.ReadLine(), out double newLimit)) {
                            if (newLimit > 0) {
                                dataManager.SetSpendingLimit(selectedCategory, newLimit);
                                Console.WriteLine("Spending limits updated.");

                                dataManager.LoadCategoriesFromFile();
                                currentCategoryData = dataManager.GetAllCategories();
                                categoryList = new List<string>(currentCategoryData.Keys);
                                selectedIndex = 0;
                            } else {
                                Console.WriteLine("Invalid input! Please enter a number.");
                                Console.WriteLine("Press Enter to continue...");
                                Console.ReadLine();
                            }
                        }
                    } else {
                        settingLimits = false;
                    }
                }
            }
            Console.CursorVisible = false;
        }

        private static void DisplayProgressBar(double spent, double limit, string indent = "  ") {
            if (limit <= 0) {
                if (spent > 0) {
                    Console.WriteLine($"{indent}[{new string('=', 20)}] ∞%");
                } else {
                    Console.WriteLine($"{indent}[{new string(' ', 20)}] 0%");
                }
                return;
            }

            if (spent > limit) {
                Console.WriteLine($"{indent}[{new string('=', 20)}] ∞%");
                return;
            }

            double percentage = Math.Max(0, Math.Min(1, spent / limit));
            int filledLength = (int)(percentage * 20);
            int emptyLength = 20 - filledLength;
            Console.WriteLine($"{indent}[{new string('=', filledLength)}{new string(' ', emptyLength)}] {percentage:P0}");
        }
    }
}
