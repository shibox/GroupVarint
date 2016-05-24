using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        #region 字段

        readonly static int[] codeIdx = new int[] {
            0, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1
        };

        readonly static int[] lenId2 = new int[] { 0, 0, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3 };

        static Random rd = new Random(Guid.NewGuid().GetHashCode());

        #endregion

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
                GetValues(out v1, out v2, out v3, out v4);
                src[i] = v1;
                src[i + 1] = v2;
                src[i + 2] = v3;
                src[i + 3] = v4;
            }
            Stopwatch watch = Stopwatch.StartNew();
            pos = GroupVarintCodec.Encode(bytes, 0, src, 0, src.Length);
            int encodeCost = (int)watch.ElapsedMilliseconds;

            watch = Stopwatch.StartNew();
            n = GroupVarintCodec.Decode(bytes, 0, pos, dst, ref apos);
            int decodeCost = (int)watch.ElapsedMilliseconds;

            int errorCount = 0;
            for (int i = 0; i < count; i++)
                if (src[i] != dst[i])
                    errorCount++;
            string rs = "encodeCost:" + encodeCost + ",decodeCost:" + decodeCost + " errorCount:" + errorCount;
            Console.WriteLine(rs);
            Console.ReadLine();
        }
        
        public unsafe static void GetByteSizeTestValidity()
        {
            uint v1 = 0, v2 = 0, v3 = 0, v4 = 0;
            for (int i = 0; i < 10000000; i++)
            {
                int n1 = GetValues(out v1, out v2, out v3, out v4);
                int n2 = GetByteSize(v1) << 6 | GetByteSize(v2) << 4 | GetByteSize(v3) << 2 | GetByteSize(v4);
                //if (n1 != n2)
                //    Console.WriteLine(n2);
                Assert.AreEqual(n1, n2);
            }
            Console.WriteLine("ok");
            Console.ReadLine();
        }

        public unsafe static void GetByteSizeTest()
        {
            uint[] arr = new uint[100000000];
            uint v1 = 0, v2 = 0, v3 = 0, v4 = 0;
            for (int i = 0; i < arr.Length; i += 4)
            {
                int n1 = GetValues(out v1, out v2, out v3, out v4);
                arr[i] = v1;
                arr[i + 1] = v2;
                arr[i + 2] = v3;
                arr[i + 3] = v4;
            }
            Stopwatch w = Stopwatch.StartNew();
            //int len = GetLenthByLookupCode(arr);
            int len = GetLenthIfelse(arr);
            w.Stop();

            Console.WriteLine(w.ElapsedMilliseconds + " ok");
            Console.ReadLine();
        }

        public unsafe static int GetLenthLookupCode(uint[] arr)
        {
            int len = 0;
            fixed (int* p1 = &codeIdx[0], p2 = &lenId2[0])
            {
                for (int i = 0; i < arr.Length; i += 4)
                {
                    len += GroupVarintCodec.GetByteSize(arr[i], p1, p2);
                    len += GroupVarintCodec.GetByteSize(arr[i + 1], p1, p2);
                    len += GroupVarintCodec.GetByteSize(arr[i + 2], p1, p2);
                    len += GroupVarintCodec.GetByteSize(arr[i + 3], p1, p2);
                }
            }
            return len;
        }

        public static int GetLenthIfelse(uint[] arr)
        {
            int len = 0;
            for (int i = 0; i < arr.Length; i += 4)
            {
                len += GetLength(arr[i]);
                len += GetLength(arr[i + 1]);
                len += GetLength(arr[i + 2]);
                len += GetLength(arr[i + 3]);
            }
            return len;
        }

        public static uint GetValueByByteSize(uint bits)
        {
            if (bits == 0)
                return (uint)rd.Next(1, 255);
            else if (bits == 1)
                return (uint)rd.Next(256, 256 * 256 - 1);
            else if (bits == 2)
                return (uint)rd.Next(256 * 256, 256 * 256 * 256 - 1);
            else if (bits == 3)
                return (uint)rd.Next(256 * 256 * 256, int.MaxValue);
            else
                throw new Exception();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetLength(uint v1)
        {
            if (v1 < 256)
                return 0;
            else if (v1 < 256 * 256)
                return 1;
            else if (v1 < 256 * 256 * 256)
                return 2;
            else
                return 3;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetByteSize(uint v)
        {
            int n = (codeIdx[v >> 24] << 3);
            n += (codeIdx[(byte)(v >> 16)] << 2);
            n += (codeIdx[(byte)(v >> 8)] << 1);
            n += (codeIdx[(byte)(v)]);
            return lenId2[n];
        }

        public static int GetValues(out uint v1, out uint v2, out uint v3, out uint v4)
        {
            uint n = (uint)rd.Next(0, 0);
            v1 = GetValueByByteSize(n >> 6);
            v2 = GetValueByByteSize((n << 26) >> 30);
            v3 = GetValueByByteSize((n << 28) >> 30);
            v4 = GetValueByByteSize((n << 30) >> 30);
            return (int)n;
        }

        

    }

}
