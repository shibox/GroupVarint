using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupVarint.Tests
{
    class TestCodes
    {
        public unsafe static void WriteGVInt1(byte[] buffer, uint v1, uint v2, uint v3, uint v4, ref int pos)
        {
            byte b = 0;
            int p = pos;
            pos++;
            if (v1 < 256)
            {
                buffer[pos] = (byte)(v1);
                pos += 1;
            }
            else if (v1 < 256 * 256)
            {
                fixed (byte* bp = &buffer[pos])
                {
                    byte* bpp = bp;
                    *((ushort*)bpp) = (ushort)v1;
                }
                b |= 0x40;
                pos += 2;
            }
            else if (v1 < 256 * 256 * 256)
            {
                fixed (byte* bp = &buffer[pos])
                {
                    byte* bpp = bp;
                    //*((int*)(bpp - 1)) = (((int)(*bpp)) << 24) | (int)v1;
                    *((uint*)bpp) = (uint)v1;
                }

                b |= 0x80;
                pos += 3;
            }
            else if (v1 < 256 * 256 * 256 * 256L)
            {
                fixed (byte* bp = &buffer[pos])
                {
                    byte* bpp = bp;
                    *((uint*)bpp) = (uint)v1;
                }
                b |= 0xC0;
                pos += 4;
            }
            else
            {
                throw new Exception("数字超出了范围！");
            }

            if (v2 < 256)
            {
                buffer[pos] = (byte)(v2);
                pos += 1;
            }
            else if (v2 < 256 * 256)
            {
                fixed (byte* bp = &buffer[pos])
                {
                    byte* bpp = bp;
                    *((ushort*)bpp) = (ushort)v2;
                }
                b |= 0x10;
                pos += 2;
            }
            else if (v2 < 256 * 256 * 256)
            {
                fixed (byte* bp = &buffer[pos])
                {
                    byte* bpp = bp;
                    *((uint*)bpp) = (uint)v2;
                }
                b |= 0x20;
                pos += 3;
            }
            else if (v2 < 256 * 256 * 256 * 256L)
            {
                fixed (byte* bp = &buffer[pos])
                {
                    byte* bpp = bp;
                    *((uint*)bpp) = (uint)v2;
                }
                b |= 0x30;
                pos += 4;
            }
            else
            {
                throw new Exception("数字超出了范围！");
            }

            if (v3 < 256)
            {
                buffer[pos] = (byte)(v3);
                pos += 1;
            }
            else if (v3 < 256 * 256)
            {
                fixed (byte* bp = &buffer[pos])
                {
                    byte* bpp = bp;
                    *((ushort*)bpp) = (ushort)v3;
                }
                b |= 0x04;
                pos += 2;
            }
            else if (v3 < 256 * 256 * 256)
            {
                fixed (byte* bp = &buffer[pos])
                {
                    byte* bpp = bp;
                    *((uint*)bpp) = (uint)v3;
                }
                b |= 0x08;
                pos += 3;
            }
            else if (v3 < 256 * 256 * 256 * 256L)
            {
                fixed (byte* bp = &buffer[pos])
                {
                    byte* bpp = bp;
                    *((uint*)bpp) = (uint)v3;
                }
                b |= 0x0C;
                pos += 4;
            }
            else
            {
                throw new Exception("数字超出了范围！");
            }

            if (v4 < 256)
            {
                buffer[pos] = (byte)(v4);
                pos += 1;
            }
            else if (v4 < 256 * 256)
            {
                fixed (byte* bp = &buffer[pos])
                {
                    byte* bpp = bp;
                    *((ushort*)bpp) = (ushort)v4;
                }
                b |= 0x01;
                pos += 2;
            }
            else if (v4 < 256 * 256 * 256)
            {
                fixed (byte* bp = &buffer[pos])
                {
                    byte* bpp = bp;
                    *((uint*)bpp) = (uint)v4;
                }
                b |= 0x02;
                pos += 3;
            }
            else if (v4 < 256 * 256 * 256 * 256L)
            {
                fixed (byte* bp = &buffer[pos])
                {
                    byte* bpp = bp;
                    *((uint*)bpp) = (uint)v4;
                }
                b |= 0x03;
                pos += 4;
            }
            else
            {
                throw new Exception("数字超出了范围！");
            }
            buffer[p] = b;
        }

        public unsafe static byte* WriteGVInt1(byte* buffer, uint v1, uint v2, uint v3, uint v4, ref int pos)
        {
            byte b = 0;
            int p = pos;
            pos++;
            if (v1 < 256)
            {
                *buffer = (byte)(v1);
                pos += 1;
                buffer++;
            }
            else if (v1 < 256 * 256)
            {
                *((ushort*)buffer) = (ushort)v1;
                b |= 0x40;
                pos += 2;
                buffer += 2;
            }
            else if (v1 < 256 * 256 * 256)
            {
                *((uint*)buffer) = (uint)v1;
                b |= 0x80;
                pos += 3;
                buffer += 3;
            }
            else if (v1 < 256 * 256 * 256 * 256L)
            {
                *((uint*)buffer) = (uint)v1;
                b |= 0xC0;
                pos += 4;
                buffer += 4;
            }
            else
            {
                throw new Exception("数字超出了范围！");
            }

            if (v2 < 256)
            {
                *buffer = (byte)(v2);
                pos += 1;
                buffer++;
            }
            else if (v2 < 256 * 256)
            {
                *((ushort*)buffer) = (ushort)v2;
                b |= 0x10;
                pos += 2;
                buffer += 2;
            }
            else if (v2 < 256 * 256 * 256)
            {
                *((uint*)buffer) = (uint)v2;
                b |= 0x20;
                pos += 3;
                buffer += 3;
            }
            else if (v2 < 256 * 256 * 256 * 256L)
            {
                *((uint*)buffer) = (uint)v2;
                b |= 0x30;
                pos += 4;
                buffer += 4;
            }
            else
            {
                throw new Exception("数字超出了范围！");
            }

            if (v3 < 256)
            {
                *buffer = (byte)(v3);
                pos += 1;
                buffer++;
            }
            else if (v3 < 256 * 256)
            {
                *((ushort*)buffer) = (ushort)v3; ;
                b |= 0x04;
                pos += 2;
                buffer += 2;
            }
            else if (v3 < 256 * 256 * 256)
            {
                *((uint*)buffer) = (uint)v3;
                b |= 0x08;
                pos += 3;
                buffer += 3;
            }
            else if (v3 < 256 * 256 * 256 * 256L)
            {
                *((uint*)buffer) = (uint)v3;
                b |= 0x0C;
                pos += 4;
                buffer += 4;
            }
            else
            {
                throw new Exception("数字超出了范围！");
            }

            if (v4 < 256)
            {
                *buffer = (byte)(v4);
                pos += 1;
                buffer++;
            }
            else if (v4 < 256 * 256)
            {
                *((ushort*)buffer) = (ushort)v4;
                b |= 0x01;
                pos += 2;
                buffer += 2;
            }
            else if (v4 < 256 * 256 * 256)
            {
                *((uint*)buffer) = (uint)v4;
                b |= 0x02;
                pos += 3;
                buffer += 3;
            }
            else if (v4 < 256 * 256 * 256 * 256L)
            {
                *((uint*)buffer) = (uint)v4;
                b |= 0x03;
                pos += 4;
                buffer += 4;
            }
            else
            {
                throw new Exception("数字超出了范围！");
            }
            buffer[p] = b;
            return buffer;
        }

        //public unsafe static int[] GReadVIntFast1()
        //{
        //    //byte[] bytes = new byte[] { 45, 55, 14, 9, 7, 8, 65, 3, 5, 9, 6, 78, 21, 55, 69, 24, 34 };
        //    byte[] bytes = new byte[] { 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        //    int[] ints = new int[100];
        //    fixed(byte* pb = &bytes[0])
        //    {
        //        byte* pbb = pb;

        //        fixed(int* pdi = &ints[0])
        //        {
        //            int* pdii = pdi;
        //            *pdii = *(int*)(pbb + 1);
        //            pdii++;
        //            * pdii = *(int*)(pbb + 6);
        //        }
        //    }

        //    return ints;
        //}
    }
}
