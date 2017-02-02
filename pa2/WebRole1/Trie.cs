using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebRole1
{
    public class Trie
    {
        private TrieNode root;
        private int maxSuggestions;
        public int Count;

        public Trie() : this(new TrieNode(), 10) { }

        public Trie(TrieNode root, int maxSuggestions)
        {
            this.root = root;
            this.maxSuggestions = maxSuggestions;
            Count = 0;
        }

        public void AddTitle(String title)
        {
            TrieNode currentNode = root;
            for (int i = 0; i < title.Length; i++)
            {
                string charAtIndex = "" + title[i];
                // if character doesn't exist, then add it
                if (!currentNode.children.ContainsKey(charAtIndex))
                {
                    currentNode.children.Add(charAtIndex, new TrieNode(title[i]));
                }
                currentNode = currentNode.children[charAtIndex];
            }
            Count++;
            currentNode.isValid = true;
        }

        public List<string> SearchForPrefix(string prefix)
        {
            List<string> suggestions = new List<string>();
            if (prefix.Length > 0)
            {
                SearchForPrefix(prefix, suggestions, root, "");
            }
            return suggestions;
        }

        private void SearchForPrefix(string prefix, List<string> suggestions, TrieNode root, string current)
        {
            if (root != null && suggestions.Count < maxSuggestions)
            {
                if (prefix.Length > current.Length) // don't get any suggestions that are shorter than what the user has typed
                {
                    string charAtIndex = "" + prefix[current.Length];
                    if (root.children.ContainsKey(charAtIndex))
                    {
                        SearchForPrefix(prefix, suggestions, root.children[charAtIndex], current + charAtIndex);
                    }
                }
                else // get all available suggestions
                {
                    if (root.isValid)
                    {
                        suggestions.Add(current);
                    }
                    foreach (KeyValuePair<string, TrieNode> kvp in root.children)
                        SearchForPrefix(prefix, suggestions, root.children[kvp.Key], current + kvp.Key);
                }
            }
        }
    }
}