/// <copyright file="FormulaSyntaxTests.cs" company="UofU-CS3500">
///   Copyright 2024 UofU-CS3500. All rights reserved.
/// </copyright>
/// <authors> Hudson Dalby </authors>
/// <date> 9/20/2024 </date>
/// NOTE: Some rules tested may indirectly confirm valid inputs for other rules.
/// Because of this, some sections have simple tests removed to decrease redundancy.

namespace CS3500.FormulaTests;

using CS3500.Formula;

[TestClass]
public class FormulaTests
{

    // --- Tests for One Token Rule ---
    [TestMethod]
    public void FormulaConstructor_TestOneToken_Valid()
    {
        _ = new Formula("1");
    }

    [TestMethod]
    public void FormulaConstructor_TestMultipleTokens_Valid()
    {
        _ = new Formula("13+10");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestZeroToken_Invalid()
    {
        _ = new Formula(string.Empty);
    }

    // --- Tests for Valid Token Rule ---
    [TestMethod]

    public void FormulaConstructor_TestInteger_Valid()
    {
        _ = new Formula("12");
    }

    [TestMethod]
    public void FormulaConstructor_TestDouble_Valid()
    {
        _ = new Formula("1.54");
    }

    [TestMethod]
    public void FormulaConstructor_TestLowercaseExponent_Valid()
    {
        _ = new Formula("1e6");
    }

    [TestMethod]
    public void FormulaConstructor_TestUppercaseExponent_Valid()
    {
        _ = new Formula("1E4");
    }

    [TestMethod]
    public void FormulaConstructor_TestOneNegativeExponent_Valid()
    {
        _ = new Formula("1E-4");
    }

    [TestMethod]
    public void FormulaConstructor_TestComplexNegativeExponent_Valid()
    {
        _ = new Formula("5.42E-4");
    }

    [TestMethod]
    public void FormulaConstructor_TestAddOperator_Valid()
    {
        _ = new Formula("1+2");
    }

    [TestMethod]
    public void FormulaConstructor_TestSubtractOperator_Valid()
    {
        _ = new Formula("3-1");
    }

    [TestMethod]
    public void FormulaConstructor_TestMultiplyOperator_Valid()
    {
        _ = new Formula("1*2");
    }

    [TestMethod]
    public void FormulaConstructor_TestDivideOperator_Valid()
    {
        _ = new Formula("1/2");
    }

    [TestMethod]
    public void FormulaConstructor_TestParenthesis_Valid()
    {
        _ = new Formula("(1)");
    }

    [TestMethod]
    public void FormulaConstructor_TestSimpleVariable_Valid()
    {
        _ = new Formula("a1");
    }

    [TestMethod]
    public void FormulaConstructor_TestSimpleUppercaseVariable_Valid()
    {
        _ = new Formula("A1");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestIncorrectVariable_Invalid()
    {
        _ = new Formula("1a");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestLowercaseVariable_Invalid()
    {
        _ = new Formula("a");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestUppercaseVariable_Invalid()
    {
        _ = new Formula("A");
    }

    [TestMethod]
    public void FormulaConstructor_TestMultipleLetterVariable_Valid()
    {
        _ = new Formula("abc1");
    }

    [TestMethod]
    public void FormulaConstructor_TestMultipleNumberVariable_Valid()
    {
        _ = new Formula("a124");
    }

    [TestMethod]
    public void FormulaConstructor_TestComplexVariable_Valid()
    {
        _ = new Formula("ab57");
    }

    [TestMethod]
    public void FormulaConstructor_TestComplexVariable2_Valid()
    {
        _ = new Formula("ZEW47");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestSymbol_Invalid()
    {
        _ = new Formula("$");
    }

    // --- Tests for Closing Parenthesis Rule ---
    [TestMethod]
    public void FormulaConstructor_TestSingleParenthesisSet_Valid()
    {
        _ = new Formula("( 1 )");
    }

    [TestMethod]
    public void FormulaConstructor_TestMultipleParenthesisExpression_Valid()
    {
        _ = new Formula("( 1 + 2) - ( 3 * 2 )");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestExtraClosingParenthesis_Invalid()
    {
        _ = new Formula("(1))");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestClosingParenthesisExceedsOpening_Invalid()
    {
        _ = new Formula("(1)+)(()");
    }

    // --- Tests for Balanced Parentheses Rule ---
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOpeningParenthesisExceedsClosing_Invalid()
    {
        _ = new Formula("(()");
    }

    [TestMethod]
    public void FormulaConstructor_TestParenthesisSet_Valid()
    {
        _ = new Formula("(1 + 1)");
    }

    [TestMethod]
    public void FormulaConstructor_TestMultipleParenthesisNested_Valid()
    {
        _ = new Formula("(1+(3*6))");
    }

    [TestMethod]
    public void FormulaConstructor_TestMultipleParenthesisSeparated_Valid()
    {
        _ = new Formula("(1)+(3)-(4)");
    }

    // --- Tests for First Token Rule ---
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestFirstTokenOperator_Invalid()
    {
        _ = new Formula("+");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestFirstTokenClosingParenthesis_Invalid()
    {
        _ = new Formula(")");
    }

    [TestMethod]
    public void FormulaConstructor_TestFirstTokenVariable_Valid()
    {
        _ = new Formula("a1 + 4");
    }

    [TestMethod]
    public void FormulaConstructor_TestFirstTokenNumberDouble_Valid()
    {
        _ = new Formula("7.35 - 4");
    }

    [TestMethod]
    public void FormulaConstructor_TestFirstTokenOpeningParenthesisExpression_Valid()
    {
        _ = new Formula("(1 + (4*3)) - (2)");
    }

    // --- Tests for  Last Token Rule ---
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestLastTokenOperator_Invalid()
    {
        _ = new Formula("a1+4-");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestLastTokenOpeningParenthesis_Invalid()
    {
        _ = new Formula("(1+4)(");
    }

    [TestMethod]
    public void FormulaConstructor_TestLastTokenVariable_Valid()
    {
        _ = new Formula("a1 + b6");
    }

    [TestMethod]
    public void FormulaConstructor_TestLastTokenNumber_Valid()
    {
        _ = new Formula("7 / 1e8");
    }

    [TestMethod]
    public void FormulaConstructor_TestLastTokenClosingParenthesisExpression_Valid()
    {
        _ = new Formula("7 * (8 + 2)");
    }

    // --- Tests for Parentheses/Operator Following Rule ---
    // (Valid operations for this rule are validated in prior testing)
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOperatorAfterParenthesis_Invalid()
    {
        _ = new Formula("(*1)");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestEmptyParenthesis_Invalid()
    {
        _ = new Formula("()");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOperatorAfterClosingParenthesis_Invalid()
    {
        _ = new Formula("((7)*)");
    }

    // --- Tests for Extra Following Rule ---
    // (Valid operations for this rule have been verified in prior testing)
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOpeningAfterClosingParenthesis_Invalid()
    {
        _ = new Formula("(a1)(3)");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestSpacedValidTokens_Invalid()
    {
        _ = new Formula("123 45");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestNextFollowing_Invalid()
    {
        _ = new Formula("123a5");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestNumberAfterClosingParenthesis_Invalid()
    {
        _ = new Formula("(7)4 + 3");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestEmptyReversedParenthesis_Invalid()
    {
        _ = new Formula(")(");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_ExtrafollowingRule_Invalid()
    {
        _ = new Formula("a4 1");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_LoneParenthesis_Invalid()
    {
        _ = new Formula("(1 + 1");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestVariableAfterClosingParenthesis_Invalid()
    {
        _ = new Formula("(7)a1");
    }

    // Tests for GetVariables Method
    [TestMethod]
    public void GetVariables_ListUppercase_ReturnsExpected()
    {
        Formula tester = new Formula("A1 + B3");
        HashSet<string> expected = new HashSet<string>();
        expected.Add("A1");
        expected.Add("B3");

        Boolean areEqual = expected.SetEquals(tester.GetVariables());
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void GetVariables_ListLowercase_ReturnsExpected()
    {
        Formula tester = new Formula("a1 + b3");
        HashSet<string> expected = new HashSet<string>();
        expected.Add("A1");
        expected.Add("B3");

        Boolean areEqual = expected.SetEquals(tester.GetVariables());
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void GetVariables_ListMixedcase_ReturnsExpected()
    {
        Formula tester = new Formula("a1 + B3 - c4");
        HashSet<string> expected = new HashSet<string>();
        expected.Add("A1");
        expected.Add("B3");
        expected.Add("C4");

        Boolean areEqual = expected.SetEquals(tester.GetVariables());
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void GetVariables_MixedList_ReturnsExpected()
    {
        Formula tester = new Formula("a1 + 4 - (3 * r2)");
        HashSet<string> expected = new HashSet<string>();
        expected.Add("A1");
        expected.Add("R2");

        Boolean areEqual = expected.SetEquals(tester.GetVariables());
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void GetVariables_EmptyList_ReturnsExpected()
    {
        Formula tester = new Formula("3 + 17");
        HashSet<string> expected = new HashSet<string>();

        Boolean areEqual = expected.SetEquals(tester.GetVariables());
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void GetVariables_RepeatedVariable_ReturnsExpected()
    {
        Formula tester = new Formula("a4 + a4 / b7");
        HashSet<string> expected = new HashSet<string>();
        expected.Add("A4");
        expected.Add("B7");

        Boolean areEqual = expected.SetEquals(tester.GetVariables());
        Assert.IsTrue(areEqual);
    }

    // Tests for toString Method

    [TestMethod]
    public void ToString_BasicExpression_ReturnsExpected()
    {
        Formula tester = new Formula("1+3");
        string expected = "1+3";

        Assert.AreEqual(expected, tester.ToString());
    }

    [TestMethod]
    public void ToString_SpacedExpression_ReturnsExpected()
    {
        Formula tester = new Formula("1   +  3");
        string expected = "1+3";

        Assert.AreEqual(expected, tester.ToString());
    }

    [TestMethod]
    public void ToString_VariableExpressionUppercase_ReturnsExpected()
    {
        Formula tester = new Formula("A1 + B2 / M7 ");
        string expected = "A1+B2/M7";

        Assert.AreEqual(expected, tester.ToString());
    }

    [TestMethod]
    public void ToString_VariableExpressionLowercase_ReturnsExpected()
    {
        Formula tester = new Formula("a1 + b2 / m7 ");
        string expected = "A1+B2/M7";

        Assert.AreEqual(expected, tester.ToString());
    }

    [TestMethod]
    public void ToString_ScientificNotation_ReturnsExpected()
    {
        Formula tester = new Formula("4E2 + 3 ");
        string expected = "400+3";

        Assert.AreEqual(expected, tester.ToString());
    }

    [TestMethod]
    public void ToString_ScientificNotationNegative_ReturnsExpected()
    {
        Formula tester = new Formula("4E-1 + 3");
        string expected = "0.4+3";

        Assert.AreEqual(expected, tester.ToString());
    }

    [TestMethod]
    public void ToString_DecimalDouble_ReturnsExpected()
    {
        Formula tester = new Formula("3.4");
        string expected = "3.4";

        Assert.AreEqual(expected, tester.ToString());
    }

    [TestMethod]
    public void ToString_ComplexExpression_ReturnsExpected()
    {
        Formula tester = new Formula("4E2 + b4 - (4 * 6.0) ");
        string expected = "400+B4-(4*6)";

        Assert.AreEqual(expected, tester.ToString());
    }

    // Tests for PS4 Formula Methods begin here:  

    // --- Tests for Evaluate Method ---

    // A Lookup containing multiple variable values for testing 
    private static double TestLookup(string t)
    {
        if (t == "A1")
            return 3;
        if (t == "B2")
            return 5;
        if (t == "C4")
            return 7.21;
        if (t == "N9")
            return 0;
        if (t == "Q0")
            throw new ArgumentException("no value");
        else return 1;
    }

    [TestMethod]
    public void Evaluate_NoVariableValue_ReturnsFormulaException()
    {
        Formula f = new Formula("1 + Q0");
        Object evaluate = f.Evaluate(TestLookup);
        Assert.IsInstanceOfType(evaluate, typeof(FormulaError));
    }

    [TestMethod]
    public void Evaluate_AdditionExpression_ReturnsExpected()
    {
        Formula f = new Formula("1 + 1");
        Assert.AreEqual(2, (double)f.Evaluate(s => 0));
    }

    [TestMethod]
    public void Evaluate_MultipleOperators_ReturnsExpected()
    {
        Formula f = new Formula("2 + 3 - 1");
        Assert.AreEqual(4, (double)f.Evaluate(s => 0));
    }

    [TestMethod]
    public void Evaluate_ExpressionWithVariable_ReturnsExpected()
    {
        Formula f = new Formula("2 * A1");
        Assert.AreEqual((double)6, f.Evaluate(TestLookup));
    }

    [TestMethod]
    public void Evaluate_ExpressionWithVariable2_ReturnsExpected()
    {
        Formula f = new Formula("2 * C4");
        Assert.AreEqual(14.42, (double)f.Evaluate(TestLookup), 10e9);
    }

    [TestMethod]
    public void Evaluate_ExpressionMultiplyFirst_ReturnsExpected()
    {
        Formula f = new Formula("2 + 3 * 3");
        Assert.AreEqual((double)11, f.Evaluate(s => 0));
    }
    [TestMethod]
    public void Evaluate_ExpressionDivideFirst_ReturnsExpected()
    {
        Formula f = new Formula("4 - 3 / 3");
        Assert.AreEqual((double)3, f.Evaluate(s => 0));
    }

    [TestMethod]
    public void Evaluate_ParenthesesPrecedence_ReturnsExpected()
    {
        Formula f = new Formula("(2 + 3) * 3");
        Assert.AreEqual((double)15, f.Evaluate(s => 0));
    }

    [TestMethod]
    public void Evaluate_MultipleParentheses_ReturnsExpected()
    {
        Formula f = new Formula("(5 * 3) - (3 + 7)");
        Assert.AreEqual((double)5, f.Evaluate(s => 0));
    }

    [TestMethod]
    public void Evaluate_NestedParentheses_ReturnsExpected()
    {
        Formula f = new Formula("(4 + (3 + 3) * 2)");
        Assert.AreEqual((double)16, f.Evaluate(s => 0));
    }

    [TestMethod]
    public void Evaluate_MultipleVariables_ReturnsExpected()
    {
        Formula f = new Formula("A1 + B2 * Z3");
        Assert.AreEqual(8, (double)f.Evaluate(TestLookup));
    }

    [TestMethod]
    public void Evaluate_ComplexExpression_ReturnsExpected()
    {
        Formula f = new Formula("2 + (10 / (A1 * 2.5))");
        Assert.AreEqual(3.33333333, (double)f.Evaluate(TestLookup), 10e9);
    }

    [TestMethod]
    public void Evaluate_DoubleExpression_ReturnsExpected()
    {
        Formula f = new Formula("2.5 + 3.1 - 4.4");
        Assert.AreEqual(1.2, (double)f.Evaluate(s => 0), 10e9);
    }

    [TestMethod]
    public void Evaluate_DoubleExpressionMultiplyDivide_ReturnsExpected()
    {
        Formula f = new Formula("4.2 * 0.8 / 1.2 ");
        Assert.AreEqual(2.8, (double)f.Evaluate(s => 0), 10e9);
    }

    [TestMethod]
    public void Evaluate_MultiplyByZero_ReturnsExpected()
    {
        Formula f = new Formula("3 * 0");
        Assert.AreEqual((double)0, f.Evaluate(s => 0));
    }

    [TestMethod]
    public void Evaluate_SuccessiveSubtract_ReturnsExpected()
    {
        Formula f = new Formula("7 - 3 - 1 - 0");
        Assert.AreEqual((double)3, f.Evaluate(s => 0));
    }

    [TestMethod]
    public void Evaluate_SuccessiveMultiply_ReturnsExpected()
    {
        Formula f = new Formula("((7 * 3) * 2) * (5 + 2)");
        Assert.AreEqual((double)294, f.Evaluate(s => 0));
    }

    [TestMethod]
    public void Evaluate_DivideZeroNumerator_ReturnsExpected()
    {
        Formula f = new Formula("(2 - 2) / 4");
        Assert.AreEqual((double)0, f.Evaluate(s => 0));
    }

    [TestMethod]
    public void Evaluate_DivideZeroDenominator_ReturnsFormulaError()
    {
        Formula f = new Formula("(2 + 2) / 0");
        object evaluate = f.Evaluate(TestLookup);
        Assert.IsInstanceOfType(evaluate, typeof(FormulaError));
    }

    [TestMethod]
    public void Evaluate_DivideZeroDenominatorDoubles_ReturnsError()
    {
        Formula f = new Formula("(2 - 2) / (3.3 - 2.2 - 1.1)");
    }

    [TestMethod]
    public void Evaluate_VariableDivideByZero_ReturnsError()
    {
        Formula f = new Formula("2.5 + 3.1 / N9");
        Object evaluate = f.Evaluate(TestLookup);
        Assert.IsInstanceOfType(evaluate, typeof(FormulaError));

    }

    [TestMethod]
    public void Evaluate_VariableDivideByZeroExpression_ReturnsError()
    {
        Formula f = new("(5 + X1) / (X1 - 3)");
        var result = f.Evaluate(s => 3);
        Assert.IsInstanceOfType(result, typeof(FormulaError));

    }


    [TestMethod]
    public void Evaluate_VariableDivideNotByZero_ReturnsNormal()
    {
        Formula f = new Formula("10 / B2");
        Assert.AreEqual(2, (double)f.Evaluate(TestLookup));

    }

    // --- Equals Method Tests ---

    [TestMethod]
    public void Equals_ExpressionNull_ReturnFalse()
    {
        Formula b = new Formula("1+1");

        bool equal = b.Equals(null);

        Assert.IsFalse(equal);
    }


    [TestMethod]
    public void Equals_NonFormula_ReturnFalse()
    {
        Formula b = new Formula("1+1");
        List<string> s = new List<string>();

        bool equal = b.Equals(s);

        Assert.IsFalse(equal);
    }

    [TestMethod]
    public void Equals_Expression_ReturnTrue()
    {
        Formula a = new Formula("1+1");
        Formula b = new Formula("1+1");

        bool equal = a.Equals(b);

        Assert.IsTrue(equal);
    }

    [TestMethod]
    public void Equals_ExpressionSpaced_ReturnTrue()
    {
        Formula a = new Formula("1+1");
        Formula b = new Formula(" 1 + 1 ");

        bool equal = a.Equals(b);

        Assert.IsTrue(equal);
    }

    [TestMethod]
    public void Equals_ExpressionNormalized_ReturnTrue()
    {
        Formula a = new Formula("a1 - 4");
        Formula b = new Formula("A1 - 4 ");

        bool equal = a.Equals(b);

        Assert.IsTrue(equal);
    }

    [TestMethod]
    public void Equals_Expression_ReturnFalse()
    {
        Formula a = new Formula("1+1");
        Formula b = new Formula("3-1");

        bool equal = a.Equals(b);

        Assert.IsFalse(equal);
    }

    [TestMethod]
    public void Equals_ExpressionVariable_ReturnFalse()
    {
        Formula a = new Formula("A1 + 3");
        Formula b = new Formula("1 + 3");

        bool equal = a.Equals(b);

        Assert.IsFalse(equal);
    }

    [TestMethod]
    public void Equals_ComplexExpression_ReturnTrue()
    {
        Formula a = new Formula("(A1 - 4) / 2");
        Formula b = new Formula("(a1 -4) / 2 ");

        bool equal = a.Equals(b);

        Assert.IsTrue(equal);
    }

    [TestMethod]
    public void Equals_ComplexExpression_ReturnFalse()
    {
        Formula a = new Formula("(A1 - 4) / 2");
        Formula b = new Formula("A1 -4 / 2 ");

        bool equal = a.Equals(b);

        Assert.IsFalse(equal);
    }

    // --- Tests for EqualsOperator Overloading ---

    [TestMethod]
    public void EqualsOperation_Expression_ReturnTrue()
    {
        Formula a = new Formula("2 + 3");
        Formula b = new Formula("2 + 3");

        Assert.IsTrue(a == b);
    }

    [TestMethod]
    public void EqualsOperation_Expression_ReturnFalse()
    {
        Formula a = new Formula("2 + 3");
        Formula b = new Formula("5");

        Assert.IsFalse(a == b);
    }

    // --- Test for NotEquals Operator overloading ---

    [TestMethod]
    public void NotEqualsOperation_Expression_ReturnFalse()
    {
        Formula a = new Formula("2 + 3");
        Formula b = new Formula("2 + 3");

        Assert.IsFalse(a != b);
    }

    [TestMethod]
    public void NotEqualsOperation_Expression_ReturnTrue()
    {
        Formula a = new Formula("2 + 3");
        Formula b = new Formula("5");

        Assert.IsTrue(a != b);
    }

    // --- Tests for GetHashCode ---

    [TestMethod]
    public void GetHashCode_Expression_Equal()
    {
        Formula a = new Formula("1 + 1");
        int aHash = a.GetHashCode();

        Formula b = new Formula("1+1");
        int bHash = b.GetHashCode();

        Assert.AreEqual(aHash, bHash);
    }

    [TestMethod]
    public void GetHashCode_Expression_NotEqual()
    {
        Formula a = new Formula("B3 - 7");
        int aHash = a.GetHashCode();

        Formula b = new Formula("44 + 2");
        int bHash = b.GetHashCode();

        Assert.AreNotEqual(aHash, bHash);
    }

    [TestMethod]
    public void GetHashCode_NormalizeExpression_Equal()
    {
        Formula a = new Formula("A1+ 2");
        int aHash = a.GetHashCode();

        Formula b = new Formula("a1+2");
        int bHash = b.GetHashCode();

        Assert.AreEqual(aHash, bHash);
    }
}
