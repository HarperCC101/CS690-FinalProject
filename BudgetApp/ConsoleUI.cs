// ConsoleUI.cs
using System;
using System.Collections.Generic;

namespace BudgetTrackerApp {
    public class ConsoleUI {
        public DataManager dataManager; // Handles budget data
        private const string DefaultFilePath = "Budget.txt";
        public string lastAddedExpense = "";

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
                MainMenu.Show(this, ref isRunning);
            }
        }

        // Handles the menu selection and method calls
        public void HandleMenuSelection(int choice, ref bool isRunning) {
            try {
                switch (choice) {
                    case 0: AddExpense(); break;
                    case 1: ShowRemainingBudget(); break;
                    case 2: SetSpendingLimits(); break;
                    case 3: Console.WriteLine("Goodbye!"); Environment.Exit(0); break;
                    default: Console.WriteLine("Invalid option!"); Console.ReadKey(); break;
                }
            } catch (Exception ex) {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        // Check for categories in Budget.txt
        public bool NoCategories() {
            List<string> categoryList = dataManager.GetCategoriesFromFile();
            if (categoryList.Count == 0) {
                Console.WriteLine("No categories found.");
                Console.ReadKey();
                return true; // No categories found
            }
            return false; // Exit check (categories found)
        }

        // Call AddExpense class (to add expenses)
        public void AddExpense() {
            BudgetTrackerApp.AddExpense.Process(this);
        }

        // Call ShowRemainingBudget class (to show remaining budget)
        public void ShowRemainingBudget() {
            BudgetTrackerApp.ShowRemainingBudget.Show(this);
        }

        // Call SetSpendingLimits class (to set spending limits)
        public void SetSpendingLimits() {
            BudgetTrackerApp.SetSpendingLimits.Process(this);
        }

        // Call DisplayProgressBar class (to display progress bar)
        public void DisplayProgressBar(double spent, double limit, string indent = "  ") {
            BudgetTrackerApp.DisplayProgressBar.Show(spent, limit, indent);
        }
    }
}
