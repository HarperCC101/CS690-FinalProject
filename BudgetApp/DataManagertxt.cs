// DataManagertxt.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BudgetTrackerApp {
    public class DataManager {
        private static Dictionary<string, (double limit, double spent)> categories = new Dictionary<string, (double, double)>();
        private string filePath;

        public DataManager(string filePath) {
            this.filePath = filePath;
            LoadCategoriesFromFile();
        }

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

        public Dictionary<string, (double limit, double spent)> GetAllCategories() {
            return categories;
        }

        public List<string> GetCategoriesFromFile() {
            return categories.Keys.ToList();
        }

        public void UpdateDataFile() {
            List<string> lines = new List<string>();
            foreach (var cat in categories) {
                lines.Add($"{cat.Key},{cat.Value.limit},{cat.Value.spent}");
            }
            File.WriteAllLines(filePath, lines);
        }

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

        public void AddCategory(string categoryName, double limit = 0) {
            if (!categories.ContainsKey(categoryName)) {
                categories[categoryName] = (limit, 0);
                UpdateDataFile();
            }
        }
    }
}
