/// Skeleton implementation written by Joe Zachary for CS 3500, September 2013
/// Version 1.1 - Joe Zachary
/// (Fixed error in comment for RemoveDependency)
/// Version 1.2 - Daniel Kopta Fall 2018
/// (Clarified meaning of dependent and dependee)
/// (Clarified names in solution/project structure)
/// Version 1.3 - H. James de St. Germain Fall 2024
///<summary>
///   <para>
///     This code fills in the provided skeleton implementation. 
///     to represent dependency relationships between variables.
///     Eventually to be used in the spreadsheet implementation. 
///   </para>
/// <authors> Hudson Dalby </authors>
/// <date> 9/13/2024 </date>
/// </summary>

namespace CS3500.DependencyGraph;

/// <summary>
/// <para>
/// (s1,t1) is an ordered pair of strings, meaning t1 depends on s1.
/// (in other words: s1 must be evaluated before t1.)
/// </para>
/// <para>
/// A DependencyGraph can be modeled as a set of ordered pairs of strings.
/// Two ordered pairs (s1,t1) and (s2,t2) are considered equal if and only
/// if s1 equals s2 and t1 equals t2.
/// </para>
/// <remarks>
/// Recall that sets never contain duplicates.
/// If an attempt is made to add an element to a set, and the element is already
/// in the set, the set remains unchanged.
/// </remarks>
/// <para>
/// Given a DependencyGraph DG:
/// </para>
/// <list type="number">
/// <item>
/// If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
/// (The set of things that depend on s.)
/// </item>
/// <item>
/// If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
/// (The set of things that s depends on.)
/// </item>
/// </list>
/// <para>
/// For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}.
/// </para>
/// <code>
/// dependents("a") = {"b", "c"}
/// dependents("b") = {"d"}
/// dependents("c") = {}
/// dependents("d") = {"d"}
/// dependees("a") = {}
/// dependees("b") = {"a"}
/// dependees("c") = {"a"}
/// dependees("d") = {"b", "d"}
/// </code>
/// </summary>
public class DependencyGraph
{
    /// <summary>
    /// A dictionary to store dependees as keys and dependents as values
    /// </summary>
    private Dictionary<string, HashSet<string>> dependents = new Dictionary<string, HashSet<string>>();

    /// <summary>
    /// A dictionary to store dependants as keys and dependees as values
    /// </summary>
    private Dictionary<string, HashSet<string>> backing = new Dictionary<string, HashSet<string>>();

    /// <summary>
    /// Size of Dependency Graph (Number of ordered pairs)
    /// </summary>
    private int size;

    /// <summary>
    /// Initializes a new instance of the <see cref="DependencyGraph"/> class.
    /// The initial DependencyGraph is empty.
    /// </summary>
    public DependencyGraph()
    {
    }
    /// <summary>
    /// The number of ordered pairs in the DependencyGraph.
    /// </summary>
    public int Size
    {
        get { return size; }
    }
    /// <summary>
    /// Reports whether the given node has dependents (i.e., other nodes depend on it).
    /// </summary>
    /// <param name="nodeName"> The name of the node.</param>
    /// <returns> true if the node has dependents. </returns>
    public bool HasDependents(string nodeName)
    {
        // If nodeName is a key in the dependents dictionary, it must have dependents.
        if (dependents.ContainsKey(nodeName))
            return true;

        else return false;
    }
    /// <summary>
    /// Reports whether the given node has dependees (i.e., depends on one or more other nodes).
    /// </summary>
    /// <returns> true if the node has dependees.</returns>
    /// <param name="nodeName">The name of the node.</param>
    public bool HasDependees(string nodeName)
    {
        // If nodeName is a key in the backing dictionary, it must have dependees
        if (backing.ContainsKey(nodeName))
            return true;

        else return false;
    }
    /// <summary>
    /// <para>
    /// Returns the dependents of the node with the given name.
    /// </para>
    /// </summary>
    /// <param name="nodeName"> The node we are looking at.</param>
    /// <returns> The dependents of nodeName. </returns>
    public IEnumerable<string> GetDependents(string nodeName)
    {
        // Returns list of dependents if the nodeName exists in the Dependency Graph. 
        HashSet<string>? currentSet;
        if (dependents.TryGetValue(nodeName, out currentSet))
        {
            return currentSet;
        }

        // Returns an empty list if the nodeName does not exist
        else return currentSet = new HashSet<string>();
    }
    /// <summary>
    /// <para>
    /// Returns the dependees of the node with the given name.
    /// </para>
    /// </summary>
    /// <param name="nodeName"> The node we are looking at.</param>
    /// <returns> The dependees of nodeName. </returns>
    public IEnumerable<string> GetDependees(string nodeName)
    {
        // Returns list of dependees if the nodeName exists in the Dependency Graph. 
        HashSet<string>? currentSet;
        if (backing.TryGetValue(nodeName, out currentSet))
        {
            return currentSet;
        }

        // Returns an empty list if the nodeName does not exist
        else return currentSet = new HashSet<string>();
    }
    /// <summary>
    /// <para>Adds the ordered pair (dependee, dependent), if it doesn't exist.</para>
    ///
    /// <para>
    /// This can be thought of as: dependee must be evaluated before dependent
    /// </para>
    /// </summary>
    /// <param name="dependee"> the name of the node that must be evaluated first</param>
    /// <param name="dependent"> the name of the node that cannot be evaluated until after dependee</param>
    public void AddDependency(string dependee, string dependent)
    {
        // If the dependee doesnt' exist in the dictionary yet, creates a dependent list for it
        if (!dependents.ContainsKey(dependee))
            dependents.Add(dependee, new HashSet<string>());

        // If the dependent doesn't exist in backing dictionary yet, creates a dependees list for it
        if (!backing.ContainsKey(dependent))
            backing.Add(dependent, new HashSet<string>());

        HashSet<string>? currentList;
        // Looks up the current dependee as dictionary key, fetches its value list 
        if (dependents.TryGetValue(dependee, out currentList))
        {
            // Checks to see if ordered pair already exists in the set
            if (!currentList.Contains(dependent))
            {

                // Adds the current dependent to the list of dependents for that node
                currentList.Add(dependent);

                // Sets the newly updated list as the value for the dependent 
                dependents[dependee] = currentList;

                // Increments the size variable of the Dependency List
                size++;
            }
        }

        // Repeats the same process for the backing dictionary
        HashSet<string>? currentSetBacking;
        // Looks up the current dependent as the backing dictionary key, fetches the value list
        if (backing.TryGetValue(dependent, out currentSetBacking))
        {
            //Ensures duplicate ordered pairs do not get added to the backing list. 
            if (!currentSetBacking.Contains(dependee))
            {
                // Adds the current dependee to the list of dependees for that node
                currentSetBacking.Add(dependee);

                // Sets the newly updated list as the value for the dependent
                backing[dependent] = currentSetBacking;
            }
        }
    }
    /// <summary>
    /// <para>
    /// Removes the ordered pair (dependee, dependent), if it exists.
    /// </para>
    /// </summary>
    /// <param name="dependee"> The name of the node that must be evaluated first</param>
    /// <param name="dependent"> The name of the node that cannot be evaluated until after dependee</param>
    public void RemoveDependency(string dependee, string dependent)
    {
        // Looks up the dependee node to be examined if it exists. 
        HashSet<string>? currentSet;
        if (dependents.TryGetValue(dependee, out currentSet))
        {
            // If the value list contains the dependent, remove the ordered pair from the dictionary
            if (currentSet.Contains(dependent))
            {

                currentSet.Remove(dependent);

                // Sets the newly updated list as the value for the dependent 
                dependents[dependee] = currentSet;

                // Decreases the count variable if an ordered pair was removed
                size--;

                // Removes the dependee from the key set if it has no values left
                if (currentSet.Count == 0)
                {
                    dependents.Remove(dependee);
                }
            }
        }

        // Repeats the same process for the backing list

        // Looks up the dependent node to be examined if it exists. 
        HashSet<string>? currentSetBacking;
        if (backing.TryGetValue(dependent, out currentSetBacking))
        {
            // if the value list contains the dependee, remove the ordered pair from the backing dictionary
            if (currentSetBacking.Contains(dependee))
            {
                // Removes the ordered pair from the backing list
                currentSetBacking.Remove(dependee);

                // Updates the backing list 
                backing[dependent] = currentSetBacking;

                // Removes the dependent from the key list if there is no longer values for it
                if (currentSetBacking.Count == 0)
                {
                    backing.Remove(dependent);
                }
            }
        }
    }
    /// <summary>
    /// Removes all existing ordered pairs of the form (nodeName, *). Then, for each
    /// t in newDependents, adds the ordered pair (nodeName, t).
    /// </summary>
    /// <param name="nodeName"> The name of the node who's dependents are being replaced</param>
    /// <param name="newDependents"> The new dependents for nodeName</param>
    public void ReplaceDependents(string nodeName, IEnumerable<string> newDependents)
    {
        // Sees if the current key exists in the Dependency dictionary
        HashSet<string>? currentSet;
        if (dependents.TryGetValue(nodeName, out currentSet))
        {
            //Alters the backing list and removes any ordered pairs found for each dependent
            foreach (string s in currentSet)
            {
                // Removes the ordered pair if it exists from the Dependency Graph
                RemoveDependency(nodeName, s);
            }
        }

        // For each string in newDependents parameter, adds a new ordered pair with the nodeName as dependee. 
        foreach (string s in newDependents)
        {
            // Adds the ordered pair to the Dependency Graph
            AddDependency(nodeName, s);
        }
    }
    /// <summary>
    /// <para>
    /// Removes all existing ordered pairs of the form (*, nodeName). Then, for each
    /// t in newDependees, adds the ordered pair (t, nodeName).
    /// </para>
    /// </summary>
    /// <param name="nodeName"> The name of the node who's dependees are being replaced</param>
    /// <param name="newDependees"> The new dependees for nodeName</param>
    public void ReplaceDependees(string nodeName, IEnumerable<string> newDependees)
    {
        // Sees if the current key exists in the Dependency dictionary
        HashSet<string>? currentSet;
        if (backing.TryGetValue(nodeName, out currentSet))
        {
            //Alters the backing list and removes any ordered pairs found for each dependent
            foreach (string s in currentSet)
            {
                // Removes the ordered pair if it exists from the Dependency Graph
                RemoveDependency(s, nodeName);
            }
        }

        // Adds each dependency in the newDependees parameter to the Dependency Graph
        foreach (string s in newDependees)
        {
            // Adds the new ordered pair to the Dependency Graph
            AddDependency(s, nodeName);
        }
    }
}
