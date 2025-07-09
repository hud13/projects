// <copyright file="SpreadsheetPage.razor.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

namespace GUI.Client.Pages;

using CS3500.Formula;
using CS3500.Spreadsheet;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Diagnostics;

/// <summary>
/// This is a partial class that works with the provided HTML to create a 
/// locally-hosted spreadsheet web application. It combines methods and classes
/// written from previous problem sets into a functional spreadsheet. 
/// Filled in by Hudson Dalby
/// Date: 10/25/24
/// </summary>
public partial class SpreadsheetPage
{
    /// <summary>
    /// Based on your computer, you could shrink/grow this value based on performance.
    /// </summary>
    private const int ROWS = 50;

    /// <summary>
    /// Number of columns, which will be labeled A-Z.
    /// </summary>
    private const int COLS = 26;

    /// <summary>
    /// Provides an easy way to convert from an index to a letter (0 -> A)
    /// </summary>
    private char[] Alphabet { get; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();


    /// <summary>
    ///   Gets or sets the name of the file to be saved
    /// </summary>
    private string FileSaveName { get; set; } = "Spreadsheet.sprd";


    /// <summary>
    ///   <para> 
    ///   Gets or sets the data for all of the cells in the spreadsheet GUI. .   
    ///   </para>
    ///   <remarks>Backing Store for HTML</remarks>
    /// </summary>
    private string[,] CellsBackingStore { get; set; } = new string[ROWS, COLS];
    /// <summary>
    /// Gets or sets the value for the selected cell in the spreadsheet GUI
    /// </summary>
    private string[,] CellsValueStore { get; set; } = new string[ROWS, COLS];

    /// <summary>
    /// Variable to hold the currently selected cell, Set to "A1" by default.
    /// </summary>
    private string SelectedCell = "A1";

    /// <summary>
    /// Variable to hold the current contents of the cell. All cells empty by default. 
    /// </summary>
    private Object CurrentContents = string.Empty;

    /// <summary>
    /// String to be shown in the GUI click. 
    /// </summary>
    private string CurrentContentsString = string.Empty;

    /// <summary>
    /// Variable to hold the current contents of the cell. All cells empty by default. 
    /// </summary>
    private string CurrentValueString = string.Empty;

    /// <summary>
    /// Variable to hold the current contents of the cell. 
    /// </summary>
    private Object CurrentValue = string.Empty;

    /// <summary>
    /// Reference to the TextArea rendered on the webpage
    /// </summary>
    private ElementReference TextArea;

    /// <summary>
    ///  Variable for the currently selected row. Set to Zero by default. 
    /// </summary>
    private int SelectedRow = 0;

    /// <summary>
    /// Variable for the currently selected column. Set to Zero by default. 
    /// </summary>
    private int SelectedCol = 0;

    /// <summary>
    /// Boolean to determine whether error popup should be shown. 
    /// </summary>
    private bool ShowError = false;

    /// <summary>
    /// private string that contains error message to be displayed when exception is thrown. 
    /// </summary>
    private string ErrorMessage = string.Empty;

    /// <summary>
    /// Spreadsheet to be used with the GUI input data. 
    /// </summary>
    private Spreadsheet Spreadsheet = new Spreadsheet();

    /// <summary>
    /// Variable for PartyMode function. 
    /// </summary>
    private bool isPartyMode = false;

    /// <summary>
    /// Handler for when a cell is clicked
    /// </summary>
    /// <param name="row">The row component of the cell's coordinates</param>
    /// <param name="col">The column component of the cell's coordinates</param>
    private void CellClicked(int row, int col)
    {
        // Shows selected cell in correct format
        SelectedCell = $"{Alphabet[col]}{row + 1}";

        //Sets variables to appropriate row and column.
        SelectedRow = row;
        SelectedCol = col;


        //Gets content string to be displayed from Cells backing. 
        CurrentContents = CellsBackingStore[row, col];

        // Gets value string to be displayed from Cells backing. 
        CurrentValueString = CellsValueStore[row, col];


        TextArea.FocusAsync();
    }

    private void CellContentChanged(ChangeEventArgs e)
    {
        try
        {

            // This uses the null forgiving (!) and coalescing (??)
            // operators to get either the value that was typed in,
            // or the empty string if it was null
            string data = e.Value!.ToString() ?? "";

            // Adds the current data to the selected cell
            // Stores output variable as list to be reevaluated. 
            List<string> cellsToRecalculate = (List<string>)Spreadsheet.SetContentsOfCell(SelectedCell, data);

            // Updates the html display arrays 
            CurrentContents = data;
            CurrentValueString = Spreadsheet.GetCellValue(SelectedCell).ToString();

            if (CurrentValueString.Equals("CS3500.Formula.FormulaError"))
                CurrentValueString = "Error";


            // Recalculates each cell in the dependency list
            foreach (string cell in cellsToRecalculate)
            {

                (int row, int col) = CellNameToNumbers(cell);

                // Recalculate the cell value and update the value store.
                object newValue = Spreadsheet.GetCellValue(cell);

                if (newValue is FormulaError)
                {
                    foreach (string dependent in cellsToRecalculate)
                    {
                        if (dependent != SelectedCell)
                        {
                            (int r, int c) = CellNameToNumbers(dependent);
                            CellsValueStore[r, c] = "Error";
                        }
                    }
                }
                else
                {
                    CellsValueStore[row, col] = newValue.ToString();
                    // Adds the current cell to the backing array. 
                    CellsBackingStore[SelectedRow, SelectedCol] = data;
                }

            }
        }
        // Catches any exceptions and displays a unique error message for each. 
        catch (Exception ex)
        {
            // Clears the contents of the textbox. 
            CurrentContents = String.Empty;


            if (ex is CircularException)
            {
                ShowError = true;
                ErrorMessage = "Error: Circular dependency detected.";
                return;
            }

            else
            {
                ShowError = true;
                ErrorMessage = "Error: Formula Cannot be Evaluated.";
                return;
            }

        }
        finally
        {
            // Refresh the GUI to reflect the changes. 
            TextArea.FocusAsync();
        }
    }

    /// <summary>
    /// Helper method that converts the given cell name back to row and column numbers. 
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    private (int, int) CellNameToNumbers(string cell)
    {
        int col = Array.IndexOf(Alphabet, cell[0]);
        int row = int.Parse(cell.Substring(1)) - 1;

        return (row, col);
    }

    /// <summary>
    /// Helper method to make the error popup disappear when box is clicked
    /// </summary>
    private void DismissError()
    {
        ShowError = false;
    }
    /// <summary>
    /// Saves the current spreadsheet, by providing a download of a file
    /// containing the json representation of the spreadsheet.
    /// </summary>
    private async void SaveFile()
    {

        string data = Spreadsheet.ReturnJson();

        await JSRuntime.InvokeVoidAsync("downloadFile", FileSaveName,
            data);
    }

    /// <summary>
    /// This method will run when the file chooser is used, for loading a file.
    /// Uploads a file containing a json representation of a spreadsheet, and 
    /// replaces the current sheet with the loaded one.
    /// </summary>
    /// <param name="args">The event arguments, which contains the selected file name</param>
    private async void HandleFileChooser(EventArgs args)
    {
        try
        {
            string fileContent = string.Empty;

            InputFileChangeEventArgs eventArgs = args as InputFileChangeEventArgs ?? throw new Exception("unable to get file name");
            if (eventArgs.FileCount == 1)
            {
                var file = eventArgs.File;
                if (file is null)
                {
                    return;
                }

                using var stream = file.OpenReadStream();
                using var reader = new StreamReader(stream);

                // fileContent will contain the contents of the loaded file
                fileContent = await reader.ReadToEndAsync();


                // Clears the existing html backing spreadsheet. 
                ClearSpreadsheetData();

                // Replaces the spreadsheet content to the Json. 
                Spreadsheet.replaceWithJson(fileContent);

                // Gets the list of all non-empty cells 
                HashSet<string> cells = (HashSet<string>)Spreadsheet.GetNamesOfAllNonemptyCells();

                foreach (string cell in cells)
                {
                    //Converts the cell name to row and column variables
                    (int row, int col) = CellNameToNumbers(cell);

                    // Updates the content backing array 
                    CellsBackingStore[row, col] = Spreadsheet.GetCellContents(cell).ToString();

                    // Updates the value backing array
                    CellsValueStore[row, col] = Spreadsheet.GetCellValue(cell).ToString();
                }

                // Reflects the changes in UI. 
                StateHasChanged();
            }
        }
        catch (Exception e)
        {
            ShowError = true;
            ErrorMessage = "An error occurred while loading the file.";
            Debug.WriteLine(e);
            StateHasChanged();

        }
    }

    /// <summary>
    /// Clears the spreadsheet data displayed in the UI.
    /// </summary>
    private void ClearSpreadsheetData()
    {
        // Sets spreadsheet to new spreadsheet
        Spreadsheet = new Spreadsheet();

        // Reset all cells in the backing store to empty strings
        for (int row = 0; row < ROWS; row++)
        {
            for (int col = 0; col < COLS; col++)
            {
                CellsBackingStore[row, col] = string.Empty;
                CellsValueStore[row, col] = string.Empty;
            }
        }
    }

    /// <summary>
    /// Generates a random color to be used for the background of a random cell. 
    /// </summary>
    /// <returns></returns>
    private string GetRandomColor()
    {
        Random r = new Random();
        return $"#{r.Next(0x1000000):X6}";
    }

    /// <summary>
    /// Used with the GUI to toggle party mode. 
    /// </summary>
    private void PartyMode()
    {
        isPartyMode = !isPartyMode;

        StateHasChanged();
    }
}
