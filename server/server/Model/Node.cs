using System;

namespace server
{
    class Node
    {
        public Int64 Key;
        public Int64 Value;
        public Node Prev;
        public Node Next;

        public Node(Int64 key, Int64 value)
        {
            Key = key;
            Value = value;
            Prev = null;
            Next = null;
        }
    }
}
