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

        static private Dictionary<Int64, Node> Map = new Dictionary<Int64, Node>();

        // Implementation Reference
        private Dictionary<Int64, Int64> Vals { get; set; } // cache K and V
        private Dictionary<Int64, Int64> Counts { get; set; } // K and counters

        private Dictionary<Int64, LinkedHashSet<Int64>> Lists { get; set; } // Counter And Item List

        public RedisLFUMechanism(string hostName,
            int portNumber, Int64 capacity)
        {
            DB = new RedisDB(2);
            DB.Host.AddWriteHost(hostName, portNumber);
            Capacity = capacity;
            Vals = new Dictionary<Int64, Int64>();
            Counts = new Dictionary<Int64, Int64>();
            Lists = new Dictionary<Int64, LinkedHashSet<Int64>>();
            Lists.Add(capacity, new LinkedHashSet<Int64>());
        }

        public Int64 GetKey(Int64 key)
        {
            if (!Vals.ContainsKey(key))
            {
                return -1;
            }

            // Get the count from counts map
            Int64 Count = Counts[key];

            // Increase the counter
            Counts.Add(key, Count + 1);

            // Remove the element from the counter to LinkedHashSet
            Lists[Count].Remove(key);

            // When the current min does not have any data, next 
            // one would be the min
            if (Count == Minimum
            && Lists[Count].Count == 0)
            {
                Minimum += Minimum + 1;
            }

            if (!Lists.ContainsKey(Count + 1))
            {
                Lists.Add(Count + 1, new LinkedHashSet<Int64>());
            }

            Lists[Count].Add(key);

            return Vals[key];
        }

        public void SetKey(Int64 Key, Int64 Value)
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
                IEnumerator<Int64> enumerator = Lists[Minimum].GetEnumerator();
                if (enumerator.MoveNext())
                {
                    Int64 evit = enumerator.Current;
                    Lists[Minimum].Remove(evit);
                    Vals.Remove(evit);
                    Counts.Remove(evit);
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Invalid access at SetKey!");
                }
            }

            // If the key is new, insert the value and current
            // min should be 1 of course
            Vals.Add(Key, Value);
            Counts.Add(Key, 1);
            Minimum = 1;
            Lists[1].Add(Key);
        }
    }
}

/*
References,

- https://stackoverflow.com/questions/9346526/what-is-the-equivalent-of-linkedhashset-java-in-c
*/