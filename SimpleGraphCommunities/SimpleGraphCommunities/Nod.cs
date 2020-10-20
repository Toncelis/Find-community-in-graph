using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleGraphCommunities
{
    class Nod
    {

        public int Value;
        public List<int> Connections;

        public Nod ()
        {
            Value = 0;
            Connections = new List<int>();
        }

        public Nod(List<int> connections)
        {
            Connections = connections;
        }

        public void AddConnection (int newNod)
        {
            if ((!Connections.Contains (newNod)))
            {
                Connections.Add(newNod);
            }
        }

        public Nod (string sampleString) // index : connection1 connection2 connection3 ... /n
        {
           this.Connections = new List<int>();

           List<string> substrings = sampleString.Split(' ').ToList();
           if (substrings[1] != ":")
            {
                throw new System.ArgumentException("Wrong string description of a nod");
            }

            int connectionReader;

            if (int.TryParse(substrings[0], out connectionReader))
            {
                this.Value = connectionReader;
            }
            else
            {
                throw new System.ArgumentException("Wrong string description of a nod");
            }

            for (int i = 2; i < substrings.Count; i++)
            {
                if (int.TryParse(substrings[i], out connectionReader))
                {
                    this.AddConnection(connectionReader);
                }
                else
                {
                    throw new System.ArgumentException($"Wrong string description of a nod. [{substrings[i]}]");
                }
            }
            
        }
    }
}
