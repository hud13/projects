# Makefile for the trie class in assignment 4. 
# Author: Hudson Dalby
# Modified: 2/4/25

# Compiler
CC = g++
CFLAGS = -Wall -fsanitize=undefined

# Directories 
INC = ./include
SRC = ./src

# Objects listed 
OBJS = trieTest.o trie.o

trieTest: $(OBJS)
	$(CC) $(CFLAGS) -o trieTest $(OBJS)

trieTest.o: $(SRC)/trieTest.cpp $(INC)/trie.h
	$(CC) $(CFLAGS) -I$(INC) -c $(SRC)/trieTest.cpp

trie.o: $(SRC)/trie.cpp $(INC)/trie.h
	$(CC) $(CFLAGS) -I$(INC) -c $(SRC)/trie.cpp

clean: 
	rm -f trieTest *.o