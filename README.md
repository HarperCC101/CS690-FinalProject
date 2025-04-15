# Aria's Budget Tracker

The Simple and fun way to budget using C#! Track expenses, set spending limits, control your future!

# Deployment Guide for Aria's Budget Tracker

This document outlines the steps to deploy and run Aria's Budget Tracker application.

## Prerequisites

* **.NET Runtime:** This application is built using .NET 8.0. The user must have the .NET 8.0 runtime installed on their system. If it's not installed, follow the official instructions on the Microsoft .NET website: [https://dotnet.microsoft.com/download/dotnet/8.0](https://dotnet.microsoft.com/download/dotnet/8.0)

## Deployment Steps

1.  **Download the Release Package:**
    * Go to the [Releases](https://github.com/HarperCC101/CS690-FinalProject/releases) page of this repository.
    * Download the `my-budget-app.zip` file to your local machine.

2.  **Extract the Application:**
    * **Command Line:** Use `unzip my-budget-app.zip` in the terminal where you downloaded the file. The contents will display in the current directory.
    * **Coursera-provided VS Code:**
        * Open the **File Explorer** (usually the top icon on the left sidebar).
        * **Right-click** in an empty area of the File Explorer where you want to upload the zip file.
        * Select **"Upload..."** from the context menu.
        * Choose the `my-budget-app.zip` file that you downloaded to your local machine.
        * Once the upload is complete, the `my-budget-app.zip` file will appear in your File Explorer.
        * Use the `unzip` command in the **terminal** to extract the contents of the zip file:
            ```bash
            unzip my-budget-app.zip
            ```
        * This will extract the contents of the zip file into the current directory.

3.  **Running the Application:**
    * From the directory  where you used `unzip` (directory containing `my-budget-app.dll` and `Budget.txt`), run:
        ```bash
        dotnet my-budget-app.dll
        ```

4.  **Testing the Application**
    * After running the application, you can verify its functionality by checking the results of the automated tests.
    * [Developer Documentation](https://github.com/HarperCC101/CS690-FinalProject/wiki/Developer-Documentation) contains all test instructions under the `Testing the Application` section.

## Notes

* **Text File:** The application uses a text file (`Budget.txt`) to store budget information. This file should be located in the same directory as the `my-budget-app.dll` file.
* **Permissions:** Ensure you have the necessary execute permissions for the .NET runtime and the application files if you encounter any permission-related issues.

## Support

If you encounter any issues during deployment or while using the application, please open an issue on the [GitHub Issues](https://github.com/HarperCC101/CS690-FinalProject/issues) page of this repository.

---

Thank you for using Aria's Budget Tracker!
