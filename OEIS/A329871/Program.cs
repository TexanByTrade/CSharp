//Christopher Cormier, Dec 17 2019
//--------------------------------
//This requires C# .NET Core 3.0+!
//Download Visual Studio 2019 or later
//and create a .NET Core project.
//--------------------------------
//Calculates up to n=15 in a few
//minutes.
//--------------------------------
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Runtime.Intrinsics.X86;
using System.Numerics;

class Program
{
    static void Main()
    {
        //Search past n=15 at your own risk!
        //The lookup table will grow much
        //larger than 2 GB, eventually
        //crashing the program.

        for (int n = 0; n <= 15; ++n)
        {
            Console.WriteLine(MinecraftWater.A329871(n, n, 0, 0));
        }
    }
}
class MinecraftWater
{
    static object writeLock = new object();
    //The use of a concurrent dict might not be necessary,
    //but replacing with regular dict won't be a noticeable speedup
    //I'd guess.
    static List<List<ConcurrentDictionary<uint, BigInteger>>> memoizeTable = new List<List<ConcurrentDictionary<uint, BigInteger>>>();
    static List<uint> non5 = new List<uint>();

    static IEnumerable<uint> NonFives(int n)
    {
        //returns numbers with no 101 in 
        //binary representation (A004742)
        if (non5.Count == 0)
        {
            for (uint k = 0; k < (1u << n); ++k)
            {
                bool success = true;
                uint s = k;
                while (s > 0)
                {
                    if ((s & 7u) == 5u)
                    {
                        success = false;
                        break;
                    }
                    s >>= 1;
                }
                if (success)
                {
                    non5.Add(k);
                }
            }
        }
        var m = non5.Count;
        for (int i = 0; i < m; ++i)
        {
            yield return non5[i];
        }
    }

    //We use a top-down recursion approach. For each valid row,
    //we go to the next row and determine if it's compatible,
    //etc. down to the bottom row.
    public static BigInteger A329871(int n, int level, uint oneAbove, uint twoAbove)
    {
        if (level == 0) return 1;
        if (level == n)
        {
            //initialize lists
            non5.Clear();
            memoizeTable.Clear();
            for (int depth = 0; depth < (n - 2); ++depth)
            {
                memoizeTable.Add(new List<ConcurrentDictionary<uint, BigInteger>>());
                for (int i = 0; i < (1 << n); ++i)
                {
                    memoizeTable[depth].Add(new ConcurrentDictionary<uint, BigInteger>());
                }
            }

            //on my 4-core/8-thread computer,
            //I only get a 2x speedup.
            BigInteger c = 0;
            Parallel.ForEach(NonFives(n), u =>
            {
                BigInteger d = A329871(n, level - 1, u, 0);
                lock (writeLock) { c += d; }
            });
            return c;
        }
        else if (level == (n - 1))
        {
            BigInteger c = 0;
            foreach (uint v in NonFives(n))
            {
                uint s = oneAbove;
                uint t = v;
                while (s > 0 && t > 0)
                {
                    //mask the lower 2 bits
                    uint ss = s & 3u;
                    uint tt = t & 3u;
                    //Karnaugh map
                    if (((ss > 1u) && (tt == 1u)) || ((ss == 2u) && (tt % 2u == 1u)) || ((ss == 1u) && (tt > 1u)) || ((ss % 2u == 1u) && (tt == 2u)))
                    {
                        goto nextLoop;
                    }
                    s >>= 1;
                    t >>= 1;
                }
                c += A329871(n, level - 1, v, oneAbove);
            nextLoop:
                continue;
            }
            return c;
        }
        else if (level == 1)
        {
            BigInteger c = 0;
            foreach (uint w in NonFives(n))
            {
                //Hardware instruction for counting 
                //# of bits set:
                if (Popcnt.PopCount(twoAbove & w & (~oneAbove)) > 0)
                {
                    //vertical 101 is present
                    continue;
                }

                uint s = oneAbove;
                uint t = w;
                while (s > 0 && t > 0)
                {
                    uint ss = s & 3u;
                    uint tt = t & 3u;

                    if (((ss > 1u) && (tt == 1u)) || ((ss == 2u) && (tt % 2u == 1u)) || ((ss == 1u) && (tt > 1u)) || ((ss % 2u == 1u) && (tt == 2u)))
                    {
                        goto nextLoop;
                    }
                    s >>= 1;
                    t >>= 1;
                }
                c++;

            nextLoop:
                continue;
            }
            return c;
        }
        else
        {
            BigInteger c = 0;
            foreach (uint w in NonFives(n))
            {
                if (Popcnt.PopCount(twoAbove & w & (~oneAbove)) > 0)
                {
                    continue;
                }

                uint s = oneAbove;
                uint t = w;
                while (s > 0 && t > 0)
                {
                    uint ss = s & 3u;
                    uint tt = t & 3u;

                    if (((ss > 1u) && (tt == 1u)) || ((ss == 2u) && (tt % 2u == 1u)) || ((ss == 1u) && (tt > 1u)) || ((ss % 2u == 1u) && (tt == 2u)))
                    {
                        goto nextLoop;
                    }
                    s >>= 1;
                    t >>= 1;
                }

                //If we've seen this result before, return it
                if (memoizeTable[n - level - 2][(int)oneAbove].TryGetValue(w, out BigInteger d))
                    c += d;
                else
                {
                    //Otherwise, determine the value
                    var nd = A329871(n, level - 1, w, oneAbove);
                    memoizeTable[n - level - 2][(int)oneAbove].AddOrUpdate(w, nd, (key, old) => old);
                    c += nd;
                }

            nextLoop:
                continue;
            }
            return c;
        }
    }
}