/*
Assignment 4-
A trie class that is represented with an array of 26 pointers for letters a-z
Once words are added, can search to determine whether trie contains a valid word.

Author: Hudson Dalby
Modified: 2/5/25
*/

#include "trie.h"
#include <string>
#include <vector>

using std::string;
using std::vector;

Trie::Trie()
{
    // Word flag set false by default
    wordFlag = false;

    // Sets each node to empty by default
    for (int i = 0; i < 26; i++)
    {
        charNodes[i] = nullptr;
    }
}

Trie::~Trie()
{
    // Recursively deletes each tree node.
    for (int i = 0; i < 26; i++)
    {
        delete charNodes[i];
        charNodes[i] = nullptr;
    }
}

Trie::Trie(const Trie &other)
{
    wordFlag = other.wordFlag;

    for (int i = 0; i < 26; i++)
    {
        charNodes[i] = nullptr;
        if (other.charNodes[i])
        {
            // Recursively copies the character nodes
            charNodes[i] = new Trie(*(other.charNodes[i]));
        }
    }
}

Trie &Trie::operator=(Trie other)
{
    // Swaps the array elements
    std::swap(wordFlag, other.wordFlag);
    for (int i = 0; i < 26; i++)
    {
        std::swap(charNodes[i], other.charNodes[i]);
    }
    // returns pointer to the newly swapped caller
    return *this;
}

void Trie::addWord(string word)
{
    // Starts at the root node
    Trie *current = this;

    // find the correctly numbered index for letter a-z
    for (char c : word)
    {
        int index = c - 'a';

        // Check if node for current letter exists, create one if no
        if (!current->charNodes[index])
        {
            current->charNodes[index] = new Trie();
        }

        // Moves to next letter (node)
        current = current->charNodes[index];
    }

    // set the word flag at the end of the word
    current->wordFlag = true;
}

bool Trie::isWord(string word)
{
    // Starts at the root node
    Trie *current = this;

    // find the correctly numbered index for letter a-z
    for (char c : word)
    {
        int index = c - 'a';

        // If node doesn't exist, the word can't be in the trie
        if (!current || !current->charNodes[index])
        {
            return false;
        }

        // Moves to next letter (node)
        current = current->charNodes[index];
    }

    // if node is marked as end of a word, it is in the trie.
    return current->wordFlag;
}

vector<string> Trie::allWordsStartingWithPrefix(string word)
{
    vector<string> wordList;
    Trie *current = this;

    // Traverse the nodes for the prefix word
    for (char c : word)
    {
        int index = c - 'a';
        if (!current || !current->charNodes[index])
        {
            return wordList;
        }
        current = current->charNodes[index];
    }

    // Recursively search words with the same prefix and add to same vector
    if (current)
    {
        getAllWords(current, word, wordList);
    }

    return wordList;
}

void Trie::getAllWords(Trie *node, string currentWord, vector<string> &wordList)
{
    if (!node)
        return;

    // If a node contains a word flag, add it to the referenced vector
    if (node->wordFlag)
    {
        wordList.push_back(currentWord);
    }

    // Recursively searches nodes in trie that branch off current tree.
    for (int i = 0; i < 26; i++)
    {
        if (node->charNodes[i])
        {
            Trie *nextNode = node->charNodes[i];
            getAllWords(nextNode, currentWord + char('a' + i), wordList);
        }
    }
}