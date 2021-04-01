using System;

namespace server
{
    class Node
    {
        public String Key;
        public String Value;
        public Node Prev;
        public Node Next;

        public Node(String key, String value)
        {
            Key = key;
            Value = value;
            Prev = null;
            Next = null;
        }
    }
}
