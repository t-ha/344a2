using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebRole1
{
    public class TrieNode
    {
        public char letter;
        public SortedDictionary<string, TrieNode> children = new SortedDictionary<string, TrieNode>(StringComparer.InvariantCultureIgnoreCase);
        public bool isValid;

        public TrieNode() : this('0', false) { }

        public TrieNode(char letter) : this(letter, false) { }

        public TrieNode(char letter, bool isValid)
        {
            this.letter = letter;
            this.isValid = isValid;
        }

        override public String ToString()
        {
            return ("" + letter);
        }
    }
}