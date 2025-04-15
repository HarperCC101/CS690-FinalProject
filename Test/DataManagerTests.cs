// DataManagerTests.cs
using BudgetTrackerApp;
using Xunit;
using System.IO;
using System.Collections.Generic;

public class DataManagerTests {
    private const string TestFilePath = "TestBudgetDM.txt";

    public DataManagerTests() {
        // Clean up test file if it exists
        if (File.Exists(TestFilePath)) {
            File.Delete(TestFilePath);
        }
    }

    [Fact]
    public void DataManager_InitializesFileIfNoFile() {
        // Arrange
        string TempTestFilePath = "TempTestBudgetDM.txt"; // Create a temporary file path
        if (File.Exists(TempTestFilePath)) {
            File.Delete(TempTestFilePath);
        }
        DataManager dataManager = new DataManager(TempTestFilePath); // Use the temporary file path

        // Act & Assert
        Assert.True(File.Exists(TempTestFilePath));
        string fileContent = File.ReadAllText(TempTestFilePath);
        Assert.Equal("Groceries,200,0\nUtilities,200,0\nEntertainment,200,0\nTransportation,200,0\nMiscellaneous,200,0\n", fileContent);

        //Cleanup
        File.Delete(TempTestFilePath);
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
    public void DataManager_UpdateData_InvalidAmount() {
        // Arrange
        File.WriteAllLines(TestFilePath, new[] { "Groceries,100,50" });
        DataManager dataManager = new DataManager(TestFilePath);

        // Act
        dataManager.UpdateData("Groceries", -10);
        dataManager.UpdateData("Groceries", 0);
        Dictionary<string, (double limit, double spent)> categories = dataManager.GetAllCategories();

        // Assert
        Assert.Equal(50, categories["Groceries"].spent); // Should remain unchanged
    }

    [Fact]
    public void DataManager_UpdatesPersistToFile() {
        // Arrange
        File.WriteAllLines(TestFilePath, new[] { "Groceries,100,50" });
        DataManager dataManager = new DataManager(TestFilePath);

        // Act
        dataManager.UpdateData("Groceries", 25);
        dataManager = new DataManager(TestFilePath); // Re-load from file

        Dictionary<string, (double limit, double spent)> categories = dataManager.GetAllCategories();

        // Assert
        Assert.Equal(75, categories["Groceries"].spent);
    }

    [Fact]
    public void DataManager_SetSpendingLimit_InvalidLimit() {
        // Arrange
        File.WriteAllLines(TestFilePath, new[] { "Groceries,100,50" });
        DataManager dataManager = new DataManager(TestFilePath);

        // Act
        dataManager.SetSpendingLimit("Groceries", -10);
        dataManager.SetSpendingLimit("Groceries", 0);
        Dictionary<string, (double limit, double spent)> categories = dataManager.GetAllCategories();

        // Assert
        Assert.Equal(100, categories["Groceries"].limit); // Should remain unchanged
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
