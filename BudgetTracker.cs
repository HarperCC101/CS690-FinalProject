using System;
using System.Collections.Generic;
using System.Data.SQLite;

class BudgetTracker {
    static Dictionary<string, (double limit, double spent)> categories = new Dictionary<string, (double, double)>();
    static string connectionString = "Data Source=Budget.db;Version=3;";

    static void Main()     {
        Console.WriteLine("Welcome to Aria's Budget Tracker!");
        LoadCategoriesFromDatabase();

        bool isRunning = true; // Flag to control the main loop

        while (isRunning)         {
            ShowMenu(ref isRunning);
        }
    }

    static void LoadCategoriesFromDatabase()     {
        using (var connection = new SQLiteConnection(connectionString)) {
            connection.Open();
            string query = "SELECT CategoryName, LimitAmount, SpentAmount FROM Categories";

            using (var command = new SQLiteCommand(query, connection))
            using (var reader = command.ExecuteReader())             {
                categories.Clear();
                while (reader.Read())
                {
                    string categoryName = reader.GetString(0);
                    double limitAmount = reader.GetDouble(1);
                    double spentAmount = reader.GetDouble(2);
                    categories[categoryName] = (limitAmount, spentAmount);
                }
            }
        }
    }

    static void ShowMenu(ref bool isRunning) {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("Please choose an option:");

        string[] menuOptions = { "Add Expense", "View Remaining Budget", "Set Spending Limits", "Exit" };
        int selectedIndex = 0;

        bool menuRunning = true;

        // Set cursor color to blue as well
        Console.CursorVisible = true; // Ensure cursor is visible

        // Loop for displaying and navigating through the menu
        while (menuRunning) {
            // Display the header only once at the beginning of the loop
            Console.SetCursorPosition(0, 0); // Move cursor back to the top to redraw the header
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Please choose an option:");

            // Loop through menu options and display them
            for (int i = 0; i < menuOptions.Length; i++) {
                // Set the color for the selected option
                if (i == selectedIndex) {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"> {menuOptions[i]}");
                } else {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"  {menuOptions[i]}");
                }
            }

            // Read user input for navigating menu
            ConsoleKeyInfo key = Console.ReadKey(true);

            // Wrap-around logic for the cursor
            if (key.Key == ConsoleKey.UpArrow) {
                // At the top? Wrap to the bottom
                selectedIndex = (selectedIndex == 0) ? menuOptions.Length - 1 : selectedIndex - 1;
            } else if (key.Key == ConsoleKey.DownArrow) {
                // At the bottom? Wrap to the top
                selectedIndex = (selectedIndex == menuOptions.Length - 1) ? 0 : selectedIndex + 1;
            } else if (key.Key == ConsoleKey.Enter) {
                menuRunning = false; // Exit the loop when Enter is pressed
                HandleMenuSelection(selectedIndex, ref isRunning); // Handle the menu selection
            }
        }
    }

    static void HandleMenuSelection(int choice, ref bool isRunning) {
        switch (choice) {
            case 0: AddExpense(); break;
            case 1: ShowRemainingBudget(); break;
            case 2: SetSpendingLimits(); break;
            case 3: Console.WriteLine("Goodbye!"); Environment.Exit(0); break;
            default: Console.WriteLine("Invalid option!"); Console.ReadKey(); break;
        }
    }

    static void AddExpense() {
        Console.Clear();
        Console.WriteLine("Add a New Expense");
        Console.WriteLine("Select a category:");

        // Retrieve categories from the database
        List<string> categoryList = GetCategoriesFromDatabase();
        if (categoryList.Count == 0) {
            Console.WriteLine("No categories found in the database.");
            Console.ReadKey();
            return;
        }

        // Add "Back" option to the list
        categoryList.Add("Back");

        int selectedIndex = 0;
        bool addingExpense = true;
        Console.CursorVisible = true;

        while (addingExpense) {
            // Move the cursor back to the line after "Select a category:"
            Console.SetCursorPosition(0, 2);

            // Display each category with cyan for the selected one
            for (int i = 0; i < categoryList.Count; i++) {
                if (i == selectedIndex) {
                    Console.ForegroundColor = ConsoleColor.Cyan; // Highlight the selected option
                    Console.WriteLine($"> {categoryList[i]}");
                } else {
                    Console.ForegroundColor = ConsoleColor.White; // Default color for other options
                    Console.WriteLine($"  {categoryList[i]}");
                }
            }

            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            if (keyInfo.Key == ConsoleKey.UpArrow) {
                // Wrap around logic for cycling
                selectedIndex = (selectedIndex == 0) ? categoryList.Count - 1 : selectedIndex - 1;
            } else if (keyInfo.Key == ConsoleKey.DownArrow) {
                // Wrap around logic for cycling
                selectedIndex = (selectedIndex == categoryList.Count - 1) ? 0 : selectedIndex + 1;
            } else if (keyInfo.Key == ConsoleKey.Enter) {
                if (selectedIndex < categoryList.Count - 1) {
                    string selectedCategory = categoryList[selectedIndex];
                    Console.Write($"Enter the amount spent on {selectedCategory}: ");

                    if (double.TryParse(Console.ReadLine(), out double amountSpent) && amountSpent > 0) {
                        UpdateDatabase(selectedCategory, amountSpent);
                        addingExpense = false; // Exit the loop
                    } else {
                        Console.WriteLine("Invalid input. Press Enter to try again.");
                        Console.ReadLine();
                        Console.Clear();
                        Console.WriteLine("Add a New Expense");
                        Console.WriteLine("Select a category:");
                        Console.SetCursorPosition(0, 2 + selectedIndex); // Keep selection
                    }
                } else { // "Back" is selected
                    Console.WriteLine("Returning to the main menu...");
                    addingExpense = false; // Exit the loop
                }
            }
        }
    }

    static List<string> GetCategoriesFromDatabase() {
        List<string> categories = new List<string>();
        using (var conn = new SQLiteConnection("Data Source=Budget.db;Version=3;")) {
            conn.Open();
            string query = "SELECT CategoryName FROM Categories";
            using (var cmd = new SQLiteCommand(query, conn))
            using (var reader = cmd.ExecuteReader()) {
                while (reader.Read())
                    categories.Add(reader.GetString(0));
            }
        }
        return categories;
    }

    static void UpdateDatabase(string category, double amountSpent) {
        using (var conn = new SQLiteConnection("Data Source=Budget.db;Version=3;")) {
            conn.Open();

            // Update the spent amount
            string query = @"
                UPDATE Categories 
                SET SpentAmount = SpentAmount + @amount 
                WHERE CategoryName = @category";
            
            using (var cmd = new SQLiteCommand(query, conn)) {
                cmd.Parameters.AddWithValue("@amount", amountSpent);
                cmd.Parameters.AddWithValue("@category", category);
                cmd.ExecuteNonQuery();
            }

            Console.WriteLine($"Expense of {amountSpent:C} added to {category}.");
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }
    }

    static void UpdateSpentAmount(string category, double newSpent) {
        using (var connection = new SQLiteConnection(connectionString)) {
            connection.Open();
            string query = "UPDATE Categories SET SpentAmount = @spent WHERE CategoryName = @name";

            using (var command = new SQLiteCommand(query, connection)) {
                command.Parameters.AddWithValue("@spent", newSpent);
                command.Parameters.AddWithValue("@name", category);
                command.ExecuteNonQuery();
            }
        }
    }

    static void ShowRemainingBudget() {
        Console.Clear();
        Console.WriteLine("Budget Status\n");

        using (var conn = new SQLiteConnection("Data Source=Budget.db;Version=3;")) {
            conn.Open();
            string query = "SELECT CategoryName, LimitAmount, SpentAmount FROM Categories";

            using (var cmd = new SQLiteCommand(query, conn))
            using (var reader = cmd.ExecuteReader()) {
                while (reader.Read()) {
                    string category = reader.GetString(0);
                    double limit = reader.GetDouble(1);
                    double spent = reader.GetDouble(2);
                    double remaining = limit - spent;

                    // Set category color based on remaining budget
                    if (spent > limit) {
                        Console.ForegroundColor = ConsoleColor.Red;  // Red if exceeded
                        Console.WriteLine($"{category}: Remaining = {remaining:C} ");
                        DisplayProgressBar(spent, limit, indent: "  ");
                        Console.WriteLine($"WARNING: {category} budget exceeded!");
                    } else if (spent == limit) {
                        Console.ForegroundColor = ConsoleColor.Red;  // Red if met
                        Console.WriteLine($"{category}: Remaining = {remaining:C} ");
                        DisplayProgressBar(spent, limit, indent: "  ");
                        Console.WriteLine($"WARNING: {category} budget is met!");
                    } else if (spent >= limit * 0.9) {
                        Console.ForegroundColor = ConsoleColor.Yellow;  // Yellow if close to limit
                        Console.WriteLine($"{category}: Remaining = {remaining:C} ");
                        DisplayProgressBar(spent, limit, indent: "  ");
                        Console.WriteLine($"WARNING: {category} budget is almost met!");
                    } else {
                        Console.ForegroundColor = ConsoleColor.White;  // Default color
                        Console.WriteLine($"{category}: Remaining = {remaining:C} ");
                        DisplayProgressBar(spent, limit, indent: "  ");
                    }

                    Console.ResetColor();
                }
            }
        }

        Console.WriteLine("\nPress Enter to continue...");
        Console.ReadLine();
    }

    static void SetSpendingLimits() {
        Console.Clear();
        Console.WriteLine("Set Spending Limits for Categories\n");

        Dictionary<string, (double limit, double spent)> currentCategoryData = new Dictionary<string, (double limit, double spent)>();

        using (var conn = new SQLiteConnection("Data Source=Budget.db;Version=3;")) {
            conn.Open();
            string query = "SELECT CategoryName, LimitAmount, SpentAmount FROM Categories";

            using (var cmd = new SQLiteCommand(query, conn))
            using (var reader = cmd.ExecuteReader()) {
                while (reader.Read()) {
                    string category = reader.GetString(0);
                    double limit = reader.GetDouble(1);
                    double spent = reader.GetDouble(2);
                    currentCategoryData[category] = (limit, spent);
                }
            }
        }

        List<string> categoryList = new List<string>(currentCategoryData.Keys);
        int selectedIndex = 0;
        bool settingLimits = true;
        Console.CursorVisible = true;

        while (settingLimits) {
            Console.SetCursorPosition(0, 2); // Start drawing from this position in each iteration

            // Display categories and the "Back" option
            for (int i = 0; i <= categoryList.Count; i++) { // Iterate one more time for "Back"
                if (i < categoryList.Count) { // Display a category
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
                    Console.WriteLine(); // Add an empty line for spacing
                } else { // i == categoryList.Count, display "Back"
                    Console.ForegroundColor = (selectedIndex == categoryList.Count) ? ConsoleColor.Cyan : ConsoleColor.White;
                    Console.WriteLine($"{(selectedIndex == categoryList.Count ? "> " : "  ")}Back");
                    Console.ResetColor();
                }
            }

            Console.ResetColor();

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
                        currentCategoryData[selectedCategory] = (newLimit, currentCategoryData[selectedCategory].spent);
                        UpdateLimitAmount(selectedCategory, newLimit);
                        Console.WriteLine("Spending limits updated.");

                        // Clear the console before redrawing
                        Console.Clear();
                        Console.WriteLine("Set Spending Limits for Categories\n");

                        // Immediately redraw the menu
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
                        Console.ResetColor();
                    } else {
                        Console.WriteLine("Invalid input! Please enter a number.");
                    }
                    // Console.WriteLine("Press Enter to continue...");
                    // Console.ReadLine();
                } else { // selectedIndex == categoryList.Count ("Back")
                    settingLimits = false; // Exit the loop
                }
            }
        }
    }

    /**
    * @brief Displays a text-based progress bar in the console.
    *
    * This function calculates and prints a progress bar to the console,
    * indicating the progress of a value (`spent`) towards a limit (`limit`).
    * The progress bar is visually represented using square brackets, equals signs,
    * and spaces, along with a percentage.
    *
    * @param spent The current amount spent (or the current progress value).
    * @param limit The total limit or target value.
    * @param indent An optional string to prepend to the progress bar for indentation
    * (defaults to "  ").
    */
    static void DisplayProgressBar(double spent, double limit, string indent = "  ") {
        int progressBarWidth = 20;
        double progress = (limit > 0) ? Math.Min(1, spent / limit) : 0;
        int filledBars = (int)(progress * progressBarWidth);
        Console.WriteLine($"{indent}[{new string('=', filledBars)}{new string(' ', progressBarWidth - filledBars)}] {progress * 100:F0}%");
    }

    /**
    * @brief Updates the spending limit for a specific category in the database.
    *
    * This function takes a category name and a new spending limit as input
    * and updates the corresponding record in the 'Categories' table of the
    * SQLite database.
    *
    * @param category The name of the category to update.
    * @param newLimit The new spending limit to set for the category.
    */
    static void UpdateLimitAmount(string category, double newLimit) {
        using (var connection = new SQLiteConnection(connectionString)) {
            connection.Open();
            string query = "UPDATE Categories SET LimitAmount = @limit WHERE CategoryName = @name";

            using (var command = new SQLiteCommand(query, connection)) {
                command.Parameters.AddWithValue("@limit", newLimit);
                command.Parameters.AddWithValue("@name", category);
                command.ExecuteNonQuery();
            }
        }
    }
}
