// Program.cs
using System;

namespace BudgetTrackerApp {
    // Main class
    class BudgetTracker {
        // Enter application
        static void Main(string[] args) {
            // Create ConsoleUI instance (of ConsoleUI class)
            ConsoleUI ui = new ConsoleUI();
            // Start application loop
            ui.Run();
        }
    }
}
