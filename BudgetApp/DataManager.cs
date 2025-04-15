// DataManager.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BudgetTrackerApp {
    public class DataManager {
        // Dictionary to store category data (limit & spent amounts)
        private static Dictionary<string, (double limit, double spent)> categories = new Dictionary<string, (double, double)>();
        // Path to Budget.txt
        private string filePath;
        // Always the same start Budget.txt contents
        private const string StartFileContent = "Groceries,200,0\nUtilities,200,0\nEntertainment,200,0\nTransportation,200,0\nMiscellaneous,200,0\n";

        // Constructor: Initializes DataManager with file path
        public DataManager(string filePath) {
            this.filePath = filePath;
            // Don't initialize test file
            if (filePath != "TestBudgetDM.txt") {
                InitializeFile();
            }           
            LoadCategoriesFromFile();
        }

        // Initialize Budget.txt with start contents
        private void InitializeFile() {
            try {
                // Create Budget.txt if it doesn't exist
                if (!File.Exists(filePath)) {
                    File.WriteAllText(filePath, StartFileContent);
                } else {
                    // Re-write Budget.txt if file doesn't match start contents
                    string fileContent = File.ReadAllText(filePath);
                    if (fileContent != StartFileContent) {
                        File.WriteAllText(filePath, StartFileContent);
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine($"Error initializing file: {ex.Message}");
            }

        }

        // Populate category data from file into Dictionary
        public void LoadCategoriesFromFile() {
            if (File.Exists(filePath)) {
                categories.Clear();
                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines) {
                    string[] parts = line.Split(',');
                    if (parts.Length == 3) {
                        string categoryName = parts[0];
                        if (double.TryParse(parts[1], out double limitAmount) && double.TryParse(parts[2], out double spentAmount)) {
                            categories[categoryName] = (limitAmount, spentAmount);
                        }
                    }
                }
            }
        }

        // Populated category Dictionary
        public Dictionary<string, (double limit, double spent)> GetAllCategories() {
            return categories;
        }

        // Returns name list from category Dictionary
        public List<string> GetCategoriesFromFile() {
            return categories.Keys.ToList();
        }

        // Update Budget.txt with category data
        public void UpdateDataFile() {
            List<string> lines = new List<string>();
            foreach (var cat in categories) {
                lines.Add($"{cat.Key},{cat.Value.limit},{cat.Value.spent}");
            }
            File.WriteAllLines(filePath, lines);
        }

        // Update category spent amount (save to Budget.txt)
        public void UpdateData(string categoryName, double amountSpent) {
            if (amountSpent <= 0) {
                Console.WriteLine("Invalid amount. Expense amount must be positive.");
                return;
            }

            if (categories.ContainsKey(categoryName)) {
                (double limit, double spent) = categories[categoryName];
                categories[categoryName] = (limit, spent + amountSpent);
                UpdateDataFile();
            }
        }

        // Set category spending limit (save to Budget.txt)
        public void SetSpendingLimit(string categoryName, double newLimit) {
            if (newLimit <= 0) {
                Console.WriteLine("Invalid limit. Spending limit must be a positive value.");
                return;
            }

            if (categories.ContainsKey(categoryName)) {
                (double limit, double spent) = categories[categoryName];
                categories[categoryName] = (newLimit, spent);
                UpdateDataFile();
            }
        }

        /*
        // Future implementation
        public void AddCategory(string categoryName, double limit = 0) {
            if (!categories.ContainsKey(categoryName)) {
                categories[categoryName] = (limit, 0);
                UpdateDataFile();
            }
        }
        */
    }
}
