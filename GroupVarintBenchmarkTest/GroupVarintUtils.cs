using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace GroupVarint.Tests
{
    public class GroupVarintUtils
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
