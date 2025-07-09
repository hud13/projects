#ifndef TRIE_H
#define TRIE_H
/*
Assignment 4-
A trie class that is represented with an array of 26 pointers for letters a-z
Once words are added, can search to determine whether trie contains valid word.

Author: Hudson Dalby
Modified: 2/5/25
*/

#include <iostream>
#include <string>
#include <vector>

class Trie
{
private:
    // Array of lowercase char nodes from a-z
    Trie *charNodes[26];
    // Boolean flag that is true if trie ending at node represents word
    bool wordFlag;

public:
    /**
     * Default constructor
     * Creates a new Trie that contains no words.
     */
    Trie();

    /**
     * Destructor
     * Deletes all nodes allocated by the Trie.
     */
    ~Trie();

    /**
     * Copy Constructor
     * Creates a new trie object with an existing trie
     * @param other - the trie with data to construct a copy of
     */
    Trie(const Trie &other);

    /**
     * Assignment Operator
     * Assigns a trie object to an existing trie. Subsequent modifications to either tree should not modify the other.
     * @param other - trie to be assigned
     */
    Trie &operator=(Trie other);

    /**
     * Adds a word to the trie.
     * Assumes all words only contain lowercase characters a-z.
     * @param word - word to be added to the trie
     */
    void addWord(std::string word);

    /**
     * Method to determine if given word is contained in the trie
     * @param word - word to be searched for in trie
     * @returns True if word was found in trie, false if not.
     */
    bool isWord(std::string word);

    /**
     * Method that gets all words in the trie that start with a given prefix
     * @param word - word to be used as prefix in word list
     * @returns a list of words that are included in the trie with the prefix
     */
    std::vector<std::string> allWordsStartingWithPrefix(std::string word);

    /**
     * Helper method for above method that get a vector that contains all trie words.
     * Searches all branches of the current node, and each branch adds to the same vector reference.
     * @param node - The pointer to the node to be examined
     * @param currentWord - The current word the node represents
     * @param wordList - A reference to the vector that stores the list of words to be returned.
     */
    void getAllWords(Trie *node, std::string currentWord, std::vector<std::string> &wordList);
};

#endif // Include guard for TRIE_H