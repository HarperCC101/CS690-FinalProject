// DataManagerTests.cs
using BudgetTrackerApp;
using Xunit;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class DataManagerTests {
    private const string TestFilePath = "TestBudgetDM.txt";

    public DataManagerTests() {
        // Clean up test file if it exists
        if (File.Exists(TestFilePath)) {
            File.Delete(TestFilePath);
        }
    }

    [Fact]
    public void DataManager_LoadsCategoriesCorrectly() {
        // Arrange
        File.WriteAllLines(TestFilePath, new[] { "Groceries,100,50", "Entertainment,200,100" });
        DataManager dataManager = new DataManager(TestFilePath);

        // Act
        Dictionary<string, (double limit, double spent)> categories = dataManager.GetAllCategories();

        // Assert
        Assert.Equal(2, categories.Count);
        Assert.Equal((100, 50), categories["Groceries"]);
        Assert.Equal((200, 100), categories["Entertainment"]);
    }

    [Fact]
    public void DataManager_UpdatesSpentAmountCorrectly() {
        // Arrange
        File.WriteAllLines(TestFilePath, new[] { "Groceries,100,50" });
        DataManager dataManager = new DataManager(TestFilePath);

        // Act
        dataManager.UpdateData("Groceries", 25);
        Dictionary<string, (double limit, double spent)> categories = dataManager.GetAllCategories();

        // Assert
        Assert.Equal(75, categories["Groceries"].spent);
    }

    [Fact]
    public void DataManager_UpdatesSpentLimitCorrectly() {
        // Arrange
        File.WriteAllLines(TestFilePath, new[] { "Groceries,100,50" });
        DataManager dataManager = new DataManager(TestFilePath);

        // Act
        dataManager.SetSpendingLimit("Groceries", 150);
        Dictionary<string, (double limit, double spent)> categories = dataManager.GetAllCategories();

        // Assert
        Assert.Equal(150, categories["Groceries"].limit);
    }

    [Fact]
    public void DataManager_GetCategoriesFromFileCorrectly() {
        // Arrange
        File.WriteAllLines(TestFilePath, new[] { "Groceries,100,50", "Entertainment,200,100" });
        DataManager dataManager = new DataManager(TestFilePath);

        // Act
        List<string> categoryList = dataManager.GetCategoriesFromFile();

        // Assert
        Assert.Equal(2, categoryList.Count);
        Assert.Contains("Groceries", categoryList);
        Assert.Contains("Entertainment", categoryList);
    }
}
