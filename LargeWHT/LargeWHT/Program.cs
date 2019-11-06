using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace LargeWHT
{
    class Program
    {
        //This code performs an in-place Walsh-Hadamard Transform (WHT) for very large files.
        //Edit the values of N and B, and change the filepath

        static void Main(string[] args)
        {

            int N = 32; //2^N is the size of your input matrix
            int B = 28; //2^B is the largest block that will fit in a C# array
                        //You want this as large as possible because using RAM is faster (duh) 
                        
            string path = "data.dat";
            
            Stopwatch timer = new Stopwatch();
            //For testing, create a file to perform WHT on:
            //
            /*timer.Start();
            using (FileStream fs = File.Open("xor.dat", FileMode.Create, FileAccess.Write))
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                for (long a = 0; a < (1L << N / 2); ++a)
                {
                    for (long b = 0; b < (1L << N / 2); ++b)
                    {
                        bw.Write(a ^ b);
                    }
                }
            }
            Console.WriteLine("done writing input: {0} ms", timer.ElapsedMilliseconds);
            */
            
            timer.Restart();
            using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.ReadWrite))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        //first round
                        long[] M = new long[1 << B];
                        for (long u = 0; u < (1L << (N - B)); ++u)
                        {
                            for(int q = 0; q < (1 << B); ++q)
                            {
                                M[q] = br.ReadInt64();
                            }
                            WHT(M, B);
                            bw.BaseStream.Seek(-8 * (1L << B), SeekOrigin.Current);
                            for (int q = 0; q < (1 << B); ++q)
                            {
                                bw.Write(M[q]);
                            }
                        }

                        int blockSize = 2048;
                        for (int k = B; k < N; ++k)
                        {
                            long pt = 0;
                            long j = 1L << k;
                            for (long i = 0; i < (1L << (N - 1)); i+=blockSize)
                            {
                                
                                br.BaseStream.Seek(8 * pt, SeekOrigin.Begin);
                                byte[] block1 = br.ReadBytes(8 * blockSize);
                                long[] b1 = new long[blockSize];
                                for (int q = 0; q < blockSize; ++q)
                                    b1[q] = BitConverter.ToInt64(block1, 8 * q);


                                long[] t1 = new long[b1.Length];
                                long[] t2 = new long[b1.Length];
                                br.BaseStream.Seek(8 * (j-blockSize), SeekOrigin.Current);
                                byte[] block2 = br.ReadBytes(8 * blockSize);
                                for (int q = 0; q < blockSize; ++q)
                                {
                                    long b2 = BitConverter.ToInt64(block2, 8 * q);
                                    t1[q] = b1[q] + b2;
                                    t2[q] = b1[q] - b2;
                                }

                                bw.BaseStream.Seek(8 * (pt), SeekOrigin.Begin);
                                foreach (long l in t1)
                                    bw.Write(l);

                                bw.BaseStream.Seek(8 * (j - blockSize), SeekOrigin.Current);
                                foreach (long l in t2)
                                    bw.Write(l);

                                pt += blockSize;

                                if ((pt & (j - 1L)) == 0)
                                {
                                    pt += j;
                                }
                            }
                        }
                    }
                }
            }
            Console.WriteLine("WHT Done... {0} ms", timer.ElapsedMilliseconds);
        }

        static void WHT(long[] M, int N)
        {
            for(int i = 0; i < N; ++i)
            {
                for(long j = 0; j < (1 << N); j += 1 << (i+1))
                {
                    for(long k = 0; k < (1 << i); ++k)
                    {
                        long tmp = M[j + k];
                        M[j + k] += M[j + k + (1 << i)];
                        M[j + k + (1 << i)] -= tmp;
                    }
                }
            }
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        static long intPow(long x, int p)
        {
            long a = 1;
            while (p > 0)
            {
                a *= x;
                --p;
            }
            return a;
        }
        
        static int log2(long x)
        {
            int p;
            for(p = 0; x > 1; x >>= 1)
            {
                ++p;
            }
            return p;
        }
    }
}
