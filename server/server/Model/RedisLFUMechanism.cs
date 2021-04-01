using System;
using System.Collections.Generic;
using BeetleX.Redis;

namespace server
{
    public class RedisLFUMechanism
    {
        public RedisDB DB { get; set; }

        static private Int64 Capacity { get; set; }
        private Int64 Minimum { get; set; }

        static private Dictionary<string, Node> Map = new Dictionary<string, Node>();

        // Implementation Reference
        private Dictionary<string, string> Vals { get; set; } // cache K and V
        private Dictionary<string, Int64> Counts { get; set; } // K and counters

        private Dictionary<Int64, LinkedHashSet<string>> Lists { get; set; } // Counter And Item List

        public RedisLFUMechanism(string hostName,
            int portNumber, Int64 capacity)
        {
            // DB = new RedisDB(2);
            // DB.Host.AddWriteHost(hostName, portNumber);
            Capacity = capacity;
            Vals = new Dictionary<string, string>();
            Counts = new Dictionary<string, Int64>();
            Lists = new Dictionary<Int64, LinkedHashSet<string>>();
            Lists.Add(capacity, new LinkedHashSet<string>());
        }

        public string GetKey(string key)
        {
            if (!Vals.ContainsKey(key))
            {
                return "";
            }

            // Get the count from counts map
            Int64 Count = Counts[key];

            // Increase the counter
            if (!Counts.ContainsKey(key))
            {
                Counts.Add(key, Count + 1);
            }


            // Remove the element from the counter to LinkedHashSet
            Lists[Count].Remove(key);


            // When the current     min does not have any data, next 
            // one would be the min
            if (Count == Minimum
            && Lists[Count].Count == 0)
            {
                Minimum += Minimum + 1;
            }


            if (!Lists.ContainsKey(Count + 1))
            {
                Lists.Add(Count + 1, new LinkedHashSet<string>());
            }

            Lists[Count].Add(key);

            return Vals[key];
        }

        public void SetKey(string Key, string Value)
        {
            if (Capacity <= 0)
            {
                return;
            }

            // If key does exist, we're returning from here
            if (Vals.ContainsKey(Key))
            {
                Vals.Add(Key, Value);
                GetKey(Key);
                return;
            }

            if (Vals.Count >= Capacity)
            {
                IEnumerator<string> enumerator = Lists[Minimum].GetEnumerator();
                if (enumerator.MoveNext())
                {
                    string evit = enumerator.Current;
                    Lists[Minimum].Remove(evit);
                    Vals.Remove(evit);
                    Counts.Remove(evit);
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Invalid access at SetKey!");
                }
            }

            Console.WriteLine("Hereererre");

            // If the key is new, insert the value and current
            // min should be 1 of course
            if (!Vals.ContainsKey(Key))
            {
                Vals.Add(Key, Value);
            }

            if (!Counts.ContainsKey(Key))
            {
                Counts.Add(Key, 1);
            }
            Minimum = 1;
            if (!Lists.ContainsKey(Minimum))
            {
                Lists.Add(Minimum, new LinkedHashSet<string>());
            }
            else
            {
                Lists[Minimum].Add(Key);
            }
        }
    }
}

/*
References,

- https://stackoverflow.com/questions/9346526/what-is-the-equivalent-of-linkedhashset-java-in-c
*/