using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GroupVarint.Tests
{
    public class GroupVarintTests
    {
        public unsafe static void EncodeDecodeTest(int count)
        {
            byte[] bytes = new byte[count * 5];
            uint[] src = new uint[count];
            uint[] dst = new uint[count];
            uint v1 = 0, v2 = 0, v3 = 0, v4 = 0;
            int pos = 0;
            int apos = 0, n = 0;
            for (int i = 0; i < count; i += 4)
            {
                GroupVarintUtils.GetValues(out v1, out v2, out v3, out v4);
                src[i] = v1;
                src[i + 1] = v2;
                src[i + 2] = v3;
                src[i + 3] = v4;
            }
            Stopwatch watch = Stopwatch.StartNew();
            pos = GroupVarintCodec.Encode(bytes, 0, src, 0, src.Length);
            int encodeCost = (int)watch.ElapsedMilliseconds;

            watch = Stopwatch.StartNew();
            //n = GroupVarintCodec.Decode(bytes, 0, pos, dst, ref apos);
            for (int i = 0; i < 100000; i++)
            {
                apos = 0;
                //n = GroupVarintCodec.Decode(bytes, 0, pos, dst, ref apos);
                n = DecodeForTestA(bytes, 0, pos, dst, ref apos);
            }
            int decodeCost = (int)watch.ElapsedMilliseconds;

            int errorCount = 0;
            for (int i = 0; i < count; i++)
                if (src[i] != dst[i])
                    errorCount++;
            string rs =
                $"encodeCost:{encodeCost}" +
                $"decodeCost:{decodeCost}" +
                $"errorCount:{errorCount}";
            Console.WriteLine(rs);
        }

        public unsafe static void EncodeDecodeSameTest(int count)
        {
            byte[] bytes = new byte[count * 5];
            uint[] src = new uint[count];
            uint[] dst = new uint[count];
            uint v1 = 0, v2 = 0, v3 = 0, v4 = 0;
            int pos = 0;
            int apos = 0, n = 0;
            for (int i = 0; i < count; i += 4)
            {
                src[i] = v1;
                src[i + 1] = v2;
                src[i + 2] = v3;
                src[i + 3] = v4;
            }
            Stopwatch watch = Stopwatch.StartNew();
            pos = GroupVarintCodec.Encode(bytes, 0, src, 0, src.Length);
            int encodeCost = (int)watch.ElapsedMilliseconds;

            watch = Stopwatch.StartNew();
            //n = GroupVarintCodec.Decode(bytes, 0, pos, dst, ref apos);
            for (int i = 0; i < 100000; i++)
            {
                apos = 0;
                //n = GroupVarintCodec.Decode(bytes, 0, pos, dst, ref apos);
                n = DecodeForTestA(bytes, 0, pos, dst, ref apos);
            }
            int decodeCost = (int)watch.ElapsedMilliseconds;

            int errorCount = 0;
            for (int i = 0; i < count; i++)
                if (src[i] != dst[i])
                    errorCount++;
            string rs = 
                $"encodeCost:{encodeCost}" +
                $"decodeCost:{decodeCost}" +
                $"errorCount:{errorCount}";
            Console.WriteLine(rs);
        }

        public unsafe static int DecodeForTestA(byte[] buffer, int bpos, int bsize, uint[] array, ref int ap)
        {
            int n = 0;
            int bend = bpos + bsize;

            fixed (byte* pSrc = &buffer[bpos])
            {
                fixed (uint* pDst = &array[ap])
                {
                    byte* bs = pSrc;
                    uint* aps = pDst;
                    while (bpos < bend)
                    {
                        *(aps + 0) = *(bs + 1);
                        *(aps + 1) = *(bs + 2);
                        *(aps + 2) = *(bs + 3);
                        *(aps + 3) = *(bs + 4);
                        bs += 5;
                        bpos += 5;
                        aps += 4;
                        n++;
                    }
                }
            }
            ap += (n << 2);
            return n << 2;
        }

        /// <summary>
        /// 实际性能测试，似乎没有A方案快
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="bpos"></param>
        /// <param name="bsize"></param>
        /// <param name="array"></param>
        /// <param name="ap"></param>
        /// <returns></returns>
        public unsafe static int DecodeForTestB(byte[] buffer, int bpos, int bsize, uint[] array, ref int ap)
        {
            int n = 0;
            fixed (byte* pSrc = &buffer[bpos])
            {
                fixed (uint* pDst = &array[ap])
                {
                    byte* bs = pSrc;
                    uint* aps = pDst;
                    uint* dStart = aps;

                    byte* bend = bs;
                    bend += bsize;
                    while (bs < bend)
                    {
                        *(aps + 0) = *(bs + 1);
                        *(aps + 1) = *(bs + 2);
                        *(aps + 2) = *(bs + 3);
                        *(aps + 3) = *(bs + 4);
                        bs += 5;
                        aps += 4;
                    }
                    n = (int)(aps - dStart);
                }
            }
            ap += (n << 2);
            return n << 2;
        }



        public unsafe static void GetByteSizeTestValidity()
        {
            uint v1 = 0, v2 = 0, v3 = 0, v4 = 0;
            for (int i = 0; i < 10000000; i++)
            {
                int n1 =GroupVarintUtils.GetValues(out v1, out v2, out v3, out v4);
                int n2 = GroupVarintUtils.GetByteSize(v1) << 6 | GroupVarintUtils.GetByteSize(v2) << 4 | GroupVarintUtils.GetByteSize(v3) << 2 | GroupVarintUtils.GetByteSize(v4);
                //if (n1 != n2)
                //    Console.WriteLine(n2);
                //Assert.AreEqual(n1, n2);
            }
            Console.WriteLine("ok");
            Console.ReadLine();
        }

        public unsafe static void GetByteSizeBenchMarkTest()
        {
            uint[] arr = new uint[100000000];
            uint v1 = 0, v2 = 0, v3 = 0, v4 = 0;
            for (int i = 0; i < arr.Length; i += 4)
            {
                int n1 = GroupVarintUtils.GetValues(out v1, out v2, out v3, out v4);
                arr[i] = v1;
                arr[i + 1] = v2;
                arr[i + 2] = v3;
                arr[i + 3] = v4;
            }
            Stopwatch w = Stopwatch.StartNew();
            GroupVarintUtils.GetLenthLookupCode(arr);
            int cost1 = (int)w.ElapsedMilliseconds;
            w = Stopwatch.StartNew();
            GroupVarintUtils.GetLenthIfelse(arr);
            int cost2 = (int)w.ElapsedMilliseconds;
            Console.WriteLine(
                $"GetLenthLookupCode Cost:{cost1}" +
                $"GetLenthIfelse Cost:{cost2}");

        }

        

        

    }

}
