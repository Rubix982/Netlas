using System;
using System.Collections.Generic;
using BeetleX.Redis;

namespace server
{
    public class RedisLRUMechanism
    {
        public RedisDB DB { get; }
        static private Node Head { get; set; }
        static private Node Tail { get; set; }
        static private Dictionary<string, Node> Map = new Dictionary<string, Node>();

        static private Int64 Capacity { get; set; } = 0;

        public RedisLRUMechanism(string hostName,
            int portNumber, Int64 capacity)
        {
            // DB = new RedisDB(1);
            // DB.Host.AddWriteHost(hostName, portNumber);
            Capacity = capacity;
        }

        public string GetKey(string key)
        {
            if (!Map.ContainsKey(key))
            {
                return "";
            }

            //move to tail
            Node newNode = Map[key];

            removeNode(newNode);
            offerNode(newNode);

            return newNode.Value;
        }

        public void PutKey(string key, string value)
        {
            if (Map.ContainsKey(key))
            {
                Node newNode = Map[key];
                newNode.Value = value;

                //move to tail
                removeNode(newNode);
                offerNode(newNode);
            }
            else
            {
                if (Map.Count >= Capacity)
                {
                    //delete head
                    Map.Remove(Head.Key);
                    removeNode(Head);
                }

                // Add to tail
                Node node = new Node(key, value);

                offerNode(node);
                Map[key] = node;
            }
        }

        private void removeNode(Node node)
        {
            if (node.Prev != null)
            {
                node.Prev.Next = node.Next;
            }
            else
            {
                Head = node.Next;
            }

            if (node.Next != null)
            {
                node.Next.Prev = node.Prev;
            }
            else
            {
                Tail = node.Prev;
            }
        }

        private void offerNode(Node node)
        {
            if (Tail != null)
            {
                Tail.Next = node;
            }

            node.Prev = Tail;
            node.Next = null;
            Tail = node;

            if (Head == null)
            {
                Head = Tail;
            }
        }
    }
}
