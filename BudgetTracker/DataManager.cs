using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace BudgetTrackerApp {
    public class DataManager {
        private static Dictionary<string, (double limit, double spent)> categories = 
                   new Dictionary<string, (double, double)>();
        // private static string connectionString = "Data Source=Budget.db;Version=3;";
        private string connectionString;

        public DataManager(string dbConnectionString) {
            connectionString = dbConnectionString;
        }

        public void LoadCategoriesFromDatabase() {
            using (var connection = new SQLiteConnection(connectionString)) {
                connection.Open();
                string query = "SELECT CategoryName, LimitAmount, SpentAmount FROM Categories";

                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader()) {
                    categories.Clear();
                    while (reader.Read()) {
                        string categoryName = reader.GetString(0);
                        double limitAmount = reader.GetDouble(1);
                        double spentAmount = reader.GetDouble(2);
                        categories[categoryName] = (limitAmount, spentAmount);
                    }
                }
            }
        }

        public Dictionary<string, (double limit, double spent)> GetAllCategories() {
            return categories;
            // return new Dictionary<string, (double limit, double spent)>(categories);
        }

        public List<string> GetCategoriesFromDatabase() {
            List<string> categoryList = new List<string>();
            using (var conn = new SQLiteConnection(connectionString)) {
                conn.Open();
                string query = "SELECT CategoryName FROM Categories";
                using (var cmd = new SQLiteCommand(query, conn))
                using (var reader = cmd.ExecuteReader()) {
                    while (reader.Read())
                        categoryList.Add(reader.GetString(0));
                }
            }
            return categoryList;
        }

        public void UpdateDatabase(string categoryName, double amountSpent) {
            if (amountSpent <= 0) {
                Console.WriteLine("Invalid amount. Expense amount must be positive.");
                return; // Exit without updating the database
            }

            using (var conn = new SQLiteConnection(connectionString)) {
                try {
                    conn.Open();

                    string query = @"
                        UPDATE Categories
                        SET SpentAmount = SpentAmount + @amount
                        WHERE CategoryName = @category";

                    using (var cmd = new SQLiteCommand(query, conn)) {
                        cmd.Parameters.AddWithValue("@amount", amountSpent);
                        cmd.Parameters.AddWithValue("@category", categoryName);
                    }
                } catch (SQLiteException ex) {
                    Console.WriteLine($"Database Error: {ex.Message}");
                }
            }
        }       

        public void SetSpendingLimit(string categoryName, double newLimit) {
            if (newLimit <= 0) {
                Console.WriteLine("Invalid limit. Spending limit must be a positive value.");
                return; // Exit without updating the database
            }

            using (var connection = new SQLiteConnection(connectionString)) {
                connection.Open();
                string query = "UPDATE Categories SET LimitAmount = @limit WHERE CategoryName = @name";

                using (var command = new SQLiteCommand(query, connection)) {
                    command.Parameters.AddWithValue("@limit", newLimit);
                    command.Parameters.AddWithValue("@name", categoryName);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void AddCategory(string categoryName, double limit = 0) {
            using (var connection = new SQLiteConnection(connectionString)) {
                connection.Open();
                string insertSQL = "INSERT INTO Categories (CategoryName, LimitAmount, SpentAmount) VALUES (@name, @limit, 0)";
                using (var command = new SQLiteCommand(insertSQL, connection)) {
                    command.Parameters.AddWithValue("@name", categoryName);
                    command.Parameters.AddWithValue("@limit", limit);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
