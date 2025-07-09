/// <copyright file="Spreadsheet.cs" company="UofU-CS3500">
/// Copyright (c) 2024 UofU-CS3500. All rights reserved.
/// </copyright>
/// Written by Joe Zachary for CS 3500, September 2013
/// Update by Profs Kopta and de St. Germain, Fall 2021, Fall 2024
/// - Updated return types
/// - Updated documentation
/// <summary>
/// <para>
/// This class contains the structural elements of the spreadsheet as specified in PS5.  
/// Given Cell values are filled with formula, double, or string contents, and 
/// immediate dependencies & cycles are handled.
/// Note: This spreadsheet has been updated to the specifications of PS6.
/// </para>
/// </summary>
/// Filled in by Hudson Dalby
/// Date: 10/18/24

namespace CS3500.Spreadsheet;

using CS3500.DependencyGraph;
using CS3500.Formula;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

/// <summary>
/// <para>
/// Thrown to indicate that a change to a cell will cause a circular dependency.
/// </para>
/// </summary>
public class CircularException : Exception
{
}
/// <summary>
/// <para>
/// Thrown to indicate that a name parameter was invalid.
/// </para>
/// </summary>
public class InvalidNameException : Exception
{
}

/// <summary>
/// <para>
/// Thrown to indicate that a read or write attempt has failed with
/// an expected error message informing the user of what went wrong.
/// </para>
/// </summary>
public class SpreadsheetReadWriteException : Exception
{
    /// <summary>
    /// <para>
    /// Creates the exception with a message defining what went wrong.
    /// </para>
    /// </summary>
    /// <param name="msg"> An informative message to the user. </param>
    public SpreadsheetReadWriteException(string msg)
    : base(msg)
    {
    }
}

/// <summary>
/// <para>
/// An Spreadsheet object represents the state of a simple spreadsheet. A
/// spreadsheet represents an infinite number of named cells.
/// </para>
/// <para>
/// Valid Cell Names: A string is a valid cell name if and only if it is one or
/// more letters followed by one or more numbers, e.g., A5, BC27.
/// </para>
/// <para>
/// Cell names are case insensitive, so "x1" and "X1" are the same cell name.
/// Your code should normalize (uppercased) any stored name but accept either.
/// </para>
/// <para>
/// A spreadsheet represents a cell corresponding to every possible cell name. (This
/// means that a spreadsheet contains an infinite number of cells.) In addition to
/// a name, each cell has a contents and a value. The distinction is important.
/// </para>
/// <para>
/// The <b>contents</b> of a cell can be (1) a string, (2) a double, or (3) a Formula.
/// If the contents of a cell is set to the empty string, the cell is considered empty.
/// </para>
/// <para>
/// By analogy, the contents of a cell in Excel is what is displayed on
/// the editing line when the cell is selected.
/// </para>
/// <para>
/// In a new spreadsheet, the contents of every cell is the empty string. Note:
/// this is by definition (it is IMPLIED, not stored).
/// </para>
/// <para>
/// The <b>value</b> of a cell can be (1) a string, (2) a double, or (3) a FormulaError.
/// (By analogy, the value of an Excel cell is what is displayed in that cell's position
/// in the grid.) We are not concerned with cell values yet, only with their contents,
/// but for context:
/// </para>
/// <list type="number">
/// <item>If a cell's contents is a string, its value is that string.</item>
/// <item>If a cell's contents is a double, its value is that double.</item>
/// <item>
/// <para>
/// If a cell's contents is a Formula, its value is either a double or a FormulaError,
/// as reported by the Evaluate method of the Formula class. For this assignment,
/// you are not dealing with values yet.
/// </para>
/// </item>
/// </list>
/// <para>
/// Spreadsheets are never allowed to contain a combination of Formulas that establish
/// a circular dependency. A circular dependency exists when a cell depends on itself,
/// either directly or indirectly.
/// For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
/// A1 depends on B1, which depends on C1, which depends on A1. That's a circular
/// dependency.
/// </para>
/// </summary>
public class Spreadsheet
{
    /// <summary>
    /// Regex pattern that represents a valid variable 
    /// *This expression was taken from an earlier problem set*
    /// </summary>
    private const string VariableRegExPattern = @"^[a-zA-Z]+\d+$";

    /// <summary>
    /// DependencyGraph variable to keep track of the relationships between cells in the spreadsheet. 
    /// </summary>
    private DependencyGraph dependencyGraph = new();


    /// <summary>
    /// Dictionary variable for mapping names of cells to cell objects. 
    /// </summary>
    [JsonInclude]
    private Dictionary<string, Cell> Cells = [];

    /// <summary>
    /// Provides a copy of the normalized names of all of the cells in the spreadsheet
    /// that contain information (i.e., non-empty cells).
    /// </summary>
    /// <returns>
    /// A set of the names of all the non-empty cells in the spreadsheet.
    /// </returns>
    public ISet<string> GetNamesOfAllNonemptyCells()
    {
        // Creates a HashSet to store the names of cells with content. 
        HashSet<string> nonemptyCells = new HashSet<string>();

        // Iterates through each name/cell object in the spreadsheet
        foreach (var cell in Cells)
        {
            // Assuming the content is not null, 
            // If the cell has content, the value of "content" will be something other than String.Empty. 
            if (!cell.Value.Content.Equals(String.Empty))
            {
                // adds the name of the cell to the set if it has content
                nonemptyCells.Add(cell.Key);
            }

        }
        // Returns the collection of nonempty Cells 
        return nonemptyCells;
    }
    /// <summary>
    /// Returns the contents (as opposed to the value) of the named cell.
    /// </summary>
    ///
    /// <exception cref="InvalidNameException">
    /// Thrown if the name is invalid.
    /// </exception>
    ///
    /// <param name="name">The name of the spreadsheet cell to query. </param>
    /// <returns>
    /// The contents as either a string, a double, or a Formula.
    /// See the class header summary.
    /// </returns>
    public object GetCellContents(string name)
    {
        // Checks to see if the name is valid 
        // If name does not follow variable naming convention, InvalidNameException is thrown
        if (!Regex.IsMatch(name, VariableRegExPattern))
            throw new InvalidNameException();

        // Normalizes the name if it isn't already
        name = name.ToUpper();

        // Tries to find an existing cell given the name 
        Cells.TryGetValue(name, out var cell);

        // If cell exists, return the content of the cell object 
        if (cell != null)
            return cell.Content;

        // Otherwise cell does not exist, return default cell value.
        return String.Empty;
    }
    /// <summary>
    /// Set the contents of the named cell to the given number.
    /// </summary>
    ///
    /// <exception cref="InvalidNameException">
    /// If the name is invalid, throw an InvalidNameException.
    /// </exception>
    ///
    /// <param name="name"> The name of the cell. </param>
    /// <param name="number"> The new contents of the cell. </param>
    /// <returns>
    /// <para>
    /// This method returns an ordered list consisting of the passed in name
    /// followed by the names of all other cells whose value depends, directly
    /// or indirectly, on the named cell.
    /// </para>
    /// <para>
    /// The order must correspond to a valid dependency ordering for recomputing
    /// all of the cells, i.e., if you re-evaluate each cells in the order of the list,
    /// the overall spreadsheet will be correctly updated.
    /// </para>
    /// <para>
    /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
    /// list [A1, B1, C1] is returned, i.e., A1 was changed, so then A1 must be
    /// evaluated, followed by B1, followed by C1.
    /// </para>
    /// </returns>
    private IList<string> SetCellContents(string name, double number)
    {
        // If cell hasn't been added to spreadsheet yet, add it directly and create new cell object
        if (!Cells.ContainsKey(name))
        {
            // Creates a new cell and sets the content to the number
            Cell s = new Cell();
            s.Content = number;

            // sets the cell StringForm to the double as a string
            s.StringForm = number.ToString();

            // Add the new cell to the spreadsheet dictionary 
            Cells.Add(name, s);
        }

        // Cell has already been added to spreadsheet, and cells may have to be recalculated. 
        else
        {
            // Sets the cell value to the given parameter
            Cells[name].Content = number;

            // Sets the StringForm to the double as a string
            Cells[name].StringForm = number.ToString();

            // Recalculates the cells to maintain correct dependencies. 
            dependencyGraph.ReplaceDependents(name, new List<string>());
        }

        // Sets the changed flag to true
        Changed = true;

        // Creates a new list to be returned
        List<string> returnList = [];

        // Gets the ordered list of direct dependents and adds them to return list
        returnList.AddRange(GetCellsToRecalculate(name));

        // Returns the created list
        return returnList;

    }
    /// <summary>
    /// The contents of the named cell becomes the given text.
    /// </summary>
    ///
    /// <exception cref="InvalidNameException">
    /// If the name is invalid, throw an InvalidNameException.
    /// </exception>
    /// <param name="name"> The name of the cell. </param>
    /// <param name="text"> The new contents of the cell. </param>
    /// <returns>
    /// The same list as defined in <see cref="SetCellContents(string, double)"/>.
    /// </returns>
    private IList<string> SetCellContents(string name, string text)
    {
        // If spreadsheet does not contain the name already, adds a new cell
        if (!Cells.ContainsKey(name))
        {
            // Creates a new cell and sets the content to the provided string
            Cell s = new();
            s.Content = text;

            // Sets the StringForm to the string
            s.StringForm = text;

            // Add the new cell to the Spreadsheet Dictionary
            Cells.Add(name, s);

        }


        // The spreadsheet contains a cell with the given name
        else
        {
            // Sets the cell content to the given string
            Cells[name].Content = text;

            // Sets the StringForm to the string
            Cells[name].StringForm = text;

            // Recalculates the cells to maintain correct dependencies. 
            dependencyGraph.ReplaceDependents(name, new List<string>());
        }

        // Sets Changed flag to true
        Changed = true;

        // String to be returned by the method
        List<string> returnList = [];

        // Gets the ordered list of direct dependents and adds them to return list
        returnList.AddRange(GetCellsToRecalculate(name));

        // Returns the newly edited list
        return returnList;
    }
    /// <summary>
    /// Set the contents of the named cell to the given formula.
    /// </summary>
    /// <exception cref="InvalidNameException">
    /// If the name is invalid, throw an InvalidNameException.
    /// </exception>
    /// <exception cref="CircularException">
    /// <para>
    /// If changing the contents of the named cell to be the formula would
    /// cause a circular dependency, throw a CircularException, and no
    /// change is made to the spreadsheet.
    /// </para>
    /// </exception>
    /// <param name="name"> The name of the cell. </param>
    /// <param name="formula"> The new contents of the cell. </param>
    /// <returns>
    /// The same list as defined in <see cref="SetCellContents(string, double)"/>.
    /// </returns>
    private IList<string> SetCellContents(string name, Formula formula)
    {

        // Replace the dependents in the dependency graph with new ones in formula
        dependencyGraph.ReplaceDependents(name, formula.GetVariables());

        // Check each variable in the formula's variables
        foreach (var variable in formula.GetVariables())
        {
            //If the dependents of this variable contain given variable, a cycle is in the graph. 
            if (dependencyGraph.GetDependents(variable).Contains(name))
                throw new CircularException();
            break;
        }

        // If the spreadsheet doesn't contain the named cell, add content to it 
        if (!Cells.ContainsKey(name))
        {
            // Creates a new cell object and sets its content to the formula
            Cell s = new();
            // Sets the content of the cell to formula
            s.Content = formula;
            // creates StringForm based on content
            s.StringForm = "=" + formula.ToString();
            // Adds the spreadsheet 
            Cells.Add(name, s);

        }

        // Spreadsheet already contains the named cell, change the content. 
        else
        {
            // Sets the cell content to the given string
            Cells[name].Content = formula;
            // Sets StringForm based on new content
            Cells[name].StringForm = "=" + formula.ToString();
        }

        // Sets the Changed flag to true
        Changed = true;

        // String to be returned by the method
        List<string> returnList = [];

        // Gets the ordered list of direct dependents and adds them to return list
        returnList.AddRange(GetCellsToRecalculate(name));

        // Returns the newly edited list
        return returnList;

    }
    /// <summary>
    /// Returns an enumeration, without duplicates, of the names of all cells whose
    /// values depend directly on the value of the named cell.
    /// </summary>
    /// <param name="name"> This <b>MUST</b> be a valid name. </param>
    /// <returns>
    /// <para>
    /// Returns an enumeration, without duplicates, of the names of all cells
    /// that contain formulas containing name.
    /// </para>
    /// <para>For example, suppose that: </para>
    /// <list type="bullet">
    /// <item>A1 contains 3</item>
    /// <item>B1 contains the formula A1 * A1</item>
    /// <item>C1 contains the formula B1 + A1</item>
    /// <item>D1 contains the formula B1 - C1</item>
    /// </list>
    /// <para> The direct dependents of A1 are B1 and C1. </para>
    /// </returns>
    private IEnumerable<string> GetDirectDependents(string name)
    {
        // Returns the dependents of the current string from the Spreadsheet dependency graph
        return dependencyGraph.GetDependees(name);
    }
    /// <summary>
    /// <para>
    /// This method is implemented for you, but makes use of your GetDirectDependents.
    /// </para>
    /// <para>
    /// Returns an enumeration of the names of all cells whose values must
    /// be recalculated, assuming that the contents of the cell referred
    /// to by name has changed. The cell names are enumerated in an order
    /// in which the calculations should be done.
    /// </para>
    /// <exception cref="CircularException">
    /// If the cell referred to by name is involved in a circular dependency,
    /// throws a CircularException.
    /// </exception>
    /// <para>
    /// For example, suppose that:
    /// </para>
    /// <list type="number">
    /// <item>
    /// A1 contains 5
    /// </item>
    /// <item>
    /// B1 contains the formula A1 + 2.
    /// </item>
    /// <item>
    /// C1 contains the formula A1 + B1.
    /// </item>
    /// <item>
    /// D1 contains the formula A1 * 7.
    /// </item>
    /// <item>
    /// E1 contains 15
    /// </item>
    /// </list>
    /// <para>
    /// If A1 has changed, then A1, B1, C1, and D1 must be recalculated,
    /// and they must be recalculated in an order which has A1 first, and B1 before C1
    /// (there are multiple such valid orders).
    /// The method will produce one of those enumerations.
    /// </para>
    /// <para>
    /// PLEASE NOTE THAT THIS METHOD DEPENDS ON THE METHOD GetDirectDependents.
    /// IT WON'T WORK UNTIL GetDirectDependents IS IMPLEMENTED CORRECTLY.
    /// </para>
    /// </summary>
    /// <param name="name"> The name of the cell. Requires that name be a valid cell name.</param>
    /// <returns>
    /// Returns an enumeration of the names of all cells whose values must
    /// be recalculated.
    /// </returns>
    private IEnumerable<string> GetCellsToRecalculate(string name)
    {
        LinkedList<string> changed = new();
        HashSet<string> visited = [];
        Visit(name, name, visited, changed);
        return changed;
    }
    /// <summary>
    /// A helper for the GetCellsToRecalculate method.
    /// Visits each of the cells in dependencies list to determine which cells need recalculating 
    /// and the correct order in which to do so. 
    /// </summary>
    /// <param name="start"> The name of the cell the recalculating must occur at. </param>
    /// <param name="name"> The name of the current cell being visited.</param>
    /// <param name="visited"> List of cells that have been traversed </param>
    /// <param name="changed"> List of cells that need to be recalculated .</param>
    private void Visit(string start, string name, ISet<string> visited,
    LinkedList<string> changed)
    {
        // Marks the current cell as visited
        visited.Add(name);

        // Iterates through the direct dependents in the cell. 
        foreach (string n in GetDirectDependents(name))
        {
            // Throws a CircularException if starting node is found in dependents
            if (n.Equals(start))
            {
                throw new CircularException();
            }

            // If a dependent cell has not been visited, visit it. 
            else if (!visited.Contains(n))
            {
                // Visits the node in dependent list recursively 
                Visit(start, n, visited, changed);
            }
        }
        // Add the cell to the changed list 
        changed.AddFirst(name);
    }

    /// <summary>
    /// A basic cell class that holds the details of the cell. 
    /// </summary>
    private class Cell
    {
        /// <summary>
        /// A variable to hold the contents of the cell (Is an empty string by default).  
        /// </summary>
        [JsonIgnore]
        public object Content { get; set; } = string.Empty;

        /// <summary>
        /// A variable to hold the string representation of a cell  
        /// This value is set and determined in the appropriate SetCellValue method
        /// </summary>
        public string StringForm { get; set; } = string.Empty;

        /// <summary>
        /// A variable to hold the value of the cell 
        /// Can be double, string, or FormulaException 
        /// </summary>
        [JsonIgnore]
        public object Value { get; set; } = string.Empty;


        // Default constructor for the cell object. 
        public Cell()
        {
            Content = String.Empty;
            StringForm = String.Empty;
        }

    }

    /// <summary>
    /// <para>
    /// Return the value of the named cell, as defined by
    /// <see cref="GetCellValue(string)"/>.
    /// </para>
    /// </summary>
    /// <param name="name"> The cell in question. </param>
    /// <returns>
    /// <see cref="GetCellValue(string)"/>
    /// </returns>
    /// <exception cref="InvalidNameException">
    /// If the provided name is invalid, throws an InvalidNameException.
    /// </exception>
    public object this[string name]
    {
        // Invokes the GetCellValue method on the cell with the provided name.
        // If the name is invalid, an InvalidNameException will be thrown. 
        get
        {
            return GetCellValue(name);
        }

    }
    /// <summary>
    /// True if this spreadsheet has been changed since it was
    /// created or saved (whichever happened most recently),
    /// False otherwise.
    /// </summary>
    /// 
    [JsonIgnore]
    public bool Changed { get; private set; }

    /// <summary>
    /// Constructs a spreadsheet using no arguments 
    /// (A new spreadsheet is created with every cell empty.)
    /// </summary>
    public Spreadsheet()
    {
        // Sets Changed to false by default.
        Changed = false;
    }
    /// <summary>
    /// Constructs a spreadsheet using the saved data in the file refered to by
    /// the given filename.
    /// <see cref="Save(string)"/>
    /// </summary>
    /// <exception cref="SpreadsheetReadWriteException">
    /// Thrown if the file can not be loaded into a spreadsheet for any reason
    /// </exception>
    /// <param name="filename">The path to the file containing the spreadsheet to load</param>
    public Spreadsheet(string filename)
    {
        try
        {
            // converts the provided file into JSON text
            var spreadsheetData = JsonSerializer.Deserialize<Spreadsheet>(File.ReadAllText(filename));


            if (spreadsheetData != null)
            {
                // For each value of Cells in the file, Set the cell contents
                foreach (var cellname in spreadsheetData.Cells.Keys)
                {
                    SetContentsOfCell(cellname, spreadsheetData.Cells[cellname].StringForm);
                }

                // Changed is set to false by default.
                Changed = false;
            }
        }
        catch
        {
            // Throws error if for any reason file can not be read/found. 
            throw new SpreadsheetReadWriteException(filename);
        }
    }
    /// <summary>
    /// <para>
    /// Writes the contents of this spreadsheet to the named file using a JSON format.
    /// If the file already exists, overwrite it.
    /// </para>
    /// <para>
    /// The output JSON should look like the following.
    /// </para>
    /// <para>
    /// For example, consider a spreadsheet that contains a cell "A1"
    /// with contents being the double 5.0, and a cell "B3" with contents
    /// being the Formula("A1+2"), and a cell "C4" with the contents "hello".
    /// </para>
    /// <para>
    /// This method would produce the following JSON string:
    /// </para>
    /// <code>
    /// {
    /// "Cells": {
    /// "A1": {
    /// "StringForm": "5"
    /// },
    /// "B3": {
    /// "StringForm": "=A1+2"
    /// },
    /// "C4": {
    /// "StringForm": "hello"
    /// }
    /// }
    /// }
    /// </code>
    /// <para>
    /// You can achieve this by making sure your data structure is a dictionary
    /// and that the contained objects (Cells) have property named "StringForm"
    /// (if this name does not match your existing code, use the JsonPropertyName
    /// attribute).
    /// </para>
    /// <para>
    /// There can be 0 cells in the dictionary, resulting in { "Cells" : {} }
    /// </para>
    /// <para>
    /// Further, when writing the value of each cell...
    /// </para>
    /// <list type="bullet">
    /// <item>
    /// If the contents is a string, the value of StringForm is that string
    /// </item>
    /// <item>
    /// If the contents is a double d, the value of StringForm is d.ToString()
    /// </item>
    /// <item>
    /// If the contents is a Formula f, the value of StringForm is "=" + f.ToString()
    /// </item>
    /// </list>
    /// </summary>
    /// <param name="filename"> The name (with path) of the file to save to.</param>
    /// <exception cref="SpreadsheetReadWriteException">
    /// If there are any problems opening, writing, or closing the file,
    /// the method should throw a SpreadsheetReadWriteException with an
    /// explanatory message.
    /// </exception>
    public void Save(string filename)
    {
       
        try
        {
            // Creates a spreadsheet with the read JSON string. 
            var spreadsheetData = this.ReturnJson();

            // Writes the file to the specified location
            File.WriteAllText(filename, spreadsheetData);

            // Sets changed back to false. 
            Changed = false;
        }
        catch
        {
            // Throws error if for any reason file can not be read/found. 
            throw new SpreadsheetReadWriteException(filename);
        }
    }
    /// <summary>
    /// <para>
    /// Return the value of the named cell.
    /// </para>
    /// </summary>
    /// <param name="name"> The cell in question. </param>
    /// <returns>
    /// Returns the value (as opposed to the contents) of the named cell. The return
    /// value should be either a string, a double, or a CS3500.Formula.FormulaError.
    /// </returns>
    /// <exception cref="InvalidNameException">
    /// If the provided name is invalid, throws an InvalidNameException.
    /// </exception>
    public object GetCellValue(string name)
    {
        // Checks to see if the name is valid 
        if (!Regex.IsMatch(name, VariableRegExPattern))
            throw new InvalidNameException();

        //Normalizes the cell name if it isn't already
        name = name.ToUpper();

        // Returns default cell value if cell is not in dictionary but name is valid. 
        if (!Cells.ContainsKey(name))
            return String.Empty;

        // Returns the stored value of the Cell
        return Cells[name].Value;
    }
    /// <summary>
    /// <para>
    /// Set the contents of the named cell to be the provided string
    /// which will either represent (1) a string, (2) a number, or
    /// (3) a formula (based on the prepended '=' character).
    /// </para>
    /// <para>
    /// Rules of parsing the input string:
    /// </para>
    /// <list type="bullet">
    /// <item>
    /// <para>
    /// If 'content' parses as a double, the contents of the named
    /// cell becomes that double.
    /// </para>
    /// </item>
    /// <item>
    /// If the string does not begin with an '=', the contents of the
    /// named cell becomes 'content'.
    /// </item>
    /// <item>
    /// <para>
    /// If 'content' begins with the character '=', an attempt is made
    /// to parse the remainder of content into a Formula f using the Formula
    /// constructor. There are then three possibilities:
    /// </para>
    /// <list type="number">
    /// <item>
    /// If the remainder of content cannot be parsed into a Formula, a
    /// CS3500.Formula.FormulaFormatException is thrown.
    /// </item>
    /// <item>
    /// Otherwise, if changing the contents of the named cell to be f
    /// would cause a circular dependency, a CircularException is thrown,
    /// and no change is made to the spreadsheet.
    /// </item>
    /// <item>
    /// Otherwise, the contents of the named cell becomes f.
    /// </item>
    /// </list>
    /// </item>
    /// </list>
    /// </summary>
    /// <returns>
    /// <para>
    /// The method returns a list consisting of the name plus the names
    /// of all other cells whose value depends, directly or indirectly,
    /// on the named cell. The order of the list should be any order
    /// such that if cells are re-evaluated in that order, their dependencies
    /// are satisfied by the time they are evaluated.
    /// </para>
    /// <example>
    /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
    /// list {A1, B1, C1} is returned.
    /// </example>
    /// </returns>
    /// <exception cref="InvalidNameException">
    /// If name is invalid, throws an InvalidNameException.
    /// </exception>
    /// <exception cref="CircularException">
    /// If a formula would result in a circular dependency, throws CircularException.
    /// </exception>
    public IList<string> SetContentsOfCell(string name, string content)
    {
        // Checks to see if the name is valid 
        if (!Regex.IsMatch(name, VariableRegExPattern))
            throw new InvalidNameException();


        // Normalizes the name if it isn't already
        name = name.ToUpper();

        // List of Cells to be recalculated to return. 
        IList<string> dependees;


        // Check to see if content is empty string (So further checks don't cause exceptions).
        if (content.Equals(String.Empty))
        {
            dependees = SetCellContents(name, content);

            Cells[name].Value = content;

        }

        // See if content can be parsed as a double, invokes method if appropriate
        else if (Double.TryParse(content, out double result))
        {
            dependees =  SetCellContents(name, result);

            // Sets the value of the string to double. 
            Cells[name].Value = result;

        }
        // Checks if first character is '=', indicating a formula
        else if (content[0] == '=')
        {
            // Removes '=' from the string to be turned into a formula and sets cell contents
            content = content.Substring(1);
            Formula formula = new Formula(content);

            // Sets the cell contents and value to the formula
            dependees =  SetCellContents(name, formula);
            Cells[name].Value = formula.Evaluate(LookupMethod);
        }
        else
        {
            // Content is a string by default
            dependees = SetCellContents(name, content);

            Cells[name].Value = content;
        }


        // Checks each cell value in the dependees list
        foreach (var cellname in dependees)
        {
            // Evaluate the cell if the content is a formula. 
            if (Cells[cellname].Content is Formula f)
            {
                // If the cell value is a formula, evaluate it. 
                Cells[cellname].Value = f.Evaluate(LookupMethod);
            }
        }

        // Returns the list of cells that need to be updated. 
        return dependees;
    }

    /// <summary>
    /// Lookup method to be used by Formula Evaluate. 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    /// <exception cref="InvalidNameException"></exception>
    private double LookupMethod(string name)
    {
        // Lookup each cell by name in the evaluate method. 
        if (Cells.TryGetValue(name, out Cell? cell))
        {
            if (cell.Value is double d)
            {
                return d;
            }
        }

        // If cell name is invalid, throw new exception 
        throw new InvalidNameException();
    }

    /// <summary>
    /// Returns this as a json string. 
    /// To be used in the GUI when loading a spreadsheet from a file. 
    /// </summary>
    /// <returns></returns>
    public string ReturnJson()
    {
        // Creates json options for a nicer, more readable file. 
        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

            // Creates a spreadsheet with the read JSON string. 
            var spreadsheetData = JsonSerializer.Serialize(this, jsonOptions);

        return spreadsheetData.ToString();
        }


    /// <summary>
    /// Helper method in GUI to replace the existing spreadsheet data. 
    /// Uses the json string provided as a parameter. 
    /// </summary>
    /// <param name="json"></param>
    /// <exception cref="SpreadsheetReadWriteException"></exception>
     public Spreadsheet replaceWithJson(string json)
    {

        try
        {
            // Deserializes the provided json string into a new spreadsheet 
            var spreadsheetData = JsonSerializer.Deserialize<Spreadsheet>(json);


            if (spreadsheetData != null)
            {
                // For each value of Cells in the file, Set the cell contents
                foreach (var cellname in spreadsheetData.Cells.Keys)
                {
                    SetContentsOfCell(cellname, spreadsheetData.Cells[cellname].StringForm);
                }

                // Changed is set to false by default.
                Changed = false;
            }

            if (null != spreadsheetData)
                return spreadsheetData;

            else
                throw new Exception();
        }
        catch
        {
            // Throws error if for any reason file can not be read/found. 
            throw new SpreadsheetReadWriteException("");
        }
    }
}
