/*
A test class that exercises the trie class.
Takes two text files as arguments, the first is a file of words, and second is queries.

Uses code from cplusplus.com to help read text files for testing purposes.

Author: Hudson Dalby
Modified: 2/5/25
*/

#include <iostream>
#include <fstream>
#include <string>
#include "trie.h"

using std::string;
using std::vector;

/*
Main function that executes following tasks:

Task One:
Takes two text file command line arguments: words to add, and queries to perform.
After adding words, tests each query and outputs test results in the following format:
Checking xxxxx:
Word found/Word not found
(List of words with query as prefix, blank lines indicate no words exist in trie.)

Task Two:
Tests the Rule of Three for the Trie class.
(Destructor, Copy Constructor, Assignment Operator)
*/
int main(int argc, char *argv[])
{
    // Task One:

    if (argc != 3)
    {
        std::cout << "Requires 2 text files as arguments: Words and Queries" << std::endl;
        return 0;
    }

    Trie testTrie;
    string line;
    std::ifstream wordFile(argv[1]);
    std::ifstream queriesFile(argv[2]);

    // Adds all words from the wordFile argument to a test trie
    if (wordFile.is_open())
    {
        while (getline(wordFile, line))
        {
            testTrie.addWord(line);
        }
        wordFile.close();
    }

    else
        std::cout << "Unable to open word file";

    // Checks if each word in the queries file is in the trie
    // Prints word being checked, if it's included in the trie, and list of words with the queried word as a prefix
    if (queriesFile.is_open())
    {
        while (getline(queriesFile, line))
        {
            // Prints word being checked
            std::cout << "Checking " << line << ":" << std::endl;

            // Prints whether word is found in trie or not
            bool trieWord = testTrie.isWord(line);
            if (trieWord)
            {
                std::cout << "Word found" << std::endl;
            }
            else
            {
                std::cout << "Word not found" << std::endl;
            }

            // Prints list of words with the queried word as a prefix
            vector<string> wordList = testTrie.allWordsStartingWithPrefix(line);
            for (string s : wordList)
            {
                std::cout << s << " ";
            }

            // Separates each test for cleaner reading.
            std::cout << "\n";
        }
        wordFile.close();
    }
    else
        std::cout << "Unable to open queries file";

    // Task Two:

    /*
    Destructor test:
    Adds words "egg" and "chicken" to a trie, calls the deconstructor, adds word "dog" to tree
    Checks that trie no longer contains old words, and contains the new word

    Expected contents:
    firstTrie (initial): egg chicken
    firstTrie (reconstructed): dog
    */

    Trie firstTrie;
    firstTrie.addWord("egg");
    firstTrie.addWord("chicken");

    firstTrie.~Trie();
    firstTrie.addWord("dog");

    bool eggCheck = firstTrie.isWord("egg");
    bool chickenCheck = firstTrie.isWord("chicken");
    bool dogCheck = firstTrie.isWord("dog");

    if (eggCheck == true || chickenCheck == true || dogCheck == false)
    {
        return 1;
    }

    /*
    Copy Constructor test:
    Creates two additional tries with the first trie as a parameter.
    Adds a word to the second tree and checks that others were not modified.

    Expected contents:
    firstTrie: dog
    secondTrie: dog cat
    thirdTrie: dog
    */

    Trie secondTrie(firstTrie);
    Trie thirdTrie(firstTrie);

    secondTrie.addWord("cat");

    if (firstTrie.isWord("cat") || thirdTrie.isWord("cat"))
    {
        return 1;
    }

    /*
    Assignment Operator test:
    assigns the third Trie to the second Trie. Makes sure it contains same added word as second Trie.
    Next adds new word to third Trie, checks that no other tries were modified.

    Expected contents:
    firstTrie: dog
    secondTrie: dog cat
    thirdTrie: dog cat bird
    */

    thirdTrie = secondTrie;

    if (!thirdTrie.isWord("cat"))
    {
        return 1;
    }

    thirdTrie.addWord("bird");

    if (firstTrie.isWord("bird") || secondTrie.isWord("bird"))
    {
        return 1;
    }

    // Tests are all functional
    return 0;
}
