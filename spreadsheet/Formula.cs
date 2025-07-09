/// <summary>
///   <para>
///     This project is to examine and evaluate the expression in each cell of the Spreadsheet.
///     It has been implemented using the PS starting points provided by Profs Joe, Danny, and Jim. 
///     A constructor that ensures correct syntax rules has been implemented in PS2.
///     Various methods to evaluate the formula method were implemented in PS4.
///   </para>
/// <authors> Hudson Dalby </authors>
/// <date> 9/20/2024 </date>
/// </summary>


namespace CS3500.Formula;

using System.Text.RegularExpressions;

/// <summary>
///   <para>
///     This class represents formulas written in standard infix notation using standard precedence
///     rules.  The allowed symbols are non-negative numbers written using double-precision
///     floating-point syntax; variables that consist of one ore more letters followed by
///     one or more numbers; parentheses; and the four operator symbols +, -, *, and /.
///   </para>
///   <para>
///     Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
///     a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable;
///     and "x 23" consists of a variable "x" and a number "23".  Otherwise, spaces are to be removed.
///   </para>
///   <para>
///     For Assignment Two, you are to implement the following functionality:
///   </para>
///   <list type="bullet">
///     <item>
///        Formula Constructor which checks the syntax of a formula.
///     </item>
///     <item>
///        Get Variables
///     </item>
///     <item>
///        ToString
///     </item>
///   </list>
/// </summary>
public class Formula
{
    /// <summary>
    /// String to categorize the current token when making sure rules are followed
    /// </summary>
    private string thisToken = "none";

    /// <summary>
    /// Keeps the ongoing concatenation of the current formula as a string, starts empty by default. 
    /// </summary>
    private string formulaAsString = string.Empty;

    /// <summary>
    /// A list of the unique variables that have been encountered in the formula. 
    /// </summary>
    private List<string> variables = new List<string>();

    /// <summary>
    ///   All variables are letters followed by numbers.  This pattern
    ///   represents valid variable name strings.
    /// </summary>
    private const string VariableRegExPattern = @"[a-zA-Z]+\d+";

    /// <summary>
    ///   Initializes a new instance of the <see cref="Formula"/> class.
    ///   <para>
    ///     Creates a Formula from a string that consists of an infix expression written as
    ///     described in the class comment.  If the expression is syntactically incorrect,
    ///     throws a FormulaFormatException with an explanatory Message.  See the assignment
    ///     specifications for the syntax rules you are to implement.
    ///   </para>
    ///   <para>
    ///     Non Exhaustive Example Errors:
    ///   </para>
    ///   <list type="bullet">
    ///     <item>
    ///        Invalid variable name, e.g., x, x1x  (Note: x1 is valid, but would be normalized to X1)
    ///     </item>
    ///     <item>
    ///        Empty formula, e.g., string.Empty
    ///     </item>
    ///     <item>
    ///        Mismatched Parentheses, e.g., "(("
    ///     </item>
    ///     <item>
    ///        Invalid Following Rule, e.g., "2x+5"
    ///     </item>
    ///   </list>
    /// </summary>
    /// <param name="formula"> The string representation of the formula to be created.</param>
    public Formula(string formula)
    {
        int parenthesisCount = 0;
        string currentType = "none";
        string previousType = "none";
        Boolean firstToken = false;

        // Check if Rule#1 (One Token Rule) is broken
        if (String.IsNullOrWhiteSpace(formula))
            throw new FormulaFormatException("Formula must contain at least one token.");

        // Calls the GetTokens method to return a list of tokens
        List<string> tokensList = GetTokens(formula);

        foreach (string token in tokensList)
        {
            // Sets necessary variables for checks to be performed
            thisToken = token;
            currentType = TokenType(token);

            // Throws error if consecutive tokens of the same type are found
            // Does not apply to consecutive parenthesis
            if (currentType.Equals("Number")|currentType.Equals("Operator")|currentType.Equals("Variable"))
            {
                if (currentType.Equals(previousType))
                    throw new FormulaFormatException("Consecutive operators of the same type are not allowed");
            }

            // Exception occurs if Rule#2 (Valid Tokens) is broken
            if (currentType.Equals("Invalid"))
                throw new FormulaFormatException("Formula contains invalid characters.");

            // Conditional to check if Rule#5 (First Token Rule) has been broken
            // Throws an error if the first token isn't opening parenthesis, number, or variable. 
            if (!firstToken)
            {
                if (currentType.Equals("Operator") | currentType.Equals("ClosingParenthesis"))
                {
                    throw new FormulaFormatException("Formula contains an invalid first token");
                }
                firstToken = true;
            }

            // Increments the parenthesis count if opening parenthesis is seen 
            if (currentType.Equals("OpeningParenthesis"))
                parenthesisCount++;

            // Decrements the parenthesis count if opening parenthesis is seen 
            if (currentType.Equals("ClosingParenthesis"))
                parenthesisCount--;

            // Throws error if Rule#3 (Closing parenthesis seen exceed opening parenthesis) is broken
            if (parenthesisCount < 0)
                throw new FormulaFormatException("Closing parenthesis seen cannot exceed opening parenthesis");

            // Checks to see if Rule#7 (Parenthesis/Operator Following Rule) is broken

            // Case where opening parenthesis is followed by closing parenthesis
            if (currentType.Equals("ClosingParenthesis") && previousType.Equals("OpeningParenthesis"))
                throw new FormulaFormatException("Closing parenthesis cannot immediately follow opening parenthesis");

            // Case where opening parenthesis is followed by operator
            if (currentType.Equals("Operator") && previousType.Equals("OpeningParenthesis"))
                throw new FormulaFormatException("Operator cannot immediately follow opening parenthesis");

            // Case where operator is followed by closing parenthesis
            if (currentType.Equals("ClosingParenthesis") && previousType.Equals("Operator"))
                throw new FormulaFormatException("Closing parenthesis cannot immediately follow opening parenthesis");

            // Checks to see if Rule#8 (Extra Following Rule) is broken

            // Case where a variable or closing parenthesis is followed by a number
            if (currentType.Equals("Number") && (previousType.Equals("Variable")||previousType.Equals("ClosingParenthesis")))
                throw new FormulaFormatException("A number cannot follow a variable or closing parenthesis");

            // Case where a number or closing parenthesis is followed by a variable
            if (currentType.Equals("Variable") && (previousType.Equals("Number")||previousType.Equals("ClosingParenthesis")))
                throw new FormulaFormatException("A variable cannot follow a number or closing parenthesis");

            // Case where closing parenthesis is directly followed by opening parenthesis
            if (currentType.Equals("OpeningParenthesis") && previousType.Equals("ClosingParenthesis"))
                throw new FormulaFormatException("A variable cannot follow a number or closing parenthesis");

            // Conditionals if the token is a variable
            if (currentType.Equals("Variable"))
            {
                // Normalizes the variable if it isn't already
                string normalized = token.ToUpper();
                thisToken = normalized;

                // Adds variable to the list of variables if it doesn't already exist within it
                Boolean existsInList = variables.Contains(token);
                if (!existsInList)
                    variables.Add(thisToken);
            }

            // Normalizes doubles before adding to the created formula string
            if (currentType.Equals("Number"))
            {
                double result;
                _=double.TryParse(thisToken, out result);

                thisToken = result.ToString();

            }

            // Adds the current token to the created formula string
            if (string.IsNullOrEmpty(formulaAsString))
            {
                formulaAsString = thisToken;
            }
            else
                formulaAsString += thisToken;

            // Keeps track of the previous type for the next loop iteration
            previousType = currentType;
        }

        // Check to see if Rule#4 (Balanced Parenthesis Rule) has been broken.
        if (parenthesisCount != 0)
            throw new FormulaFormatException("Amount of opening & closing parenthesis is unequal.");

        // Check to see if Rule#6 (Last Token Rule) has been broken
        if (currentType.Equals("OpeningParenthesis")|currentType.Equals("Operator"))
            throw new FormulaFormatException("Formula must end with number, variable, or closing parenthesis");
    }

    /// <summary>
    ///   <para>
    ///     Returns a set of all the variables in the formula.
    ///   </para>
    ///   <remarks>
    ///     Important: no variable may appear more than once in the returned set, even
    ///     if it is used more than once in the Formula.
    ///   </remarks>
    ///   <para>
    ///     For example, if N is a method that converts all the letters in a string to upper case:
    ///   </para>
    ///   <list type="bullet">
    ///     <item>new("x1+y1*z1").GetVariables() should enumerate "X1", "Y1", and "Z1".</item>
    ///     <item>new("x1+X1"   ).GetVariables() should enumerate "X1".</item>
    ///   </list>
    /// </summary>
    /// <returns> the set of variables (string names) representing the variables referenced by the formula. </returns>
    public ISet<string> GetVariables()
    {
        HashSet<string> output = new HashSet<string>(variables);
        //Returns the set of variables created in the constructor
        return output;
    }

    /// <summary>
    ///   <para>
    ///     Returns a string representation of a canonical form of the formula.
    ///   </para>
    ///   <para>
    ///     The string will contain no spaces.
    ///   </para>
    ///   <para>
    ///     If the string is passed to the Formula constructor, the new Formula f 
    ///     will be such that this.ToString() == f.ToString().
    ///   </para>
    ///   <para>
    ///     All of the variables in the string will be normalized.  This
    ///     means capital letters.
    ///   </para>
    ///   <para>
    ///       For example:
    ///   </para>
    ///   <code>
    ///       new("x1 + y1").ToString() should return "X1+Y1"
    ///       new("X1 + 5.0000").ToString() should return "X1+5".
    ///   </code>
    ///   <para>
    ///     This code should execute in O(1) time.
    ///   <para>
    /// </summary>
    /// <returns>
    ///   A canonical version (string) of the formula. All "equal" formulas
    ///   should have the same value here.
    /// </returns>
    public override string ToString()
    {
        // Returns the concatenated string created in the constructor
        return formulaAsString;
    }

    /// <summary>
    ///   Reports whether "token" is a variable.  It must be one or more letters
    ///   followed by one or more numbers.
    /// </summary>
    /// <param name="token"> A token that may be a variable. </param>
    /// <returns> true if the string matches the requirements, e.g., A1 or a1. </returns>
    private static bool IsVar(string token)
    {
        // notice the use of ^ and $ to denote that the entire string being matched is just the variable
        string standaloneVarPattern = $"^{VariableRegExPattern}$";
        return Regex.IsMatch(token, standaloneVarPattern);
    }

    /// <summary>
    ///   <para>
    ///     Given an expression, enumerates the tokens that compose it.
    ///   </para>
    ///   <para>
    ///     Tokens returned are:
    ///   </para>
    ///   <list type="bullet">
    ///     <item>left paren</item>
    ///     <item>right paren</item>
    ///     <item>one of the four operator symbols</item>
    ///     <item>a string consisting of one or more letters followed by one or more numbers</item>
    ///     <item>a double literal</item>
    ///     <item>and anything that doesn't match one of the above patterns</item>
    ///   </list>
    ///   <para>
    ///     There are no empty tokens; white space is ignored (except to separate other tokens).
    ///   </para>
    /// </summary>
    /// <param name="formula"> A string representing an infix formula such as 1*B1/3.0. </param>
    /// <returns> The ordered list of tokens in the formula. </returns>
    private static List<string> GetTokens(string formula)
    {
        List<string> results = [];

        string lpPattern = @"\(";
        string rpPattern = @"\)";
        string opPattern = @"[\+\-*/]";
        string doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
        string spacePattern = @"\s+";

        // Overall pattern
        string pattern = string.Format(
                                        "({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                        lpPattern,
                                        rpPattern,
                                        opPattern,
                                        VariableRegExPattern,
                                        doublePattern,
                                        spacePattern);

        // Enumerate matching tokens that don't consist solely of white space.
        foreach (string s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
        {
            if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
            {
                results.Add(s);
            }
        }

        return results;
    }

    /// <summary>
    /// Helper method to determine the type of the current token to check for valid syntax rules
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    private static String TokenType(string token)
    {
        // Sets the current token type to invalid by default
        string currentType = "Invalid";
        double parseDouble;

        // The given token is an operator
        if (Regex.IsMatch(token, @"[\+\-*/]"))
            currentType = "Operator";

        // The given token is a variable
        if (Regex.IsMatch(token, VariableRegExPattern))
            currentType = "Variable";

        // The given token is a Number 
        if (double.TryParse(token, out parseDouble))
            currentType = "Number";

        // The given token is an opening parenthesis
        if (Regex.IsMatch(token, @"\("))
            currentType = "OpeningParenthesis";

        // The given token is a closing parenthesis
        if (Regex.IsMatch(token, @"\)"))
            currentType = "ClosingParenthesis";

        return currentType;
    }

    /// <summary>
    /// <para>
    /// Reports whether f1 == f2, using the notion of equality from the <see cref="Equals"/> method.
    /// </para>
    /// </summary>
    /// <param name="f1"> The first of two formula objects. </param>
    /// <param name="f2"> The second of two formula objects. </param>
    /// <returns> true if the two formulas are the same.</returns>
    public static bool operator ==(Formula f1, Formula f2)
    {
        // Use the overloaded equals operator to assert equals
        return f1.Equals(f2);
    }
    /// <summary>
    /// <para>
    /// Reports whether f1 != f2, using the notion of equality from the <see cref="Equals"/> method.
    /// </para>
    /// </summary>
    /// <param name="f1"> The first of two formula objects. </param>
    /// <param name="f2"> The second of two formula objects. </param>
    /// <returns> true if the two formulas are not equal to each other.</returns>
    public static bool operator !=(Formula f1, Formula f2)
    {
        // Use the overloaded equals operator to assert not equals
        return !(f1.Equals(f2));
    }
    /// <summary>
    /// <para>
    /// Determines if two formula objects represent the same formula.
    /// </para>
    /// <para>
    /// By definition, if the parameter is null or does not reference
    /// a Formula Object then return false.
    /// </para>
    /// <para>
    /// Two Formulas are considered equal if their canonical string representations
    /// (as defined by ToString) are equal.
    /// </para>
    /// </summary>
    /// <param name="obj"> The other object.</param>
    /// <returns>
    /// True if the two objects represent the same formula.
    /// </returns>
    public override bool Equals(object? obj)
    {
        if (obj == null) return false;

        // Returns false if the object is not of Formula type
        if (!obj.GetType().Equals(typeof(Formula)))
            return false;

        // Normalizes the two strings by converting each to string
        string? otherFormula = obj.ToString();
        string? thisFormula = this.ToString();

        // Compares the two strings and returns the result if equal 
        return thisFormula.Equals(otherFormula);

    }
    /// <summary>
    /// <para>
    /// Evaluates this Formula, using the lookup delegate to determine the values of
    /// variables.
    /// </para>
    /// <remarks>
    /// When the lookup method is called, it will always be passed a normalized (capitalized)
    /// variable name. The lookup method will throw an ArgumentException if there is
    /// not a definition for that variable token.
    /// </remarks>
    /// <para>
    /// If no undefined variables or divisions by zero are encountered when evaluating
    /// this Formula, the numeric value of the formula is returned. Otherwise, a
    /// FormulaError is returned (with a meaningful explanation as the Reason property).
    /// </para>
    /// <para>
    /// This method should never throw an exception.
    /// </para>
    /// </summary>
    /// <param name="lookup">
    /// <para>
    /// Given a variable symbol as its parameter, lookup returns the variable's value
    /// (if it has one) or throws an ArgumentException (otherwise). This method will expect
    /// variable names to be normalized.
    /// </para>
    /// </param>
    /// <returns> Either a double or a FormulaError, based on evaluating the formula.</returns>
    public object Evaluate(Lookup lookup)
    {
        // Initialize stack for the values in the expression
        Stack<double> valueStack = new Stack<double>();

        // Initialize stack for the operators in the expression 
        Stack<string> operatorStack = new Stack<string>();

        // Stores the list of tokens in the current formula 
        List<string> tokens = new List<string>();

        // Stores the current value of a variable if it is able to be retrieved
        double variableValue = 0;

        // Variables to store two values for an operation between them 
        double rightValue;
        double leftValue;

        // Normalizes the formula and fills the list of tokens 
        string normalized = this.ToString();
        tokens = GetTokens(normalized);

        // Goes through each token in the formula from left to right
        foreach (string t in tokens)
        {
            // Condition to check if current token is a number and deal with it accordingly
            double number;
            if (double.TryParse(t, out number))
            {

                //If / is at the top of the operator stack, divide by value of t
                if (IsOnTop(operatorStack, "/"))
                {
                    // Pops the top value of the value stack to use in operation 
                    double currentValue = valueStack.Pop();

                    // Pops the operator out of the stack
                    operatorStack.Pop();

                    // Checks to see if dividing by zero happens
                    if (number == 0)
                        return new FormulaError("Divide by zero");

                    // Applies the operator and pushes the new value onto the stack 
                    valueStack.Push(currentValue / number);
                }

                // If * is at the top of the operation stack, multiply by value of t
                else if (IsOnTop(operatorStack, "*"))
                {
                    // Pops the top value of the value stack to use in operation 
                    double currentValue = valueStack.Pop();

                    // Pops the operator out of the stack
                    operatorStack.Pop();

                    // pushes the new value onto the stack
                    valueStack.Push(currentValue * number);
                }

                //Pushes the token onto the value stack 
                else
                    valueStack.Push(number);

            }

            // Condition to check if current token is a variable and deal with it accordingly
            if (IsVar(t))
            {
                // Uses the delegate to look up the double value of current token
                try
                {
                    variableValue = lookup(t);
                }
                // If the variable doesn't have a value, return a new Formula Error with the message
                catch (Exception e)
                {
                    return new FormulaError(e.Message);
                }

                // If / is on top of the operator stack, divide top value by variable value
                if (IsOnTop(operatorStack, "/"))
                {
                    // Pops the top value of the value stack to use in operation 
                    double currentValue = valueStack.Pop();

                    // Pops the top value of the operator stack
                    operatorStack.Pop();

                    // Checks to see if dividing by zero happens
                    if (variableValue == 0)
                        return new FormulaError("Divide by zero");

                    // Applies the operator and pushes the new value onto the stack 
                    valueStack.Push(currentValue / variableValue);
                }

                // If * is at the top of the operation stack, multiply by value of t
                else if (IsOnTop(operatorStack, "*"))
                {
                    // Pops the top value of the value stack to use in operation 
                    double currentValue = valueStack.Pop();

                    // Pops the current operator to be applied 
                    operatorStack.Pop();

                    // Applies the operator and pushes the new value onto the stack
                    valueStack.Push(currentValue * variableValue);

                }

                // Otherwise adds the variable to the value stack 
                else valueStack.Push(variableValue);
            }

            // Condition to check if current token is + or - and deal with it accordingly
            if (Regex.IsMatch(t, @"[\+\-]"))
            {
                // If the current top operator is a +, add two popped values together
                if (IsOnTop(operatorStack, "+"))
                {
                    // Gets the two values on top of the stack 
                    rightValue = valueStack.Pop();
                    leftValue = valueStack.Pop();

                    // Pushes the sum of the two to the value stack 
                    valueStack.Push(leftValue + rightValue);
                    operatorStack.Pop();
                }

                // The current operator is a -, subtract the popped values
                if (IsOnTop(operatorStack, "-"))
                {
                    // Gets the two values on top of the stack 
                    rightValue = valueStack.Pop();
                    leftValue = valueStack.Pop();

                    // Pushes the difference of the two to the value stack
                    valueStack.Push(leftValue - rightValue);
                    operatorStack.Pop();
                }
                // Otherwise adds the operator to the stack 
                operatorStack.Push(t);
            }

            // Condition to check if current token is * or / and deal with it accordingly
            if (Regex.IsMatch(t, @"[*/]"))
            {
                // Pushes t onto the operator stack
                operatorStack.Push(t);

            }

            // Condition to check if the current token is an opening parenthesis and deal accordingly
            if (Regex.IsMatch(t, @"\("))
            {
                // Pushes t onto the operator stack
                operatorStack.Push(t);

            }

            // Condition to check if current token is a closing parenthesis and deal accordingly
            if (Regex.IsMatch(t, @"\)"))
            {
                // If + is on top of the operator stack 
                if (IsOnTop(operatorStack, "+"))
                {
                    // Pops two values from the top of value stack
                    rightValue = valueStack.Pop();
                    leftValue = valueStack.Pop();

                    // Pop the current operator on the stack
                    operatorStack.Pop();

                    // Applies the operator and pushes back onto the stack
                    valueStack.Push(rightValue + leftValue);
                }


                // If - is on top of the operator stack 
                if (IsOnTop(operatorStack, "-"))
                {
                    // Pops two values from the top of value stack
                    rightValue = valueStack.Pop();
                    leftValue = valueStack.Pop();

                    // Applies the operator and pushes back onto the stack
                    valueStack.Push(leftValue - rightValue);
                    operatorStack.Pop();
                }

                // Pop the opening parenthesis on the stack
                operatorStack.Pop();


                // if * is on top of the operator stack
                if (IsOnTop(operatorStack, "*"))
                {
                    // Pops two values from the top of value stack
                    rightValue = valueStack.Pop();
                    leftValue = valueStack.Pop();

                    // Pops the operator from the stack
                    operatorStack.Pop();

                    // Applies the operator and pushes back onto the stack 
                    valueStack.Push(leftValue * rightValue);
                }

                // if / is on top of the operator stack 
                if (IsOnTop(operatorStack, "/"))
                {
                    // Pops two values from the top of value stack
                    rightValue = valueStack.Pop();
                    leftValue = valueStack.Pop();

                    // Pops the operator from the stack
                    operatorStack.Pop();

                    // Checks for divide by 0
                    if (rightValue == 0)
                        return new FormulaError("Divide by zero");

                    // Applies the operator and pushes back onto the stack 
                    valueStack.Push(leftValue / rightValue);
                }
            }
        }

        // Condition where the operator stack is not empty
        if (operatorStack.Count > 0)
        {
            // Gets the remaining value variables from the stack 
            rightValue = valueStack.Pop();
            leftValue = valueStack.Pop();

            // Final operator on the stack is a + 
            if (operatorStack.Pop().Equals("+"))
                return leftValue + rightValue;

            // Final operator on the stack is a -
            else
                return leftValue - rightValue;

        }

        // Condition where the operator stack is empty
        return valueStack.Pop();
    }

    /// <summary>
    /// Helper method to determine whether a particular object is 
    /// currently at the top of the stack
    /// </summary>
    /// <param name="s"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    private static bool IsOnTop(Stack<string> stack, string token)
    {
        // Condition to ensure the stack contains items. 
        if (stack.Count > 0)

            // Check if the value found on the top of the stack equals the token
            return stack.Peek().Equals(token);

        // Returns false if stack is empty or token does not equal the peeked value
        else return false;

    }
    /// <summary>
    /// <para>
    /// Returns a hash code for this Formula. If f1.Equals(f2), then it must be the
    /// case that f1.GetHashCode() == f2.GetHashCode(). Ideally, the probability that two
    /// randomly-generated unequal Formulas have the same hash code should be extremely small.
    /// </para>
    /// </summary>
    /// <returns> The hashcode for the object. </returns>
    public override int GetHashCode()
    {
        // Normalizes the string using the ToString method in formula
        string? normalized = this.ToString();
        return normalized.GetHashCode();
    }
}

/// <summary>
/// Used as a possible return value of the Formula.Evaluate method.
/// </summary>
public class FormulaError
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FormulaError"/> class.
    /// <para>
    /// Constructs a FormulaError containing the explanatory reason.
    /// </para>
    /// </summary>
    /// <param name="message"> Contains a message for why the error occurred.</param>
    public FormulaError(string message)
    {
        Reason = message;
    }
    /// <summary>
    /// Gets the reason why this FormulaError was created.
    /// </summary>
    public string Reason { get; private set; }
}
/// <summary>
/// Any method meeting this type signature can be used for
/// looking up the value of a variable.
/// </summary>
/// <exception cref="ArgumentException">
/// If a variable name is provided that is not recognized by the implementing method,
/// then the method should throw an ArgumentException.
/// </exception>
/// <param name="variableName">
/// The name of the variable (e.g., "A1") to lookup.
/// </param>
/// <returns> The value of the given variable (if one exists). </returns>
public delegate double Lookup(string variableName);



/// <summary>
///   Used to report syntax errors in the argument to the Formula constructor.
/// </summary>
public class FormulaFormatException : Exception
{
    /// <summary>
    ///   Initializes a new instance of the <see cref="FormulaFormatException"/> class.
    ///   <para>
    ///      Constructs a FormulaFormatException containing the explanatory message.
    ///   </para>
    /// </summary>
    /// <param name="message"> A developer defined message describing why the exception occured.</param>
    public FormulaFormatException(string message)
        : base(message)
    {
        // All this does is call the base constructor. No extra code needed.
    }
}
