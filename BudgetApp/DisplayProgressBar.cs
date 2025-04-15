// DisplayProgressBar.cs
using System;

namespace BudgetTrackerApp {
    public static class DisplayProgressBar {
        public static void Show(double spent, double limit, string indent = "  ") {
            // If limit is zero or negative
            if (limit <= 0) {
                if (spent > 0) {
                    // If spent is positive and limit is zero, full bar & infinite percent
                    Console.WriteLine($"{indent}[{new string('=', 20)}] ∞%");
                } else {
                    // If spend and limit are zero, empty bar & 0 zero percent
                    Console.WriteLine($"{indent}[{new string(' ', 20)}] 0%");
                }
                return;
            }

            // If spent exceeds limit, full bar & infinite percent
            if (spent > limit) {
                Console.WriteLine($"{indent}[{new string('=', 20)}] ∞%");
                return;
            }

            // Calculate percentage of spent to limit
            double percentage = Math.Max(0, Math.Min(1, spent / limit));
            // Calculate fill and empty characters
            int filledLength = (int)(percentage * 20);
            int emptyLength = 20 - filledLength;

            // Display calculated progress bar with percentage
            Console.WriteLine($"{indent}[{new string('=', filledLength)}{new string(' ', emptyLength)}] {percentage:P0}");
        }
    }
}
