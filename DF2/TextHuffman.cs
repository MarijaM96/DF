using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DF2
{
   public class TextHuffman
    {
        public class Node
        {
            public int fr { get; set; }
            public char c { get; set; } 
            public Node ch1, ch2;

            public Node() { }

            public Node(char d, int f = -1)
            {
                fr = f;
                c = d;
                ch1 = null;
                ch2 = null;
            }

            public Node(Node c1, Node c2)
            {
                ch1 = c1;
                ch2 = c2;
                fr = c1.fr + c2.fr;
            }

            public List<bool> MakeWay(char s, List<bool> way)
            {
                if (ch1 == null && ch2 == null)
                {
                    if (s.Equals(this.c))
                        return way;
                    else  return null;
                }
                else
                {
                    List<bool> left = null;
                    List<bool> right = null;

                    if (ch1 != null)
                    {
                        List<bool> leftPath = new List<bool>();
                        leftPath.AddRange(way);
                        leftPath.Add(false);

                        left = ch1.MakeWay(s, leftPath);
                    }

                    if (ch2 != null)
                    {
                        List<bool> rightPath = new List<bool>();
                        rightPath.AddRange(way);
                        rightPath.Add(true);
                        right = ch2.MakeWay(s, rightPath);
                    }
                    if (left != null) return left; 
                    else return right;              
                }
            }
        }

        public class HuffmanTree
        {
            private List<Node> nodes = new List<Node>();
            public Node root { get; set; }
            public Dictionary<char, int> freq = new Dictionary<char, int>();
            Node currNode { get; set; }


            public void MakeTreeWithFrequencies()
            {
                if (freq == null)
                    return;
                foreach (KeyValuePair<char, int> s in freq)
                {
                    nodes.Add(new Node() { c = s.Key, fr = s.Value });
                }

                while (nodes.Count > 1)
                {
                    List<Node> nodesordered = nodes.OrderBy(node => node.fr).ToList<Node>();

                    if (nodesordered.Count >= 2)
                    {
                        List<Node> current = nodesordered.Take(2).ToList<Node>();

                        Node parent = new Node()
                        {
                            c = '*',
                            fr = current[0].fr + current[1].fr,
                            ch1 = current[0],
                            ch2 = current[1]
                        };

                        nodes.Remove(current[0]);
                        nodes.Remove(current[1]);
                        nodes.Add(parent);
                    }
                    this.root = nodes.FirstOrDefault();
                }
            }

            public void MakeTree(string s)
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (!freq.ContainsKey(s[i]))
                        freq.Add(s[i], 0);
                    freq[s[i]]++;
                }
                MakeTreeWithFrequencies();
            }

            public List<bool> CompressText(string s)
            {
                List<bool> ret = new List<bool>();
                for (int i = 0; i < s.Length; i++)
                {
                    List<bool> compressedChar = this.root.MakeWay(s[i], new List<bool>());
                    ret.AddRange(compressedChar);
                }
                return ret;
            }

            public string DecompressText(List<bool> bits)
            {
                Node curr = this.root;
                string decoded = "";

                foreach (bool b in bits)
                {
                    if (b)
                    {
                        if (curr.ch2 != null)
                            curr = curr.ch2;
                    }
                    else
                    {
                        if (curr.ch1 != null)
                            curr = curr.ch1;
                    }

                    if (Leaf(curr))
                    {
                        decoded += curr.c;
                        if (curr.c.ToString() == "\0")
                            break;
                        curr = this.root;
                    }
                }
                return decoded;
            }

            public bool Leaf(Node node)
            {
                return (node.ch1 == null && node.ch2 == null);
            }

        }
    }
}
