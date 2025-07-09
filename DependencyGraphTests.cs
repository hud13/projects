/// <summary>
/// This is a test class for DependencyGraphTest and is intended
/// to contain all DependencyGraphTest Unit Tests for PS3 Testing 
/// <authors> Hudson Dalby </authors>
/// <date> 9/13/2024 </date>
/// </summary>

using CS3500.DependencyGraph;

namespace DependencyGraphTests;

[TestClass]
public class DependencyGraphTests
{
    // Basic tests for the variable + Add/Remove Methods

    [TestMethod]
    public void DependencyGraph_DuplicateAdd_NoChange()
    {
        DependencyGraph test = new DependencyGraph();
        test.AddDependency("a1", "b1");
        test.AddDependency("a1", "b1");
        test.AddDependency("a1", "b1");

        Assert.AreEqual(1, test.Size);
    }

    [TestMethod]
    public void DependencyGraph_SizeAdding_Success()
    {
        DependencyGraph test = new DependencyGraph();
        test.AddDependency("a1", "b1");
        test.AddDependency("a2", "b2");
        test.AddDependency("a3", "b3");

        Assert.AreEqual(3, test.Size);
    }

    [TestMethod]
    public void DependencyGraph_SizeRemoving_Success()
    {
        DependencyGraph test = new DependencyGraph();
        test.AddDependency("a1", "b1");
        test.AddDependency("a2", "b2");
        test.AddDependency("a3", "b3");

        test.RemoveDependency("a1", "b1");

        Assert.AreEqual(2, test.Size);
    }

    [TestMethod]
    public void DependencyGraph_SizeReplacing_Success()
    {
        DependencyGraph test = new DependencyGraph();
        test.AddDependency("a1", "b1");
        test.AddDependency("a2", "b2");
        test.AddDependency("a3", "b3");

        HashSet<string> newDependents = new HashSet<string>();
        newDependents.Add("b2");
        newDependents.Add("b3");
        newDependents.Add("b4");

        test.ReplaceDependents("a1", newDependents);

        Assert.AreEqual(5, test.Size);
    }

    [TestMethod]
    public void DependencyGraph_SizeEmpty_Success()
    {
        DependencyGraph test = new DependencyGraph();

        Assert.AreEqual(0, test.Size);

        test.AddDependency("a1", "b1");
        test.AddDependency("a2", "b2");
        test.AddDependency("a3", "b3");

        test.RemoveDependency("a1", "b1");
        test.RemoveDependency("a2", "b2");
        test.RemoveDependency("a3", "b3");

        Assert.AreEqual(0, test.Size);
    }

    [TestMethod]
    public void DependencyGraph_DenseSize_Success()
    {
        DependencyGraph test = new DependencyGraph();

        Assert.AreEqual(0, test.Size);

        test.AddDependency("a1", "b1");
        test.AddDependency("a1", "v4");
        test.AddDependency("a1", "o9");
        test.AddDependency("a2", "b2");
        test.AddDependency("a2", "b3");

        Assert.AreEqual(5, test.Size);
    }


    // Has dependents and has dependees method tests 


    [TestMethod]
    public void DependencyGraph_HasDependents_ReturnsTrue()
    {
        DependencyGraph test = new DependencyGraph();

        test.AddDependency("a1", "b1");

        bool has = test.HasDependents("a1");

        Assert.IsTrue(has);
    }

    [TestMethod]
    public void DependencyGraph_HasDependents_ReturnsFalse()
    {
        DependencyGraph test = new DependencyGraph();

        test.AddDependency("a1", "b1");

        bool has = test.HasDependents("b1");

        Assert.IsFalse(has);
    }

    [TestMethod]
    public void DependencyGraph_EmptyHasDependents_ReturnsTrue()
    {
        DependencyGraph test = new DependencyGraph();

        test.AddDependency("a1", "b1");
        test.RemoveDependency("a1", "b1");

        bool has = test.HasDependents("a1");

        Assert.IsFalse(has);
    }

    [TestMethod]
    public void DependencyGraph_RemovedHasDependents_ReturnsTrue()
    {
        DependencyGraph test = new DependencyGraph();

        test.AddDependency("a1", "b1");
        test.AddDependency("a1", "b3");
        test.ReplaceDependents("a1", new HashSet<string>());

        bool has = test.HasDependents("a1");

        Assert.IsFalse(has);
    }


    [TestMethod]
    public void DependencyGraph_HasDependees_ReturnsTrue()
    {
        DependencyGraph test = new DependencyGraph();

        test.AddDependency("a1", "b1");

        bool has = test.HasDependees("b1");

        Assert.IsTrue(has);
    }

    [TestMethod]
    public void DependencyGraph_HasDependeesReturnsFalse()
    {
        DependencyGraph test = new DependencyGraph();

        test.AddDependency("a1", "b1");

        bool has = test.HasDependees("a1");

        Assert.IsFalse(has);
    }

    [TestMethod]
    public void DependencyGraph_EmptyHasDependees_ReturnsTrue()
    {
        DependencyGraph test = new DependencyGraph();

        test.AddDependency("a1", "b1");
        test.RemoveDependency("a1", "b1");

        bool has = test.HasDependees("b1");

        Assert.IsFalse(has);
    }

    // Tests for the getDependents and getDependees methods

    [TestMethod]
    public void DependencyGraph_GetDependees_ReturnsEmpty()
    {
        DependencyGraph test = new DependencyGraph();
        HashSet<string> empty = new HashSet<string>();

        HashSet<string> dependees = (HashSet<string>)test.GetDependees("a1");
        bool equal = dependees.SetEquals(empty);
    }

    [TestMethod]
    public void DependencyGraph_GetDependees_ReturnsList()
    {
        DependencyGraph test = new DependencyGraph();
        test.AddDependency("A5", "a1");
        test.AddDependency("B2", "a1");
        test.AddDependency("C3", "a1");

        HashSet<string> expected = new HashSet<string>();
        expected.Add("A5");
        expected.Add("B2");
        expected.Add("C3");

        HashSet<string> dependees = (HashSet<string>)test.GetDependees("a1");

        bool equal = dependees.SetEquals(expected);

        Assert.IsTrue(equal);
    }

    [TestMethod]
    public void DependencyGraph_GetDependeesAltered_ReturnsList()
    {
        DependencyGraph test = new DependencyGraph();
        test.AddDependency("A5", "a1");
        test.AddDependency("B2", "a1");
        test.AddDependency("C3", "a1");

        HashSet<string> expected = new HashSet<string>();
        expected.Add("B12");

        test.ReplaceDependees("a1", expected);

        HashSet<string> dependees = (HashSet<string>)test.GetDependees("a1");

        bool equal = dependees.SetEquals(expected);

        Assert.IsTrue(equal);
    }
    [TestMethod]
    public void DependencyGraph_GetDependents_ReturnsEmpty()
    {
        DependencyGraph test = new DependencyGraph();
        HashSet<string> empty = new HashSet<string>();

        HashSet<string> dependees = (HashSet<string>)test.GetDependents("a1");
        bool equal = dependees.SetEquals(empty);
    }

    [TestMethod]
    public void DependencyGraph_GetDependents_ReturnsList()
    {
        DependencyGraph test = new DependencyGraph();
        test.AddDependency("a1", "b3");
        test.AddDependency("a1", "a6");
        test.AddDependency("a1", "p8");

        HashSet<string> expected = new HashSet<string>();
        expected.Add("b3");
        expected.Add("a6");
        expected.Add("p8");

        HashSet<string> dependees = (HashSet<string>)test.GetDependents("a1");

        bool equal = dependees.SetEquals(expected);

        Assert.IsTrue(equal);
    }

    // ReplaceDependees Tests

    [TestMethod]
    public void DependencyGraph_ReplaceDependees_ReturnsList()
    {

        HashSet<string> expected = new HashSet<string>();
        expected.Add("d1");
        expected.Add("d2");
        expected.Add("d3");

        DependencyGraph test = new DependencyGraph();
        test.AddDependency("A5", "a1");
        test.AddDependency("B2", "a1");
        test.AddDependency("C3", "a1");

        test.ReplaceDependees("a1", expected);

        HashSet<string> dependees = (HashSet<string>)test.GetDependees("a1");

        bool equal = dependees.SetEquals(expected);

        Assert.IsTrue(equal);
    }

    [TestMethod]
    public void DependencyGraph_DuplicateReplaceDependees_ReturnsList()
    {

        HashSet<string> expected = new HashSet<string>();
        expected.Add("d1");
        expected.Add("B2");
        expected.Add("d3");

        DependencyGraph test = new DependencyGraph();
        test.AddDependency("A5", "a1");
        test.AddDependency("B2", "a1");
        test.AddDependency("C3", "a1");

        test.ReplaceDependees("a1", expected);

        HashSet<string> dependees = (HashSet<string>)test.GetDependees("a1");

        bool equal = dependees.SetEquals(expected);

        Assert.IsTrue(equal);
    }

    /// <summary>
    /// This code is a given example of a stress test for the Dependency Graph class. 
    /// It runs a large number of operations in a short amount of time to ensure each 
    /// implemented method is both functional and minimally time complex. 
    /// </summary>
    [TestMethod]
    [Timeout(2000)] // 2 second run time limit
    public void StressTest()
    {
        DependencyGraph dg = new();
        // Generates an array of 200 strings to be placed in the graph. 
        const int SIZE = 200;
        string[] letters = new string[SIZE];

        // For loop creates a unique string in each array position. 
        for (int i = 0; i < SIZE; i++)
        {
            letters[i] = string.Empty + ((char)('a' + i));
        }
        // Creates a correct reference set of dependents and dependees.
        HashSet<string>[] dependents = new HashSet<string>[SIZE];
        HashSet<string>[] dependees = new HashSet<string>[SIZE];

        // Removes each element in the dependents and dependees sets 
        for (int i = 0; i < SIZE; i++)
        {
            dependents[i] = [];
            dependees[i] = [];
        }
        // Add a bunch of dependency relationships to each set 
        // Does this for both the created dependency graph and reference sets
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 1; j < SIZE; j++)
            {
                dg.AddDependency(letters[i], letters[j]);
                dependents[i].Add(letters[j]);
                dependees[j].Add(letters[i]);
            }
        }
        // Remove a bunch of dependency relationships to each set
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 4; j < SIZE; j += 4)
            {
                dg.RemoveDependency(letters[i], letters[j]);
                dependents[i].Remove(letters[j]);
                dependees[j].Remove(letters[i]);
            }
        }
        // Add some back
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 1; j < SIZE; j += 2)
            {
                dg.AddDependency(letters[i], letters[j]);
                dependents[i].Add(letters[j]);
                dependees[j].Add(letters[i]);
            }
        }
        // Remove some more
        for (int i = 0; i < SIZE; i += 2)
        {
            for (int j = i + 3; j < SIZE; j += 3)
            {
                dg.RemoveDependency(letters[i], letters[j]);
                dependents[i].Remove(letters[j]);
                dependees[j].Remove(letters[i]);
            }
        }
        // Makes sure each element in the dependency graph equals what it should. 
        for (int i = 0; i < SIZE; i++)
        {
            Assert.IsTrue(dependents[i].SetEquals(new
            HashSet<string>(dg.GetDependents(letters[i]))));
            Assert.IsTrue(dependees[i].SetEquals(new
            HashSet<string>(dg.GetDependees(letters[i]))));
        }
    }
}