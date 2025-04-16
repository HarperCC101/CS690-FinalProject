// DisplayProgressBarTests.cs
// Arrange-Act-Assert (AAA) Pattern Unit Testing
using BudgetTrackerApp;
using Xunit;
using System;
using System.IO;

public class DisplayProgressBarTests {

    // If limit is 0 and spent is positive, is progress bar full and percent infinite?
    [Fact]
    public void DisplayProgressBar_ZeroLimitPositiveSpent_FullBarInfinitePercent()
    {
        // Arrange
        double spent = 50;
        double limit = 0;
        string expectedOutput = "  [====================] ∞%";
        TextWriter originalConsoleOut = Console.Out;
        using var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);

        // Act
        DisplayProgressBar.Show(spent, limit);

        // Assert
        Assert.Equal(expectedOutput + Environment.NewLine, consoleOutput.ToString());

        // Cleanup: Reset Console.Out
        Console.SetOut(originalConsoleOut);
    }

    // If limit is 0 and spent is 0, is progress bar empty and percent 0?
    [Fact]
    public void DisplayProgressBar_ZeroLimitZeroSpent_EmptyBarZeroPercent() {
        // Arrange
        double spent = 0;
        double limit = 0;
        string expectedOutput = "  [                    ] 0%";
        TextWriter originalConsoleOut = Console.Out;
        using var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);

        // Act
        DisplayProgressBar.Show(spent, limit);

        // Assert
        Assert.Equal(expectedOutput + Environment.NewLine, consoleOutput.ToString());

        // Cleanup: Reset Console.Out
        Console.SetOut(originalConsoleOut);
    }

    // If spent is greater than limit, is progress bar full and percent infinite?
    [Fact]
    public void DisplayProgressBar_SpentGTLimit_FullBarInfinitePercent() {
        // Arrange
        double spent = 150;
        double limit = 100;
        string expectedOutput = "  [====================] ∞%";
        TextWriter originalConsoleOut = Console.Out;
        using var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);

        // Act
        DisplayProgressBar.Show(spent, limit);

        // Assert
        Assert.Equal(expectedOutput + Environment.NewLine, consoleOutput.ToString());

        // Cleanup: Reset Console.Out
        Console.SetOut(originalConsoleOut);
    }

    // If spent is less than limit, is progress bar calculation correct?
    [Fact]
    public void DisplayProgressBar_SpentLTLimit_CorrectBarAndPercent() {
        // Arrange
        double spent = 50;
        double limit = 100;
        string expectedOutput = "  [==========          ] 50%";
        TextWriter originalConsoleOut = Console.Out;
        using var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);

        // Act
        DisplayProgressBar.Show(spent, limit);

        // Assert
        Assert.Equal(expectedOutput + Environment.NewLine, consoleOutput.ToString());

        // Cleanup: Reset Console.Out
        Console.SetOut(originalConsoleOut);
    }
}
