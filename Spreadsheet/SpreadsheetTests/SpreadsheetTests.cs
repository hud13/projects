/// <summary>
/// These Spreadsheet tests are designed for the implementation of the spreadsheet structure in PS5. 
/// The functions tested are focused on the explicit contents of a cell, and functions such 
/// as finding a cell's direct dependents and preventing cycles. 
/// <authors> Hudson Dalby </authors>
/// <date> 9/27/2024 </date>
/// </summary>

using CS3500.Formula;
using CS3500.Spreadsheet;

namespace SpreadsheetTests
{
    [TestClass]
    public class SpreadsheetTests
    {
        [TestMethod]
        public void GetNamesOfAllNonEmptyCells_TestSheet_Returns()
        {
            Spreadsheet s = new Spreadsheet();

            s.SetContentsOfCell("a1", "3");
            s.SetContentsOfCell("b1", "=a1 + 5");
            s.SetContentsOfCell("c1", "=b1 + 2");
            s.SetContentsOfCell("d1", "=c1 + 8");
            var output = s.SetContentsOfCell("C1", "test");

            HashSet<string> expected = new();
            expected.Add("C1");
            expected.Add("D1");
            expected.Add("A1");
            expected.Add("B1");

            Boolean same = expected.SetEquals(s.GetNamesOfAllNonemptyCells());

            Assert.IsTrue(same);

        }

        /// --- Tests for SetContents of Cell ---
        /// (Many borrowed from PS5 tests)

        [TestMethod]
        public void SetContentsofCell_FormulaComplex_Returns()
        {
            Spreadsheet s = new();
            s.SetContentsOfCell("a1", "3");
            s.SetContentsOfCell("b1", "=a1 + 5");
            s.SetContentsOfCell("c1", "=b1 + 2");
            s.SetContentsOfCell("d1", "=c1 + a1");
            var output = s.SetContentsOfCell("b1", "0");

            HashSet<string> expected = new HashSet<string>();
            expected.Add("B1");
            expected.Add("C1");
            expected.Add("D1");

            Boolean equal = output.SequenceEqual(expected);
            Assert.IsTrue(equal);
        }



        [TestMethod]
        public void SetContentsOfCell_DoubleComplex_Returns()
        {
            Spreadsheet s = new();
            s.SetContentsOfCell("a1", "3");
            s.SetContentsOfCell("b1", "=a1 + 5");
            s.SetContentsOfCell("c1", "=b1 + 2");
            s.SetContentsOfCell("d1", "=c1 + a1");
            var output = s.SetContentsOfCell("a1", "4");

            List<string> expected = new List<string>();
            expected.Add("A1");
            expected.Add("B1");
            expected.Add("C1");
            expected.Add("D1");

            Boolean equal = output.SequenceEqual(expected);
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void SetContentsOfCell_StringComplex_Returns()
        {
            Spreadsheet s = new();

            s.SetContentsOfCell("a1", "3");
            s.SetContentsOfCell("b1", "=a1 + 5");
            s.SetContentsOfCell("c1", "=b1 + 2");
            s.SetContentsOfCell("d1", "=c1 + 8");
            var output = s.SetContentsOfCell("C1", "test");

            List<string> expected = new List<string>();
            expected.Add("C1");
            expected.Add("D1");


            Boolean equal = output.SequenceEqual(expected);
            Assert.IsTrue(equal);
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void SetContentsOfCell_FormulaIndirectCycle_Throws()
        {
            Spreadsheet s = new();
            s.SetContentsOfCell("a1", "4");
            s.SetContentsOfCell("b1", "=c1 + 5");
            s.SetContentsOfCell("c1", "=e1 + 2");
            s.SetContentsOfCell("d1", "=c1 + a1");
            s.SetContentsOfCell("e1", "=b1 + 2");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetContentsOfCell_InvalidName_Throws()
        {
            Spreadsheet s = new();
            s.SetContentsOfCell("", "5");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void SetContentsOfCell_InvalidFormula_Throws()
        {
            Spreadsheet s = new();
            s.SetContentsOfCell("A1", "=9+1");
            s.SetContentsOfCell("A1", "=75--a2");

        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void SetContentsOfCell_CircularException_DoesNotChange()
        {
            Spreadsheet s = new();
            s.SetContentsOfCell("A1", "=9+1");
            s.SetContentsOfCell("A1", "=A1 + 4");
        }


        /// --- Tests for Changed() Method ---

        [TestMethod]
        public void CreateSpreadsheet_DefaultConstructor_NotChanged()
        {
            Spreadsheet s = new();

            Assert.IsFalse(s.Changed);
        }

        [TestMethod]
        public void CreateSpreadsheet_DefaultConstructor_Changed()
        {
            Spreadsheet s = new();

            s.SetContentsOfCell("A1", "4");

            Assert.IsTrue(s.Changed);
        }

        [TestMethod]
        public void CreateSpreadsheet_FileConstructor_NotChanged()
        {
            Spreadsheet s = new();
            s.SetContentsOfCell("A1", "7");

            s.Save("test.txt");

            Spreadsheet load = new Spreadsheet("test.txt");

            Assert.IsFalse(load.Changed);
        }

        [TestMethod]
        public void CreateSpreadsheet_FileConstructor_Changed()
        {
            Spreadsheet s = new();
            s.SetContentsOfCell("A1", "7");

            s.Save("test.txt");

            Spreadsheet load = new Spreadsheet("test.txt");

            load.SetContentsOfCell("B1", "change");

            Assert.IsTrue(load.Changed);
        }

        [TestMethod]
        public void Save_StateAfterSaving_NotChanged()
        {
            Spreadsheet s = new();
            s.SetContentsOfCell("A1", "7");

            s.Save("test.txt");

            Assert.IsFalse(s.Changed);
        }

        [TestMethod]
        public void Save_StateAfterSaving_Changed()
        {
            Spreadsheet s = new();
            s.SetContentsOfCell("A1", "7");

            s.Save("test.txt");

            s.SetContentsOfCell("B1", "7");

            Assert.IsTrue(s.Changed);
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void Save_InvalidCellChange_NotChanged()
        {
            Spreadsheet s = new();
            s.SetContentsOfCell("A1", "7");

            s.Save("test.txt");

            s.SetContentsOfCell("B1", "=B1");
        }


        /// --- Tests for getting value of cell ---

        [TestMethod]
        public void GetCellValue_Double_Return()
        {
            Spreadsheet s = new();

            s.SetContentsOfCell("A1", "4");

            Assert.AreEqual(4.0, s.GetCellValue("A1"));
        }
        [TestMethod]
        public void GetCellValue_IndexerDouble_Return()
        {
            Spreadsheet s = new();

            s.SetContentsOfCell("A1", "4");

            Assert.AreEqual(4.0, s["A1"]);
        }

        [TestMethod]
        public void GetCellValue_String_Return()
        {
            Spreadsheet s = new();

            s.SetContentsOfCell("A1", "hello");

            Assert.AreEqual("hello", s.GetCellValue("A1"));
        }

        [TestMethod]
        public void GetCellValue_IndexerString_Return()
        {
            Spreadsheet s = new();

            s.SetContentsOfCell("A1", "hello");

            Assert.AreEqual("hello", s["A1"]);
        }

        [TestMethod]
        public void GetCellValue_Formula_Return()
        {
            Spreadsheet s = new();

            s.SetContentsOfCell("A1", "=4+4");

            Assert.AreEqual(8.0, s.GetCellValue("A1"));
        }

        [TestMethod]
        public void GetCellValue_IndexerFormula_Return()
        {
            Spreadsheet s = new();

            s.SetContentsOfCell("A1", "=4+4");

            Assert.AreEqual(8.0, s["A1"]);
        }

        [TestMethod]
        public void GetCellValue_IndexerInvalid_Throws()
        {
            Spreadsheet s = new();

            s.SetContentsOfCell("A1", "=4+4");

            Assert.ThrowsException<InvalidNameException>(() => s["Invalid"]);
        }

        [TestMethod]
        public void GetCellValue_IndexerEmpty_Returns()
        {
            Spreadsheet s = new();

            s.SetContentsOfCell("A1", "=4+4");

            Assert.AreEqual(String.Empty, s["Z1"]);
        }

        [TestMethod]
        public void GetCellValue_FormulaLookup_Return()
        {
            Spreadsheet s = new();

            s.SetContentsOfCell("A1", "=4+4");
            s.SetContentsOfCell("B1", "=A1-3");

            Assert.AreEqual(5.0, s.GetCellValue("B1"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellValue_InvalidName_Throws()
        {
            Spreadsheet s = new();

            s.SetContentsOfCell("A1", "4");

            s.GetCellContents("wrong");
        }

        [TestMethod]
        public void GetCellValue_NullName_Throws()
        {
            Spreadsheet s = new();

            s.SetContentsOfCell("A1", "4");

            var empty = s.GetCellContents("b3");

            Assert.AreEqual(String.Empty, empty);
        }

        [TestMethod]
        public void GetCellValue_FormulaInvalid_Return()
        {
            Spreadsheet s = new();

            s.SetContentsOfCell("A1", "=4+4");
            s.SetContentsOfCell("B1", "=C1+4");

            Assert.IsTrue(s.GetCellValue("B1") is FormulaError);

        }
        [TestMethod]
        public void GetContentsOfCell_FormulaDependents_Returns()
        {
            Spreadsheet s = new();
            s.SetContentsOfCell("a1", "4");
            s.SetContentsOfCell("b1", "14");
            s.SetContentsOfCell("c1", "=a1 + 2");
            s.SetContentsOfCell("d1", "=c1 + a1");

            Assert.AreEqual(10.0, s.GetCellValue("d1"));
        }

        [TestMethod]
        public void GetContentsOfCell_ChangedDependency_Returns()
        {
            Spreadsheet s = new();
            s.SetContentsOfCell("a1", "4");
            s.SetContentsOfCell("b1", "14");
            s.SetContentsOfCell("c1", "=a1 + 2");
            s.SetContentsOfCell("a1", "=b1 + 3");

            Assert.AreEqual(19.0, s.GetCellValue("c1"));
        }

        [TestMethod]
        public void GetContentsOfCell_ChangedDependencyMultiple_Returns()
        {
            Spreadsheet s = new();
            s.SetContentsOfCell("a1", "4");
            s.SetContentsOfCell("b1", "14");
            s.SetContentsOfCell("c1", "=a1 + 2");
            s.SetContentsOfCell("a1", "=B1 + 3");

            Assert.AreEqual(19.0, s.GetCellValue("c1"));
        }

        /// --- Tests for Creating and Saving Spreadsheet from file ---

        [TestMethod, ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Save_PathDoesNotExist_Throws()
        {
            Spreadsheet s = new Spreadsheet();

            s.SetContentsOfCell("A1", "10");

            s.Save("/missing/save.json");
        }

        [TestMethod, ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Spreadsheet_FileDoesNotExist_Throws()
        {
            Spreadsheet s = new Spreadsheet("INVALID");
        }

        [TestMethod, ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Spreadsheet_FileMalformed_Throws()
        {

            File.WriteAllText("bad.txt", "\r\n  \"Cells\": {\r\n    \"A1\": {\r\n      \"StringForm\": \"75\"\r\n    },\r\n    \"B1\": {\r\n      \"StringForm\": \"=A1*2\"\r\n    },\r\n    \"C1\": {\r\n      \"StringForm\": \"=A1+4\"\r\n    }\r\n  }\r\n}");

            Spreadsheet s = new Spreadsheet("bad.txt");
        }

        [TestMethod, ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Spreadsheet_FileMissingCells_Throws()
        {

            File.WriteAllText("bad.txt", "\r\n  \"\": {\r\n    \"A1\": {\r\n      \"StringForm\": \"75\"\r\n    },\r\n    \"B1\": {\r\n      \"StringForm\": \"=A1*2\"\r\n    },\r\n    \"C1\": {\r\n      \"StringForm\": \"=A1+4\"\r\n    }\r\n  }\r\n}");

            Spreadsheet s = new Spreadsheet("bad.txt");
        }

        [TestMethod, ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Spreadsheet_null_Throws()
        {

            string? json = null;

            Spreadsheet s = new Spreadsheet(json);
        }

        [TestMethod]
        public void SpreadsheetDefault_SaveAndLoad_Success()
        {
            Spreadsheet s = new();

            s.SetContentsOfCell("A1", "75");
            s.SetContentsOfCell("B1", "=A1 *2");
            s.SetContentsOfCell("C1", "=A1 + 4");

            s.Save("file2.txt");

            Spreadsheet test = new Spreadsheet("file2.txt");

            Assert.AreEqual(s.GetCellContents("A1"), test.GetCellContents("A1"));

        }

    }
}