// MainMenu.cs
using System;

namespace BudgetTrackerApp {
    public static class MainMenu {
        // 'ui' = ConsoleUI instance, 'isRunning' = loop control flag
        public static void Show(ConsoleUI ui, ref bool isRunning) {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Please choose an option:");

            string[] menuOptions = { "Add Expense", "View Remaining Budget", "Set Spending Limits", "Exit" };
            int selectedIndex = 0;

            bool menuRunning = true; // Menu control flag
            Console.CursorVisible = true;

            // Loop until user makes selection
            while (menuRunning) {
                Console.SetCursorPosition(0, 0); // Reset cursor to top of menu
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Please choose an option:");

                // Diplay menu options
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

                // Arrow key navigation
                if (key.Key == ConsoleKey.UpArrow) {
                    selectedIndex = (selectedIndex == 0) ? menuOptions.Length - 1 : selectedIndex - 1;
                } else if (key.Key == ConsoleKey.DownArrow) {
                    selectedIndex = (selectedIndex == menuOptions.Length - 1) ? 0 : selectedIndex + 1;
                } else if (key.Key == ConsoleKey.Enter) {
                    menuRunning = false;
                    ui.HandleMenuSelection(selectedIndex, ref isRunning); // Call selected method
                }
            }
            Console.CursorVisible = false;
        }
    }
}
