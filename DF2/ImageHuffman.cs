using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DF2
{
    public class ImageHuffman
    {
        public class Node
        {
            public int fr { get; set; }
            public int coefficient { get; set; }
            public Node ch1, ch2; 

            public Node() { }

            public Node(int c, int f = -1)
            {
                fr = f;
                coefficient = c;          
                ch1 = null;
                ch2 = null;
            }

            public Node(Node c1, Node c2)
            {
                ch1 = c1;
                ch2 = c2;
                fr = c1.fr + c2.fr;
            }

            public List<bool> MakeWay(int c, List<bool> way)
            {
                if (ch1 == null && ch2 == null)
                {
                    if (c.Equals(this.coefficient))
                        return way;
                    else return null;
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

                        left = ch1.MakeWay(c, leftPath);
                    }

                    if (ch2 != null)
                    {
                        List<bool> rightPath = new List<bool>();
                        rightPath.AddRange(way);
                        rightPath.Add(true);
                        right = ch2.MakeWay(c, rightPath);
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
            public Dictionary<int, int> freq = new Dictionary<int, int>();
            Node currNode { get; set; }


            public void MakeTreeWithFrequencies()
            {
                if (freq == null)
                    return;
                foreach (KeyValuePair<int, int> s in freq)
                {
                    nodes.Add(new Node() { coefficient = s.Key, fr = s.Value });
                }

                while (nodes.Count > 1)
                {
                    List<Node> nodesordered = nodes.OrderBy(node => node.fr).ToList<Node>();

                    if (nodesordered.Count >= 2)
                    {
                        List<Node> current = nodesordered.Take(2).ToList<Node>();

                        Node parent = new Node()
                        {
                            coefficient = '*',
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

            public void MakeTree(List<int> s)
            {
                for (int i = 0; i < s.Count; i++)
                {
                    if (!freq.ContainsKey(s[i]))
                        freq.Add(s[i], 0);
                    freq[s[i]]++;
                }
                MakeTreeWithFrequencies();
            }

            public List<bool> CompressImage(List<int> s)
            {
                List<bool> ret = new List<bool>();
                for (int i = 0; i < s.Count; i++)
                {
                    List<bool> compressedValue = this.root.MakeWay(s[i], new List<bool>());
                    ret.AddRange(compressedValue);
                }
                return ret;
            }

            public List<int> DecompressImage(List<bool> bits)
            {
                Node curr = this.root;
                List<int> decoded = new List<int>();

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
                        decoded.Add(curr.coefficient);
                        if (curr.coefficient == 1001)
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
