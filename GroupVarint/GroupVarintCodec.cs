using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GroupVarint
{
    public class GroupVarintCodec
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

        #endregion

        #region 公共方法

        /// <summary>
        /// 获得数字需要使用多少个字节的查询方法，没有使用条件判断逻辑
        /// 主要是为了提高性能，据测试，无论在那种数据集下，性能都比条件分支快，大约1亿条记录调用耗时250ms
        /// </summary>
        /// <param name="v"></param>
        /// <param name="codeIdx"></param>
        /// <param name="lenId2"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static int GetByteSize(uint v, int* codeIdx, int* lenId2)
        {
            int n = (codeIdx[v >> 24] << 3);
            n += (codeIdx[(byte)(v >> 16)] << 2);
            n += (codeIdx[(byte)(v >> 8)] << 1);
            n += (codeIdx[(byte)(v)]);
            return lenId2[n];
        }

        public unsafe static int Decode(byte[] buffer, uint[] array, int size)
        {
            return 0;
        }

        /// <summary>
        /// 将二进制数据解码成uint数组，使用了指针，注意做好越界判断和预留好足够的空间
        /// 该方法解码能达到每秒解码5亿个uint数字
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="bpos"></param>
        /// <param name="bsize"></param>
        /// <param name="array"></param>
        /// <param name="ap"></param>
        /// <returns></returns>
        public unsafe static int Decode(byte[] buffer, int bpos, int bsize, uint[] array, ref int ap)
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
                        switch (*bs)
                        {
                            case 0:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *(bs + 2);
                                    *(aps + 2) = *(bs + 3);
                                    *(aps + 3) = *(bs + 4);
                                    bs += 5;
                                    bpos += 5;
                                    break;
                                }
                            case 1:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *(bs + 2);
                                    *(aps + 2) = *(bs + 3);
                                    *(aps + 3) = *((ushort*)(bs + 4));
                                    bs += 6;
                                    bpos += 6;
                                    break;
                                }
                            case 2:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *(bs + 2);
                                    *(aps + 2) = *(bs + 3);
                                    *(aps + 3) = (*((uint*)(bs + 3)) >> 8);
                                    bs += 7;
                                    bpos += 7;
                                    break;
                                }
                            case 3:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *(bs + 2);
                                    *(aps + 2) = *(bs + 3);
                                    *(aps + 3) = *((uint*)(bs + 4));
                                    bs += 8;
                                    bpos += 8;
                                    break;
                                }
                            case 4:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *(bs + 2);
                                    *(aps + 2) = *((ushort*)(bs + 3));
                                    *(aps + 3) = *(bs + 5);
                                    bs += 6;
                                    bpos += 6;
                                    break;
                                }
                            case 5:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *(bs + 2);
                                    *(aps + 2) = *((ushort*)(bs + 3));
                                    *(aps + 3) = *((ushort*)(bs + 5));
                                    bs += 7;
                                    bpos += 7;
                                    break;
                                }
                            case 6:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *(bs + 2);
                                    *(aps + 2) = *((ushort*)(bs + 3));
                                    *(aps + 3) = (*((uint*)(bs + 4)) >> 8);
                                    bs += 8;
                                    bpos += 8;
                                    break;
                                }
                            case 7:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *(bs + 2);
                                    *(aps + 2) = *((ushort*)(bs + 3));
                                    *(aps + 3) = *((uint*)(bs + 5));
                                    bs += 9;
                                    bpos += 9;
                                    break;
                                }
                            case 8:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *(bs + 2);
                                    *(aps + 2) = (*((uint*)(bs + 2)) >> 8);
                                    *(aps + 3) = *(bs + 6);
                                    bs += 7;
                                    bpos += 7;
                                    break;
                                }
                            case 9:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *(bs + 2);
                                    *(aps + 2) = (*((uint*)(bs + 2)) >> 8);
                                    *(aps + 3) = *((ushort*)(bs + 6));
                                    bs += 8;
                                    bpos += 8;
                                    break;
                                }
                            case 10:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *(bs + 2);
                                    *(aps + 2) = (*((uint*)(bs + 2)) >> 8);
                                    *(aps + 3) = (*((uint*)(bs + 5)) >> 8);
                                    bs += 9;
                                    bpos += 9;
                                    break;
                                }
                            case 11:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *(bs + 2);
                                    *(aps + 2) = (*((uint*)(bs + 2)) >> 8);
                                    *(aps + 3) = *((uint*)(bs + 6));
                                    bs += 10;
                                    bpos += 10;
                                    break;
                                }
                            case 12:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *(bs + 2);
                                    *(aps + 2) = *((uint*)(bs + 3));
                                    *(aps + 3) = *(bs + 7);
                                    bs += 8;
                                    bpos += 8;
                                    break;
                                }
                            case 13:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *(bs + 2);
                                    *(aps + 2) = *((uint*)(bs + 3));
                                    *(aps + 3) = *((ushort*)(bs + 7));
                                    bs += 9;
                                    bpos += 9;
                                    break;
                                }
                            case 14:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *(bs + 2);
                                    *(aps + 2) = *((uint*)(bs + 3));
                                    *(aps + 3) = (*((uint*)(bs + 6)) >> 8);
                                    bs += 10;
                                    bpos += 10;
                                    break;
                                }
                            case 15:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *(bs + 2);
                                    *(aps + 2) = *((uint*)(bs + 3));
                                    *(aps + 3) = *((uint*)(bs + 7));
                                    bs += 11;
                                    bpos += 11;
                                    break;
                                }
                            case 16:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *((ushort*)(bs + 2));
                                    *(aps + 2) = *(bs + 4);
                                    *(aps + 3) = *(bs + 5);
                                    bs += 6;
                                    bpos += 6;
                                    break;
                                }
                            case 17:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *((ushort*)(bs + 2));
                                    *(aps + 2) = *(bs + 4);
                                    *(aps + 3) = *((ushort*)(bs + 5));
                                    bs += 7;
                                    bpos += 7;
                                    break;
                                }
                            case 18:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *((ushort*)(bs + 2));
                                    *(aps + 2) = *(bs + 4);
                                    *(aps + 3) = (*((uint*)(bs + 4)) >> 8);
                                    bs += 8;
                                    bpos += 8;
                                    break;
                                }
                            case 19:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *((ushort*)(bs + 2));
                                    *(aps + 2) = *(bs + 4);
                                    *(aps + 3) = *((uint*)(bs + 5));
                                    bs += 9;
                                    bpos += 9;
                                    break;
                                }
                            case 20:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *((ushort*)(bs + 2));
                                    *(aps + 2) = *((ushort*)(bs + 4));
                                    *(aps + 3) = *(bs + 6);
                                    bs += 7;
                                    bpos += 7;
                                    break;
                                }
                            case 21:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *((ushort*)(bs + 2));
                                    *(aps + 2) = *((ushort*)(bs + 4));
                                    *(aps + 3) = *((ushort*)(bs + 6));
                                    bs += 8;
                                    bpos += 8;
                                    break;
                                }
                            case 22:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *((ushort*)(bs + 2));
                                    *(aps + 2) = *((ushort*)(bs + 4));
                                    *(aps + 3) = (*((uint*)(bs + 5)) >> 8);
                                    bs += 9;
                                    bpos += 9;
                                    break;
                                }
                            case 23:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *((ushort*)(bs + 2));
                                    *(aps + 2) = *((ushort*)(bs + 4));
                                    *(aps + 3) = *((uint*)(bs + 6));
                                    bs += 10;
                                    bpos += 10;
                                    break;
                                }
                            case 24:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *((ushort*)(bs + 2));
                                    *(aps + 2) = (*((uint*)(bs + 3)) >> 8);
                                    *(aps + 3) = *(bs + 7);
                                    bs += 8;
                                    bpos += 8;
                                    break;
                                }
                            case 25:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *((ushort*)(bs + 2));
                                    *(aps + 2) = (*((uint*)(bs + 3)) >> 8);
                                    *(aps + 3) = *((ushort*)(bs + 7));
                                    bs += 9;
                                    bpos += 9;
                                    break;
                                }
                            case 26:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *((ushort*)(bs + 2));
                                    *(aps + 2) = (*((uint*)(bs + 3)) >> 8);
                                    *(aps + 3) = (*((uint*)(bs + 6)) >> 8);
                                    bs += 10;
                                    bpos += 10;
                                    break;
                                }
                            case 27:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *((ushort*)(bs + 2));
                                    *(aps + 2) = (*((uint*)(bs + 3)) >> 8);
                                    *(aps + 3) = *((uint*)(bs + 7));
                                    bs += 11;
                                    bpos += 11;
                                    break;
                                }
                            case 28:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *((ushort*)(bs + 2));
                                    *(aps + 2) = *((uint*)(bs + 4));
                                    *(aps + 3) = *(bs + 8);
                                    bs += 9;
                                    bpos += 9;
                                    break;
                                }
                            case 29:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *((ushort*)(bs + 2));
                                    *(aps + 2) = *((uint*)(bs + 4));
                                    *(aps + 3) = *((ushort*)(bs + 8));
                                    bs += 10;
                                    bpos += 10;
                                    break;
                                }
                            case 30:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *((ushort*)(bs + 2));
                                    *(aps + 2) = *((uint*)(bs + 4));
                                    *(aps + 3) = (*((uint*)(bs + 7)) >> 8);
                                    bs += 11;
                                    bpos += 11;
                                    break;
                                }
                            case 31:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *((ushort*)(bs + 2));
                                    *(aps + 2) = *((uint*)(bs + 4));
                                    *(aps + 3) = *((uint*)(bs + 8));
                                    bs += 12;
                                    bpos += 12;
                                    break;
                                }
                            case 32:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = (*((uint*)(bs + 1)) >> 8);
                                    *(aps + 2) = *(bs + 5);
                                    *(aps + 3) = *(bs + 6);
                                    bs += 7;
                                    bpos += 7;
                                    break;
                                }
                            case 33:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = (*((uint*)(bs + 1)) >> 8);
                                    *(aps + 2) = *(bs + 5);
                                    *(aps + 3) = *((ushort*)(bs + 6));
                                    bs += 8;
                                    bpos += 8;
                                    break;
                                }
                            case 34:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = (*((uint*)(bs + 1)) >> 8);
                                    *(aps + 2) = *(bs + 5);
                                    *(aps + 3) = (*((uint*)(bs + 5)) >> 8);
                                    bs += 9;
                                    bpos += 9;
                                    break;
                                }
                            case 35:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = (*((uint*)(bs + 1)) >> 8);
                                    *(aps + 2) = *(bs + 5);
                                    *(aps + 3) = *((uint*)(bs + 6));
                                    bs += 10;
                                    bpos += 10;
                                    break;
                                }
                            case 36:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = (*((uint*)(bs + 1)) >> 8);
                                    *(aps + 2) = *((ushort*)(bs + 5));
                                    *(aps + 3) = *(bs + 7);
                                    bs += 8;
                                    bpos += 8;
                                    break;
                                }
                            case 37:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = (*((uint*)(bs + 1)) >> 8);
                                    *(aps + 2) = *((ushort*)(bs + 5));
                                    *(aps + 3) = *((ushort*)(bs + 7));
                                    bs += 9;
                                    bpos += 9;
                                    break;
                                }
                            case 38:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = (*((uint*)(bs + 1)) >> 8);
                                    *(aps + 2) = *((ushort*)(bs + 5));
                                    *(aps + 3) = (*((uint*)(bs + 6)) >> 8);
                                    bs += 10;
                                    bpos += 10;
                                    break;
                                }
                            case 39:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = (*((uint*)(bs + 1)) >> 8);
                                    *(aps + 2) = *((ushort*)(bs + 5));
                                    *(aps + 3) = *((uint*)(bs + 7));
                                    bs += 11;
                                    bpos += 11;
                                    break;
                                }
                            case 40:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = (*((uint*)(bs + 1)) >> 8);
                                    *(aps + 2) = (*((uint*)(bs + 4)) >> 8);
                                    *(aps + 3) = *(bs + 8);
                                    bs += 9;
                                    bpos += 9;
                                    break;
                                }
                            case 41:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = (*((uint*)(bs + 1)) >> 8);
                                    *(aps + 2) = (*((uint*)(bs + 4)) >> 8);
                                    *(aps + 3) = *((ushort*)(bs + 8));
                                    bs += 10;
                                    bpos += 10;
                                    break;
                                }
                            case 42:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = (*((uint*)(bs + 1)) >> 8);
                                    *(aps + 2) = (*((uint*)(bs + 4)) >> 8);
                                    *(aps + 3) = (*((uint*)(bs + 7)) >> 8);
                                    bs += 11;
                                    bpos += 11;
                                    break;
                                }
                            case 43:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = (*((uint*)(bs + 1)) >> 8);
                                    *(aps + 2) = (*((uint*)(bs + 4)) >> 8);
                                    *(aps + 3) = *((uint*)(bs + 8));
                                    bs += 12;
                                    bpos += 12;
                                    break;
                                }
                            case 44:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = (*((uint*)(bs + 1)) >> 8);
                                    *(aps + 2) = *((uint*)(bs + 5));
                                    *(aps + 3) = *(bs + 9);
                                    bs += 10;
                                    bpos += 10;
                                    break;
                                }
                            case 45:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = (*((uint*)(bs + 1)) >> 8);
                                    *(aps + 2) = *((uint*)(bs + 5));
                                    *(aps + 3) = *((ushort*)(bs + 9));
                                    bs += 11;
                                    bpos += 11;
                                    break;
                                }
                            case 46:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = (*((uint*)(bs + 1)) >> 8);
                                    *(aps + 2) = *((uint*)(bs + 5));
                                    *(aps + 3) = (*((uint*)(bs + 8)) >> 8);
                                    bs += 12;
                                    bpos += 12;
                                    break;
                                }
                            case 47:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = (*((uint*)(bs + 1)) >> 8);
                                    *(aps + 2) = *((uint*)(bs + 5));
                                    *(aps + 3) = *((uint*)(bs + 9));
                                    bs += 13;
                                    bpos += 13;
                                    break;
                                }
                            case 48:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *((uint*)(bs + 2));
                                    *(aps + 2) = *(bs + 6);
                                    *(aps + 3) = *(bs + 7);
                                    bs += 8;
                                    bpos += 8;
                                    break;
                                }
                            case 49:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *((uint*)(bs + 2));
                                    *(aps + 2) = *(bs + 6);
                                    *(aps + 3) = *((ushort*)(bs + 7));
                                    bs += 9;
                                    bpos += 9;
                                    break;
                                }
                            case 50:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *((uint*)(bs + 2));
                                    *(aps + 2) = *(bs + 6);
                                    *(aps + 3) = (*((uint*)(bs + 6)) >> 8);
                                    bs += 10;
                                    bpos += 10;
                                    break;
                                }
                            case 51:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *((uint*)(bs + 2));
                                    *(aps + 2) = *(bs + 6);
                                    *(aps + 3) = *((uint*)(bs + 7));
                                    bs += 11;
                                    bpos += 11;
                                    break;
                                }
                            case 52:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *((uint*)(bs + 2));
                                    *(aps + 2) = *((ushort*)(bs + 6));
                                    *(aps + 3) = *(bs + 8);
                                    bs += 9;
                                    bpos += 9;
                                    break;
                                }
                            case 53:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *((uint*)(bs + 2));
                                    *(aps + 2) = *((ushort*)(bs + 6));
                                    *(aps + 3) = *((ushort*)(bs + 8));
                                    bs += 10;
                                    bpos += 10;
                                    break;
                                }
                            case 54:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *((uint*)(bs + 2));
                                    *(aps + 2) = *((ushort*)(bs + 6));
                                    *(aps + 3) = (*((uint*)(bs + 7)) >> 8);
                                    bs += 11;
                                    bpos += 11;
                                    break;
                                }
                            case 55:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *((uint*)(bs + 2));
                                    *(aps + 2) = *((ushort*)(bs + 6));
                                    *(aps + 3) = *((uint*)(bs + 8));
                                    bs += 12;
                                    bpos += 12;
                                    break;
                                }
                            case 56:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *((uint*)(bs + 2));
                                    *(aps + 2) = (*((uint*)(bs + 5)) >> 8);
                                    *(aps + 3) = *(bs + 9);
                                    bs += 10;
                                    bpos += 10;
                                    break;
                                }
                            case 57:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *((uint*)(bs + 2));
                                    *(aps + 2) = (*((uint*)(bs + 5)) >> 8);
                                    *(aps + 3) = *((ushort*)(bs + 9));
                                    bs += 11;
                                    bpos += 11;
                                    break;
                                }
                            case 58:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *((uint*)(bs + 2));
                                    *(aps + 2) = (*((uint*)(bs + 5)) >> 8);
                                    *(aps + 3) = (*((uint*)(bs + 8)) >> 8);
                                    bs += 12;
                                    bpos += 12;
                                    break;
                                }
                            case 59:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *((uint*)(bs + 2));
                                    *(aps + 2) = (*((uint*)(bs + 5)) >> 8);
                                    *(aps + 3) = *((uint*)(bs + 9));
                                    bs += 13;
                                    bpos += 13;
                                    break;
                                }
                            case 60:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *((uint*)(bs + 2));
                                    *(aps + 2) = *((uint*)(bs + 6));
                                    *(aps + 3) = *(bs + 10);
                                    bs += 11;
                                    bpos += 11;
                                    break;
                                }
                            case 61:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *((uint*)(bs + 2));
                                    *(aps + 2) = *((uint*)(bs + 6));
                                    *(aps + 3) = *((ushort*)(bs + 10));
                                    bs += 12;
                                    bpos += 12;
                                    break;
                                }
                            case 62:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *((uint*)(bs + 2));
                                    *(aps + 2) = *((uint*)(bs + 6));
                                    *(aps + 3) = (*((uint*)(bs + 9)) >> 8);
                                    bs += 13;
                                    bpos += 13;
                                    break;
                                }
                            case 63:
                                {
                                    *aps = *(bs + 1);
                                    *(aps + 1) = *((uint*)(bs + 2));
                                    *(aps + 2) = *((uint*)(bs + 6));
                                    *(aps + 3) = *((uint*)(bs + 10));
                                    bs += 14;
                                    bpos += 14;
                                    break;
                                }
                            case 64:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *(bs + 3);
                                    *(aps + 2) = *(bs + 4);
                                    *(aps + 3) = *(bs + 5);
                                    bs += 6;
                                    bpos += 6;
                                    break;
                                }
                            case 65:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *(bs + 3);
                                    *(aps + 2) = *(bs + 4);
                                    *(aps + 3) = *((ushort*)(bs + 5));
                                    bs += 7;
                                    bpos += 7;
                                    break;
                                }
                            case 66:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *(bs + 3);
                                    *(aps + 2) = *(bs + 4);
                                    *(aps + 3) = (*((uint*)(bs + 4)) >> 8);
                                    bs += 8;
                                    bpos += 8;
                                    break;
                                }
                            case 67:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *(bs + 3);
                                    *(aps + 2) = *(bs + 4);
                                    *(aps + 3) = *((uint*)(bs + 5));
                                    bs += 9;
                                    bpos += 9;
                                    break;
                                }
                            case 68:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *(bs + 3);
                                    *(aps + 2) = *((ushort*)(bs + 4));
                                    *(aps + 3) = *(bs + 6);
                                    bs += 7;
                                    bpos += 7;
                                    break;
                                }
                            case 69:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *(bs + 3);
                                    *(aps + 2) = *((ushort*)(bs + 4));
                                    *(aps + 3) = *((ushort*)(bs + 6));
                                    bs += 8;
                                    bpos += 8;
                                    break;
                                }
                            case 70:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *(bs + 3);
                                    *(aps + 2) = *((ushort*)(bs + 4));
                                    *(aps + 3) = (*((uint*)(bs + 5)) >> 8);
                                    bs += 9;
                                    bpos += 9;
                                    break;
                                }
                            case 71:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *(bs + 3);
                                    *(aps + 2) = *((ushort*)(bs + 4));
                                    *(aps + 3) = *((uint*)(bs + 6));
                                    bs += 10;
                                    bpos += 10;
                                    break;
                                }
                            case 72:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *(bs + 3);
                                    *(aps + 2) = (*((uint*)(bs + 3)) >> 8);
                                    *(aps + 3) = *(bs + 7);
                                    bs += 8;
                                    bpos += 8;
                                    break;
                                }
                            case 73:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *(bs + 3);
                                    *(aps + 2) = (*((uint*)(bs + 3)) >> 8);
                                    *(aps + 3) = *((ushort*)(bs + 7));
                                    bs += 9;
                                    bpos += 9;
                                    break;
                                }
                            case 74:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *(bs + 3);
                                    *(aps + 2) = (*((uint*)(bs + 3)) >> 8);
                                    *(aps + 3) = (*((uint*)(bs + 6)) >> 8);
                                    bs += 10;
                                    bpos += 10;
                                    break;
                                }
                            case 75:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *(bs + 3);
                                    *(aps + 2) = (*((uint*)(bs + 3)) >> 8);
                                    *(aps + 3) = *((uint*)(bs + 7));
                                    bs += 11;
                                    bpos += 11;
                                    break;
                                }
                            case 76:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *(bs + 3);
                                    *(aps + 2) = *((uint*)(bs + 4));
                                    *(aps + 3) = *(bs + 8);
                                    bs += 9;
                                    bpos += 9;
                                    break;
                                }
                            case 77:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *(bs + 3);
                                    *(aps + 2) = *((uint*)(bs + 4));
                                    *(aps + 3) = *((ushort*)(bs + 8));
                                    bs += 10;
                                    bpos += 10;
                                    break;
                                }
                            case 78:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *(bs + 3);
                                    *(aps + 2) = *((uint*)(bs + 4));
                                    *(aps + 3) = (*((uint*)(bs + 7)) >> 8);
                                    bs += 11;
                                    bpos += 11;
                                    break;
                                }
                            case 79:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *(bs + 3);
                                    *(aps + 2) = *((uint*)(bs + 4));
                                    *(aps + 3) = *((uint*)(bs + 8));
                                    bs += 12;
                                    bpos += 12;
                                    break;
                                }
                            case 80:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *((ushort*)(bs + 3));
                                    *(aps + 2) = *(bs + 5);
                                    *(aps + 3) = *(bs + 6);
                                    bs += 7;
                                    bpos += 7;
                                    break;
                                }
                            case 81:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *((ushort*)(bs + 3));
                                    *(aps + 2) = *(bs + 5);
                                    *(aps + 3) = *((ushort*)(bs + 6));
                                    bs += 8;
                                    bpos += 8;
                                    break;
                                }
                            case 82:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *((ushort*)(bs + 3));
                                    *(aps + 2) = *(bs + 5);
                                    *(aps + 3) = (*((uint*)(bs + 5)) >> 8);
                                    bs += 9;
                                    bpos += 9;
                                    break;
                                }
                            case 83:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *((ushort*)(bs + 3));
                                    *(aps + 2) = *(bs + 5);
                                    *(aps + 3) = *((uint*)(bs + 6));
                                    bs += 10;
                                    bpos += 10;
                                    break;
                                }
                            case 84:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *((ushort*)(bs + 3));
                                    *(aps + 2) = *((ushort*)(bs + 5));
                                    *(aps + 3) = *(bs + 7);
                                    bs += 8;
                                    bpos += 8;
                                    break;
                                }
                            case 85:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *((ushort*)(bs + 3));
                                    *(aps + 2) = *((ushort*)(bs + 5));
                                    *(aps + 3) = *((ushort*)(bs + 7));
                                    bs += 9;
                                    bpos += 9;
                                    break;
                                }
                            case 86:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *((ushort*)(bs + 3));
                                    *(aps + 2) = *((ushort*)(bs + 5));
                                    *(aps + 3) = (*((uint*)(bs + 6)) >> 8);
                                    bs += 10;
                                    bpos += 10;
                                    break;
                                }
                            case 87:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *((ushort*)(bs + 3));
                                    *(aps + 2) = *((ushort*)(bs + 5));
                                    *(aps + 3) = *((uint*)(bs + 7));
                                    bs += 11;
                                    bpos += 11;
                                    break;
                                }
                            case 88:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *((ushort*)(bs + 3));
                                    *(aps + 2) = (*((uint*)(bs + 4)) >> 8);
                                    *(aps + 3) = *(bs + 8);
                                    bs += 9;
                                    bpos += 9;
                                    break;
                                }
                            case 89:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *((ushort*)(bs + 3));
                                    *(aps + 2) = (*((uint*)(bs + 4)) >> 8);
                                    *(aps + 3) = *((ushort*)(bs + 8));
                                    bs += 10;
                                    bpos += 10;
                                    break;
                                }
                            case 90:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *((ushort*)(bs + 3));
                                    *(aps + 2) = (*((uint*)(bs + 4)) >> 8);
                                    *(aps + 3) = (*((uint*)(bs + 7)) >> 8);
                                    bs += 11;
                                    bpos += 11;
                                    break;
                                }
                            case 91:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *((ushort*)(bs + 3));
                                    *(aps + 2) = (*((uint*)(bs + 4)) >> 8);
                                    *(aps + 3) = *((uint*)(bs + 8));
                                    bs += 12;
                                    bpos += 12;
                                    break;
                                }
                            case 92:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *((ushort*)(bs + 3));
                                    *(aps + 2) = *((uint*)(bs + 5));
                                    *(aps + 3) = *(bs + 9);
                                    bs += 10;
                                    bpos += 10;
                                    break;
                                }
                            case 93:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *((ushort*)(bs + 3));
                                    *(aps + 2) = *((uint*)(bs + 5));
                                    *(aps + 3) = *((ushort*)(bs + 9));
                                    bs += 11;
                                    bpos += 11;
                                    break;
                                }
                            case 94:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *((ushort*)(bs + 3));
                                    *(aps + 2) = *((uint*)(bs + 5));
                                    *(aps + 3) = (*((uint*)(bs + 8)) >> 8);
                                    bs += 12;
                                    bpos += 12;
                                    break;
                                }
                            case 95:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *((ushort*)(bs + 3));
                                    *(aps + 2) = *((uint*)(bs + 5));
                                    *(aps + 3) = *((uint*)(bs + 9));
                                    bs += 13;
                                    bpos += 13;
                                    break;
                                }
                            case 96:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = (*((uint*)(bs + 2)) >> 8);
                                    *(aps + 2) = *(bs + 6);
                                    *(aps + 3) = *(bs + 7);
                                    bs += 8;
                                    bpos += 8;
                                    break;
                                }
                            case 97:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = (*((uint*)(bs + 2)) >> 8);
                                    *(aps + 2) = *(bs + 6);
                                    *(aps + 3) = *((ushort*)(bs + 7));
                                    bs += 9;
                                    bpos += 9;
                                    break;
                                }
                            case 98:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = (*((uint*)(bs + 2)) >> 8);
                                    *(aps + 2) = *(bs + 6);
                                    *(aps + 3) = (*((uint*)(bs + 6)) >> 8);
                                    bs += 10;
                                    bpos += 10;
                                    break;
                                }
                            case 99:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = (*((uint*)(bs + 2)) >> 8);
                                    *(aps + 2) = *(bs + 6);
                                    *(aps + 3) = *((uint*)(bs + 7));
                                    bs += 11;
                                    bpos += 11;
                                    break;
                                }
                            case 100:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = (*((uint*)(bs + 2)) >> 8);
                                    *(aps + 2) = *((ushort*)(bs + 6));
                                    *(aps + 3) = *(bs + 8);
                                    bs += 9;
                                    bpos += 9;
                                    break;
                                }
                            case 101:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = (*((uint*)(bs + 2)) >> 8);
                                    *(aps + 2) = *((ushort*)(bs + 6));
                                    *(aps + 3) = *((ushort*)(bs + 8));
                                    bs += 10;
                                    bpos += 10;
                                    break;
                                }
                            case 102:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = (*((uint*)(bs + 2)) >> 8);
                                    *(aps + 2) = *((ushort*)(bs + 6));
                                    *(aps + 3) = (*((uint*)(bs + 7)) >> 8);
                                    bs += 11;
                                    bpos += 11;
                                    break;
                                }
                            case 103:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = (*((uint*)(bs + 2)) >> 8);
                                    *(aps + 2) = *((ushort*)(bs + 6));
                                    *(aps + 3) = *((uint*)(bs + 8));
                                    bs += 12;
                                    bpos += 12;
                                    break;
                                }
                            case 104:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = (*((uint*)(bs + 2)) >> 8);
                                    *(aps + 2) = (*((uint*)(bs + 5)) >> 8);
                                    *(aps + 3) = *(bs + 9);
                                    bs += 10;
                                    bpos += 10;
                                    break;
                                }
                            case 105:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = (*((uint*)(bs + 2)) >> 8);
                                    *(aps + 2) = (*((uint*)(bs + 5)) >> 8);
                                    *(aps + 3) = *((ushort*)(bs + 9));
                                    bs += 11;
                                    bpos += 11;
                                    break;
                                }
                            case 106:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = (*((uint*)(bs + 2)) >> 8);
                                    *(aps + 2) = (*((uint*)(bs + 5)) >> 8);
                                    *(aps + 3) = (*((uint*)(bs + 8)) >> 8);
                                    bs += 12;
                                    bpos += 12;
                                    break;
                                }
                            case 107:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = (*((uint*)(bs + 2)) >> 8);
                                    *(aps + 2) = (*((uint*)(bs + 5)) >> 8);
                                    *(aps + 3) = *((uint*)(bs + 9));
                                    bs += 13;
                                    bpos += 13;
                                    break;
                                }
                            case 108:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = (*((uint*)(bs + 2)) >> 8);
                                    *(aps + 2) = *((uint*)(bs + 6));
                                    *(aps + 3) = *(bs + 10);
                                    bs += 11;
                                    bpos += 11;
                                    break;
                                }
                            case 109:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = (*((uint*)(bs + 2)) >> 8);
                                    *(aps + 2) = *((uint*)(bs + 6));
                                    *(aps + 3) = *((ushort*)(bs + 10));
                                    bs += 12;
                                    bpos += 12;
                                    break;
                                }
                            case 110:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = (*((uint*)(bs + 2)) >> 8);
                                    *(aps + 2) = *((uint*)(bs + 6));
                                    *(aps + 3) = (*((uint*)(bs + 9)) >> 8);
                                    bs += 13;
                                    bpos += 13;
                                    break;
                                }
                            case 111:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = (*((uint*)(bs + 2)) >> 8);
                                    *(aps + 2) = *((uint*)(bs + 6));
                                    *(aps + 3) = *((uint*)(bs + 10));
                                    bs += 14;
                                    bpos += 14;
                                    break;
                                }
                            case 112:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *((uint*)(bs + 3));
                                    *(aps + 2) = *(bs + 7);
                                    *(aps + 3) = *(bs + 8);
                                    bs += 9;
                                    bpos += 9;
                                    break;
                                }
                            case 113:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *((uint*)(bs + 3));
                                    *(aps + 2) = *(bs + 7);
                                    *(aps + 3) = *((ushort*)(bs + 8));
                                    bs += 10;
                                    bpos += 10;
                                    break;
                                }
                            case 114:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *((uint*)(bs + 3));
                                    *(aps + 2) = *(bs + 7);
                                    *(aps + 3) = (*((uint*)(bs + 7)) >> 8);
                                    bs += 11;
                                    bpos += 11;
                                    break;
                                }
                            case 115:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *((uint*)(bs + 3));
                                    *(aps + 2) = *(bs + 7);
                                    *(aps + 3) = *((uint*)(bs + 8));
                                    bs += 12;
                                    bpos += 12;
                                    break;
                                }
                            case 116:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *((uint*)(bs + 3));
                                    *(aps + 2) = *((ushort*)(bs + 7));
                                    *(aps + 3) = *(bs + 9);
                                    bs += 10;
                                    bpos += 10;
                                    break;
                                }
                            case 117:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *((uint*)(bs + 3));
                                    *(aps + 2) = *((ushort*)(bs + 7));
                                    *(aps + 3) = *((ushort*)(bs + 9));
                                    bs += 11;
                                    bpos += 11;
                                    break;
                                }
                            case 118:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *((uint*)(bs + 3));
                                    *(aps + 2) = *((ushort*)(bs + 7));
                                    *(aps + 3) = (*((uint*)(bs + 8)) >> 8);
                                    bs += 12;
                                    bpos += 12;
                                    break;
                                }
                            case 119:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *((uint*)(bs + 3));
                                    *(aps + 2) = *((ushort*)(bs + 7));
                                    *(aps + 3) = *((uint*)(bs + 9));
                                    bs += 13;
                                    bpos += 13;
                                    break;
                                }
                            case 120:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *((uint*)(bs + 3));
                                    *(aps + 2) = (*((uint*)(bs + 6)) >> 8);
                                    *(aps + 3) = *(bs + 10);
                                    bs += 11;
                                    bpos += 11;
                                    break;
                                }
                            case 121:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *((uint*)(bs + 3));
                                    *(aps + 2) = (*((uint*)(bs + 6)) >> 8);
                                    *(aps + 3) = *((ushort*)(bs + 10));
                                    bs += 12;
                                    bpos += 12;
                                    break;
                                }
                            case 122:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *((uint*)(bs + 3));
                                    *(aps + 2) = (*((uint*)(bs + 6)) >> 8);
                                    *(aps + 3) = (*((uint*)(bs + 9)) >> 8);
                                    bs += 13;
                                    bpos += 13;
                                    break;
                                }
                            case 123:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *((uint*)(bs + 3));
                                    *(aps + 2) = (*((uint*)(bs + 6)) >> 8);
                                    *(aps + 3) = *((uint*)(bs + 10));
                                    bs += 14;
                                    bpos += 14;
                                    break;
                                }
                            case 124:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *((uint*)(bs + 3));
                                    *(aps + 2) = *((uint*)(bs + 7));
                                    *(aps + 3) = *(bs + 11);
                                    bs += 12;
                                    bpos += 12;
                                    break;
                                }
                            case 125:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *((uint*)(bs + 3));
                                    *(aps + 2) = *((uint*)(bs + 7));
                                    *(aps + 3) = *((ushort*)(bs + 11));
                                    bs += 13;
                                    bpos += 13;
                                    break;
                                }
                            case 126:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *((uint*)(bs + 3));
                                    *(aps + 2) = *((uint*)(bs + 7));
                                    *(aps + 3) = (*((uint*)(bs + 10)) >> 8);
                                    bs += 14;
                                    bpos += 14;
                                    break;
                                }
                            case 127:
                                {
                                    *aps = *((ushort*)(bs + 1));
                                    *(aps + 1) = *((uint*)(bs + 3));
                                    *(aps + 2) = *((uint*)(bs + 7));
                                    *(aps + 3) = *((uint*)(bs + 11));
                                    bs += 15;
                                    bpos += 15;
                                    break;
                                }
                            case 128:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *(bs + 4);
                                    *(aps + 2) = *(bs + 5);
                                    *(aps + 3) = *(bs + 6);
                                    bs += 7;
                                    bpos += 7;
                                    break;
                                }
                            case 129:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *(bs + 4);
                                    *(aps + 2) = *(bs + 5);
                                    *(aps + 3) = *((ushort*)(bs + 6));
                                    bs += 8;
                                    bpos += 8;
                                    break;
                                }
                            case 130:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *(bs + 4);
                                    *(aps + 2) = *(bs + 5);
                                    *(aps + 3) = (*((uint*)(bs + 5)) >> 8);
                                    bs += 9;
                                    bpos += 9;
                                    break;
                                }
                            case 131:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *(bs + 4);
                                    *(aps + 2) = *(bs + 5);
                                    *(aps + 3) = *((uint*)(bs + 6));
                                    bs += 10;
                                    bpos += 10;
                                    break;
                                }
                            case 132:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *(bs + 4);
                                    *(aps + 2) = *((ushort*)(bs + 5));
                                    *(aps + 3) = *(bs + 7);
                                    bs += 8;
                                    bpos += 8;
                                    break;
                                }
                            case 133:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *(bs + 4);
                                    *(aps + 2) = *((ushort*)(bs + 5));
                                    *(aps + 3) = *((ushort*)(bs + 7));
                                    bs += 9;
                                    bpos += 9;
                                    break;
                                }
                            case 134:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *(bs + 4);
                                    *(aps + 2) = *((ushort*)(bs + 5));
                                    *(aps + 3) = (*((uint*)(bs + 6)) >> 8);
                                    bs += 10;
                                    bpos += 10;
                                    break;
                                }
                            case 135:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *(bs + 4);
                                    *(aps + 2) = *((ushort*)(bs + 5));
                                    *(aps + 3) = *((uint*)(bs + 7));
                                    bs += 11;
                                    bpos += 11;
                                    break;
                                }
                            case 136:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *(bs + 4);
                                    *(aps + 2) = (*((uint*)(bs + 4)) >> 8);
                                    *(aps + 3) = *(bs + 8);
                                    bs += 9;
                                    bpos += 9;
                                    break;
                                }
                            case 137:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *(bs + 4);
                                    *(aps + 2) = (*((uint*)(bs + 4)) >> 8);
                                    *(aps + 3) = *((ushort*)(bs + 8));
                                    bs += 10;
                                    bpos += 10;
                                    break;
                                }
                            case 138:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *(bs + 4);
                                    *(aps + 2) = (*((uint*)(bs + 4)) >> 8);
                                    *(aps + 3) = (*((uint*)(bs + 7)) >> 8);
                                    bs += 11;
                                    bpos += 11;
                                    break;
                                }
                            case 139:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *(bs + 4);
                                    *(aps + 2) = (*((uint*)(bs + 4)) >> 8);
                                    *(aps + 3) = *((uint*)(bs + 8));
                                    bs += 12;
                                    bpos += 12;
                                    break;
                                }
                            case 140:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *(bs + 4);
                                    *(aps + 2) = *((uint*)(bs + 5));
                                    *(aps + 3) = *(bs + 9);
                                    bs += 10;
                                    bpos += 10;
                                    break;
                                }
                            case 141:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *(bs + 4);
                                    *(aps + 2) = *((uint*)(bs + 5));
                                    *(aps + 3) = *((ushort*)(bs + 9));
                                    bs += 11;
                                    bpos += 11;
                                    break;
                                }
                            case 142:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *(bs + 4);
                                    *(aps + 2) = *((uint*)(bs + 5));
                                    *(aps + 3) = (*((uint*)(bs + 8)) >> 8);
                                    bs += 12;
                                    bpos += 12;
                                    break;
                                }
                            case 143:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *(bs + 4);
                                    *(aps + 2) = *((uint*)(bs + 5));
                                    *(aps + 3) = *((uint*)(bs + 9));
                                    bs += 13;
                                    bpos += 13;
                                    break;
                                }
                            case 144:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *((ushort*)(bs + 4));
                                    *(aps + 2) = *(bs + 6);
                                    *(aps + 3) = *(bs + 7);
                                    bs += 8;
                                    bpos += 8;
                                    break;
                                }
                            case 145:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *((ushort*)(bs + 4));
                                    *(aps + 2) = *(bs + 6);
                                    *(aps + 3) = *((ushort*)(bs + 7));
                                    bs += 9;
                                    bpos += 9;
                                    break;
                                }
                            case 146:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *((ushort*)(bs + 4));
                                    *(aps + 2) = *(bs + 6);
                                    *(aps + 3) = (*((uint*)(bs + 6)) >> 8);
                                    bs += 10;
                                    bpos += 10;
                                    break;
                                }
                            case 147:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *((ushort*)(bs + 4));
                                    *(aps + 2) = *(bs + 6);
                                    *(aps + 3) = *((uint*)(bs + 7));
                                    bs += 11;
                                    bpos += 11;
                                    break;
                                }
                            case 148:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *((ushort*)(bs + 4));
                                    *(aps + 2) = *((ushort*)(bs + 6));
                                    *(aps + 3) = *(bs + 8);
                                    bs += 9;
                                    bpos += 9;
                                    break;
                                }
                            case 149:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *((ushort*)(bs + 4));
                                    *(aps + 2) = *((ushort*)(bs + 6));
                                    *(aps + 3) = *((ushort*)(bs + 8));
                                    bs += 10;
                                    bpos += 10;
                                    break;
                                }
                            case 150:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *((ushort*)(bs + 4));
                                    *(aps + 2) = *((ushort*)(bs + 6));
                                    *(aps + 3) = (*((uint*)(bs + 7)) >> 8);
                                    bs += 11;
                                    bpos += 11;
                                    break;
                                }
                            case 151:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *((ushort*)(bs + 4));
                                    *(aps + 2) = *((ushort*)(bs + 6));
                                    *(aps + 3) = *((uint*)(bs + 8));
                                    bs += 12;
                                    bpos += 12;
                                    break;
                                }
                            case 152:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *((ushort*)(bs + 4));
                                    *(aps + 2) = (*((uint*)(bs + 5)) >> 8);
                                    *(aps + 3) = *(bs + 9);
                                    bs += 10;
                                    bpos += 10;
                                    break;
                                }
                            case 153:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *((ushort*)(bs + 4));
                                    *(aps + 2) = (*((uint*)(bs + 5)) >> 8);
                                    *(aps + 3) = *((ushort*)(bs + 9));
                                    bs += 11;
                                    bpos += 11;
                                    break;
                                }
                            case 154:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *((ushort*)(bs + 4));
                                    *(aps + 2) = (*((uint*)(bs + 5)) >> 8);
                                    *(aps + 3) = (*((uint*)(bs + 8)) >> 8);
                                    bs += 12;
                                    bpos += 12;
                                    break;
                                }
                            case 155:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *((ushort*)(bs + 4));
                                    *(aps + 2) = (*((uint*)(bs + 5)) >> 8);
                                    *(aps + 3) = *((uint*)(bs + 9));
                                    bs += 13;
                                    bpos += 13;
                                    break;
                                }
                            case 156:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *((ushort*)(bs + 4));
                                    *(aps + 2) = *((uint*)(bs + 6));
                                    *(aps + 3) = *(bs + 10);
                                    bs += 11;
                                    bpos += 11;
                                    break;
                                }
                            case 157:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *((ushort*)(bs + 4));
                                    *(aps + 2) = *((uint*)(bs + 6));
                                    *(aps + 3) = *((ushort*)(bs + 10));
                                    bs += 12;
                                    bpos += 12;
                                    break;
                                }
                            case 158:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *((ushort*)(bs + 4));
                                    *(aps + 2) = *((uint*)(bs + 6));
                                    *(aps + 3) = (*((uint*)(bs + 9)) >> 8);
                                    bs += 13;
                                    bpos += 13;
                                    break;
                                }
                            case 159:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *((ushort*)(bs + 4));
                                    *(aps + 2) = *((uint*)(bs + 6));
                                    *(aps + 3) = *((uint*)(bs + 10));
                                    bs += 14;
                                    bpos += 14;
                                    break;
                                }
                            case 160:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = (*((uint*)(bs + 3)) >> 8);
                                    *(aps + 2) = *(bs + 7);
                                    *(aps + 3) = *(bs + 8);
                                    bs += 9;
                                    bpos += 9;
                                    break;
                                }
                            case 161:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = (*((uint*)(bs + 3)) >> 8);
                                    *(aps + 2) = *(bs + 7);
                                    *(aps + 3) = *((ushort*)(bs + 8));
                                    bs += 10;
                                    bpos += 10;
                                    break;
                                }
                            case 162:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = (*((uint*)(bs + 3)) >> 8);
                                    *(aps + 2) = *(bs + 7);
                                    *(aps + 3) = (*((uint*)(bs + 7)) >> 8);
                                    bs += 11;
                                    bpos += 11;
                                    break;
                                }
                            case 163:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = (*((uint*)(bs + 3)) >> 8);
                                    *(aps + 2) = *(bs + 7);
                                    *(aps + 3) = *((uint*)(bs + 8));
                                    bs += 12;
                                    bpos += 12;
                                    break;
                                }
                            case 164:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = (*((uint*)(bs + 3)) >> 8);
                                    *(aps + 2) = *((ushort*)(bs + 7));
                                    *(aps + 3) = *(bs + 9);
                                    bs += 10;
                                    bpos += 10;
                                    break;
                                }
                            case 165:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = (*((uint*)(bs + 3)) >> 8);
                                    *(aps + 2) = *((ushort*)(bs + 7));
                                    *(aps + 3) = *((ushort*)(bs + 9));
                                    bs += 11;
                                    bpos += 11;
                                    break;
                                }
                            case 166:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = (*((uint*)(bs + 3)) >> 8);
                                    *(aps + 2) = *((ushort*)(bs + 7));
                                    *(aps + 3) = (*((uint*)(bs + 8)) >> 8);
                                    bs += 12;
                                    bpos += 12;
                                    break;
                                }
                            case 167:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = (*((uint*)(bs + 3)) >> 8);
                                    *(aps + 2) = *((ushort*)(bs + 7));
                                    *(aps + 3) = *((uint*)(bs + 9));
                                    bs += 13;
                                    bpos += 13;
                                    break;
                                }
                            case 168:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = (*((uint*)(bs + 3)) >> 8);
                                    *(aps + 2) = (*((uint*)(bs + 6)) >> 8);
                                    *(aps + 3) = *(bs + 10);
                                    bs += 11;
                                    bpos += 11;
                                    break;
                                }
                            case 169:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = (*((uint*)(bs + 3)) >> 8);
                                    *(aps + 2) = (*((uint*)(bs + 6)) >> 8);
                                    *(aps + 3) = *((ushort*)(bs + 10));
                                    bs += 12;
                                    bpos += 12;
                                    break;
                                }
                            case 170:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = (*((uint*)(bs + 3)) >> 8);
                                    *(aps + 2) = (*((uint*)(bs + 6)) >> 8);
                                    *(aps + 3) = (*((uint*)(bs + 9)) >> 8);
                                    bs += 13;
                                    bpos += 13;
                                    break;
                                }
                            case 171:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = (*((uint*)(bs + 3)) >> 8);
                                    *(aps + 2) = (*((uint*)(bs + 6)) >> 8);
                                    *(aps + 3) = *((uint*)(bs + 10));
                                    bs += 14;
                                    bpos += 14;
                                    break;
                                }
                            case 172:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = (*((uint*)(bs + 3)) >> 8);
                                    *(aps + 2) = *((uint*)(bs + 7));
                                    *(aps + 3) = *(bs + 11);
                                    bs += 12;
                                    bpos += 12;
                                    break;
                                }
                            case 173:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = (*((uint*)(bs + 3)) >> 8);
                                    *(aps + 2) = *((uint*)(bs + 7));
                                    *(aps + 3) = *((ushort*)(bs + 11));
                                    bs += 13;
                                    bpos += 13;
                                    break;
                                }
                            case 174:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = (*((uint*)(bs + 3)) >> 8);
                                    *(aps + 2) = *((uint*)(bs + 7));
                                    *(aps + 3) = (*((uint*)(bs + 10)) >> 8);
                                    bs += 14;
                                    bpos += 14;
                                    break;
                                }
                            case 175:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = (*((uint*)(bs + 3)) >> 8);
                                    *(aps + 2) = *((uint*)(bs + 7));
                                    *(aps + 3) = *((uint*)(bs + 11));
                                    bs += 15;
                                    bpos += 15;
                                    break;
                                }
                            case 176:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *((uint*)(bs + 4));
                                    *(aps + 2) = *(bs + 8);
                                    *(aps + 3) = *(bs + 9);
                                    bs += 10;
                                    bpos += 10;
                                    break;
                                }
                            case 177:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *((uint*)(bs + 4));
                                    *(aps + 2) = *(bs + 8);
                                    *(aps + 3) = *((ushort*)(bs + 9));
                                    bs += 11;
                                    bpos += 11;
                                    break;
                                }
                            case 178:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *((uint*)(bs + 4));
                                    *(aps + 2) = *(bs + 8);
                                    *(aps + 3) = (*((uint*)(bs + 8)) >> 8);
                                    bs += 12;
                                    bpos += 12;
                                    break;
                                }
                            case 179:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *((uint*)(bs + 4));
                                    *(aps + 2) = *(bs + 8);
                                    *(aps + 3) = *((uint*)(bs + 9));
                                    bs += 13;
                                    bpos += 13;
                                    break;
                                }
                            case 180:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *((uint*)(bs + 4));
                                    *(aps + 2) = *((ushort*)(bs + 8));
                                    *(aps + 3) = *(bs + 10);
                                    bs += 11;
                                    bpos += 11;
                                    break;
                                }
                            case 181:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *((uint*)(bs + 4));
                                    *(aps + 2) = *((ushort*)(bs + 8));
                                    *(aps + 3) = *((ushort*)(bs + 10));
                                    bs += 12;
                                    bpos += 12;
                                    break;
                                }
                            case 182:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *((uint*)(bs + 4));
                                    *(aps + 2) = *((ushort*)(bs + 8));
                                    *(aps + 3) = (*((uint*)(bs + 9)) >> 8);
                                    bs += 13;
                                    bpos += 13;
                                    break;
                                }
                            case 183:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *((uint*)(bs + 4));
                                    *(aps + 2) = *((ushort*)(bs + 8));
                                    *(aps + 3) = *((uint*)(bs + 10));
                                    bs += 14;
                                    bpos += 14;
                                    break;
                                }
                            case 184:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *((uint*)(bs + 4));
                                    *(aps + 2) = (*((uint*)(bs + 7)) >> 8);
                                    *(aps + 3) = *(bs + 11);
                                    bs += 12;
                                    bpos += 12;
                                    break;
                                }
                            case 185:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *((uint*)(bs + 4));
                                    *(aps + 2) = (*((uint*)(bs + 7)) >> 8);
                                    *(aps + 3) = *((ushort*)(bs + 11));
                                    bs += 13;
                                    bpos += 13;
                                    break;
                                }
                            case 186:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *((uint*)(bs + 4));
                                    *(aps + 2) = (*((uint*)(bs + 7)) >> 8);
                                    *(aps + 3) = (*((uint*)(bs + 10)) >> 8);
                                    bs += 14;
                                    bpos += 14;
                                    break;
                                }
                            case 187:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *((uint*)(bs + 4));
                                    *(aps + 2) = (*((uint*)(bs + 7)) >> 8);
                                    *(aps + 3) = *((uint*)(bs + 11));
                                    bs += 15;
                                    bpos += 15;
                                    break;
                                }
                            case 188:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *((uint*)(bs + 4));
                                    *(aps + 2) = *((uint*)(bs + 8));
                                    *(aps + 3) = *(bs + 12);
                                    bs += 13;
                                    bpos += 13;
                                    break;
                                }
                            case 189:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *((uint*)(bs + 4));
                                    *(aps + 2) = *((uint*)(bs + 8));
                                    *(aps + 3) = *((ushort*)(bs + 12));
                                    bs += 14;
                                    bpos += 14;
                                    break;
                                }
                            case 190:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *((uint*)(bs + 4));
                                    *(aps + 2) = *((uint*)(bs + 8));
                                    *(aps + 3) = (*((uint*)(bs + 11)) >> 8);
                                    bs += 15;
                                    bpos += 15;
                                    break;
                                }
                            case 191:
                                {
                                    *aps = (*((uint*)(bs)) >> 8);
                                    *(aps + 1) = *((uint*)(bs + 4));
                                    *(aps + 2) = *((uint*)(bs + 8));
                                    *(aps + 3) = *((uint*)(bs + 12));
                                    bs += 16;
                                    bpos += 16;
                                    break;
                                }
                            case 192:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *(bs + 5);
                                    *(aps + 2) = *(bs + 6);
                                    *(aps + 3) = *(bs + 7);
                                    bs += 8;
                                    bpos += 8;
                                    break;
                                }
                            case 193:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *(bs + 5);
                                    *(aps + 2) = *(bs + 6);
                                    *(aps + 3) = *((ushort*)(bs + 7));
                                    bs += 9;
                                    bpos += 9;
                                    break;
                                }
                            case 194:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *(bs + 5);
                                    *(aps + 2) = *(bs + 6);
                                    *(aps + 3) = (*((uint*)(bs + 6)) >> 8);
                                    bs += 10;
                                    bpos += 10;
                                    break;
                                }
                            case 195:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *(bs + 5);
                                    *(aps + 2) = *(bs + 6);
                                    *(aps + 3) = *((uint*)(bs + 7));
                                    bs += 11;
                                    bpos += 11;
                                    break;
                                }
                            case 196:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *(bs + 5);
                                    *(aps + 2) = *((ushort*)(bs + 6));
                                    *(aps + 3) = *(bs + 8);
                                    bs += 9;
                                    bpos += 9;
                                    break;
                                }
                            case 197:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *(bs + 5);
                                    *(aps + 2) = *((ushort*)(bs + 6));
                                    *(aps + 3) = *((ushort*)(bs + 8));
                                    bs += 10;
                                    bpos += 10;
                                    break;
                                }
                            case 198:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *(bs + 5);
                                    *(aps + 2) = *((ushort*)(bs + 6));
                                    *(aps + 3) = (*((uint*)(bs + 7)) >> 8);
                                    bs += 11;
                                    bpos += 11;
                                    break;
                                }
                            case 199:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *(bs + 5);
                                    *(aps + 2) = *((ushort*)(bs + 6));
                                    *(aps + 3) = *((uint*)(bs + 8));
                                    bs += 12;
                                    bpos += 12;
                                    break;
                                }
                            case 200:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *(bs + 5);
                                    *(aps + 2) = (*((uint*)(bs + 5)) >> 8);
                                    *(aps + 3) = *(bs + 9);
                                    bs += 10;
                                    bpos += 10;
                                    break;
                                }
                            case 201:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *(bs + 5);
                                    *(aps + 2) = (*((uint*)(bs + 5)) >> 8);
                                    *(aps + 3) = *((ushort*)(bs + 9));
                                    bs += 11;
                                    bpos += 11;
                                    break;
                                }
                            case 202:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *(bs + 5);
                                    *(aps + 2) = (*((uint*)(bs + 5)) >> 8);
                                    *(aps + 3) = (*((uint*)(bs + 8)) >> 8);
                                    bs += 12;
                                    bpos += 12;
                                    break;
                                }
                            case 203:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *(bs + 5);
                                    *(aps + 2) = (*((uint*)(bs + 5)) >> 8);
                                    *(aps + 3) = *((uint*)(bs + 9));
                                    bs += 13;
                                    bpos += 13;
                                    break;
                                }
                            case 204:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *(bs + 5);
                                    *(aps + 2) = *((uint*)(bs + 6));
                                    *(aps + 3) = *(bs + 10);
                                    bs += 11;
                                    bpos += 11;
                                    break;
                                }
                            case 205:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *(bs + 5);
                                    *(aps + 2) = *((uint*)(bs + 6));
                                    *(aps + 3) = *((ushort*)(bs + 10));
                                    bs += 12;
                                    bpos += 12;
                                    break;
                                }
                            case 206:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *(bs + 5);
                                    *(aps + 2) = *((uint*)(bs + 6));
                                    *(aps + 3) = (*((uint*)(bs + 9)) >> 8);
                                    bs += 13;
                                    bpos += 13;
                                    break;
                                }
                            case 207:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *(bs + 5);
                                    *(aps + 2) = *((uint*)(bs + 6));
                                    *(aps + 3) = *((uint*)(bs + 10));
                                    bs += 14;
                                    bpos += 14;
                                    break;
                                }
                            case 208:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *((ushort*)(bs + 5));
                                    *(aps + 2) = *(bs + 7);
                                    *(aps + 3) = *(bs + 8);
                                    bs += 9;
                                    bpos += 9;
                                    break;
                                }
                            case 209:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *((ushort*)(bs + 5));
                                    *(aps + 2) = *(bs + 7);
                                    *(aps + 3) = *((ushort*)(bs + 8));
                                    bs += 10;
                                    bpos += 10;
                                    break;
                                }
                            case 210:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *((ushort*)(bs + 5));
                                    *(aps + 2) = *(bs + 7);
                                    *(aps + 3) = (*((uint*)(bs + 7)) >> 8);
                                    bs += 11;
                                    bpos += 11;
                                    break;
                                }
                            case 211:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *((ushort*)(bs + 5));
                                    *(aps + 2) = *(bs + 7);
                                    *(aps + 3) = *((uint*)(bs + 8));
                                    bs += 12;
                                    bpos += 12;
                                    break;
                                }
                            case 212:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *((ushort*)(bs + 5));
                                    *(aps + 2) = *((ushort*)(bs + 7));
                                    *(aps + 3) = *(bs + 9);
                                    bs += 10;
                                    bpos += 10;
                                    break;
                                }
                            case 213:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *((ushort*)(bs + 5));
                                    *(aps + 2) = *((ushort*)(bs + 7));
                                    *(aps + 3) = *((ushort*)(bs + 9));
                                    bs += 11;
                                    bpos += 11;
                                    break;
                                }
                            case 214:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *((ushort*)(bs + 5));
                                    *(aps + 2) = *((ushort*)(bs + 7));
                                    *(aps + 3) = (*((uint*)(bs + 8)) >> 8);
                                    bs += 12;
                                    bpos += 12;
                                    break;
                                }
                            case 215:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *((ushort*)(bs + 5));
                                    *(aps + 2) = *((ushort*)(bs + 7));
                                    *(aps + 3) = *((uint*)(bs + 9));
                                    bs += 13;
                                    bpos += 13;
                                    break;
                                }
                            case 216:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *((ushort*)(bs + 5));
                                    *(aps + 2) = (*((uint*)(bs + 6)) >> 8);
                                    *(aps + 3) = *(bs + 10);
                                    bs += 11;
                                    bpos += 11;
                                    break;
                                }
                            case 217:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *((ushort*)(bs + 5));
                                    *(aps + 2) = (*((uint*)(bs + 6)) >> 8);
                                    *(aps + 3) = *((ushort*)(bs + 10));
                                    bs += 12;
                                    bpos += 12;
                                    break;
                                }
                            case 218:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *((ushort*)(bs + 5));
                                    *(aps + 2) = (*((uint*)(bs + 6)) >> 8);
                                    *(aps + 3) = (*((uint*)(bs + 9)) >> 8);
                                    bs += 13;
                                    bpos += 13;
                                    break;
                                }
                            case 219:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *((ushort*)(bs + 5));
                                    *(aps + 2) = (*((uint*)(bs + 6)) >> 8);
                                    *(aps + 3) = *((uint*)(bs + 10));
                                    bs += 14;
                                    bpos += 14;
                                    break;
                                }
                            case 220:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *((ushort*)(bs + 5));
                                    *(aps + 2) = *((uint*)(bs + 7));
                                    *(aps + 3) = *(bs + 11);
                                    bs += 12;
                                    bpos += 12;
                                    break;
                                }
                            case 221:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *((ushort*)(bs + 5));
                                    *(aps + 2) = *((uint*)(bs + 7));
                                    *(aps + 3) = *((ushort*)(bs + 11));
                                    bs += 13;
                                    bpos += 13;
                                    break;
                                }
                            case 222:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *((ushort*)(bs + 5));
                                    *(aps + 2) = *((uint*)(bs + 7));
                                    *(aps + 3) = (*((uint*)(bs + 10)) >> 8);
                                    bs += 14;
                                    bpos += 14;
                                    break;
                                }
                            case 223:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *((ushort*)(bs + 5));
                                    *(aps + 2) = *((uint*)(bs + 7));
                                    *(aps + 3) = *((uint*)(bs + 11));
                                    bs += 15;
                                    bpos += 15;
                                    break;
                                }
                            case 224:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = (*((uint*)(bs + 4)) >> 8);
                                    *(aps + 2) = *(bs + 8);
                                    *(aps + 3) = *(bs + 9);
                                    bs += 10;
                                    bpos += 10;
                                    break;
                                }
                            case 225:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = (*((uint*)(bs + 4)) >> 8);
                                    *(aps + 2) = *(bs + 8);
                                    *(aps + 3) = *((ushort*)(bs + 9));
                                    bs += 11;
                                    bpos += 11;
                                    break;
                                }
                            case 226:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = (*((uint*)(bs + 4)) >> 8);
                                    *(aps + 2) = *(bs + 8);
                                    *(aps + 3) = (*((uint*)(bs + 8)) >> 8);
                                    bs += 12;
                                    bpos += 12;
                                    break;
                                }
                            case 227:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = (*((uint*)(bs + 4)) >> 8);
                                    *(aps + 2) = *(bs + 8);
                                    *(aps + 3) = *((uint*)(bs + 9));
                                    bs += 13;
                                    bpos += 13;
                                    break;
                                }
                            case 228:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = (*((uint*)(bs + 4)) >> 8);
                                    *(aps + 2) = *((ushort*)(bs + 8));
                                    *(aps + 3) = *(bs + 10);
                                    bs += 11;
                                    bpos += 11;
                                    break;
                                }
                            case 229:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = (*((uint*)(bs + 4)) >> 8);
                                    *(aps + 2) = *((ushort*)(bs + 8));
                                    *(aps + 3) = *((ushort*)(bs + 10));
                                    bs += 12;
                                    bpos += 12;
                                    break;
                                }
                            case 230:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = (*((uint*)(bs + 4)) >> 8);
                                    *(aps + 2) = *((ushort*)(bs + 8));
                                    *(aps + 3) = (*((uint*)(bs + 9)) >> 8);
                                    bs += 13;
                                    bpos += 13;
                                    break;
                                }
                            case 231:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = (*((uint*)(bs + 4)) >> 8);
                                    *(aps + 2) = *((ushort*)(bs + 8));
                                    *(aps + 3) = *((uint*)(bs + 10));
                                    bs += 14;
                                    bpos += 14;
                                    break;
                                }
                            case 232:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = (*((uint*)(bs + 4)) >> 8);
                                    *(aps + 2) = (*((uint*)(bs + 7)) >> 8);
                                    *(aps + 3) = *(bs + 11);
                                    bs += 12;
                                    bpos += 12;
                                    break;
                                }
                            case 233:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = (*((uint*)(bs + 4)) >> 8);
                                    *(aps + 2) = (*((uint*)(bs + 7)) >> 8);
                                    *(aps + 3) = *((ushort*)(bs + 11));
                                    bs += 13;
                                    bpos += 13;
                                    break;
                                }
                            case 234:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = (*((uint*)(bs + 4)) >> 8);
                                    *(aps + 2) = (*((uint*)(bs + 7)) >> 8);
                                    *(aps + 3) = (*((uint*)(bs + 10)) >> 8);
                                    bs += 14;
                                    bpos += 14;
                                    break;
                                }
                            case 235:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = (*((uint*)(bs + 4)) >> 8);
                                    *(aps + 2) = (*((uint*)(bs + 7)) >> 8);
                                    *(aps + 3) = *((uint*)(bs + 11));
                                    bs += 15;
                                    bpos += 15;
                                    break;
                                }
                            case 236:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = (*((uint*)(bs + 4)) >> 8);
                                    *(aps + 2) = *((uint*)(bs + 8));
                                    *(aps + 3) = *(bs + 12);
                                    bs += 13;
                                    bpos += 13;
                                    break;
                                }
                            case 237:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = (*((uint*)(bs + 4)) >> 8);
                                    *(aps + 2) = *((uint*)(bs + 8));
                                    *(aps + 3) = *((ushort*)(bs + 12));
                                    bs += 14;
                                    bpos += 14;
                                    break;
                                }
                            case 238:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = (*((uint*)(bs + 4)) >> 8);
                                    *(aps + 2) = *((uint*)(bs + 8));
                                    *(aps + 3) = (*((uint*)(bs + 11)) >> 8);
                                    bs += 15;
                                    bpos += 15;
                                    break;
                                }
                            case 239:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = (*((uint*)(bs + 4)) >> 8);
                                    *(aps + 2) = *((uint*)(bs + 8));
                                    *(aps + 3) = *((uint*)(bs + 12));
                                    bs += 16;
                                    bpos += 16;
                                    break;
                                }
                            case 240:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *((uint*)(bs + 5));
                                    *(aps + 2) = *(bs + 9);
                                    *(aps + 3) = *(bs + 10);
                                    bs += 11;
                                    bpos += 11;
                                    break;
                                }
                            case 241:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *((uint*)(bs + 5));
                                    *(aps + 2) = *(bs + 9);
                                    *(aps + 3) = *((ushort*)(bs + 10));
                                    bs += 12;
                                    bpos += 12;
                                    break;
                                }
                            case 242:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *((uint*)(bs + 5));
                                    *(aps + 2) = *(bs + 9);
                                    *(aps + 3) = (*((uint*)(bs + 9)) >> 8);
                                    bs += 13;
                                    bpos += 13;
                                    break;
                                }
                            case 243:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *((uint*)(bs + 5));
                                    *(aps + 2) = *(bs + 9);
                                    *(aps + 3) = *((uint*)(bs + 10));
                                    bs += 14;
                                    bpos += 14;
                                    break;
                                }
                            case 244:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *((uint*)(bs + 5));
                                    *(aps + 2) = *((ushort*)(bs + 9));
                                    *(aps + 3) = *(bs + 11);
                                    bs += 12;
                                    bpos += 12;
                                    break;
                                }
                            case 245:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *((uint*)(bs + 5));
                                    *(aps + 2) = *((ushort*)(bs + 9));
                                    *(aps + 3) = *((ushort*)(bs + 11));
                                    bs += 13;
                                    bpos += 13;
                                    break;
                                }
                            case 246:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *((uint*)(bs + 5));
                                    *(aps + 2) = *((ushort*)(bs + 9));
                                    *(aps + 3) = (*((uint*)(bs + 10)) >> 8);
                                    bs += 14;
                                    bpos += 14;
                                    break;
                                }
                            case 247:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *((uint*)(bs + 5));
                                    *(aps + 2) = *((ushort*)(bs + 9));
                                    *(aps + 3) = *((uint*)(bs + 11));
                                    bs += 15;
                                    bpos += 15;
                                    break;
                                }
                            case 248:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *((uint*)(bs + 5));
                                    *(aps + 2) = (*((uint*)(bs + 8)) >> 8);
                                    *(aps + 3) = *(bs + 12);
                                    bs += 13;
                                    bpos += 13;
                                    break;
                                }
                            case 249:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *((uint*)(bs + 5));
                                    *(aps + 2) = (*((uint*)(bs + 8)) >> 8);
                                    *(aps + 3) = *((ushort*)(bs + 12));
                                    bs += 14;
                                    bpos += 14;
                                    break;
                                }
                            case 250:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *((uint*)(bs + 5));
                                    *(aps + 2) = (*((uint*)(bs + 8)) >> 8);
                                    *(aps + 3) = (*((uint*)(bs + 11)) >> 8);
                                    bs += 15;
                                    bpos += 15;
                                    break;
                                }
                            case 251:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *((uint*)(bs + 5));
                                    *(aps + 2) = (*((uint*)(bs + 8)) >> 8);
                                    *(aps + 3) = *((uint*)(bs + 12));
                                    bs += 16;
                                    bpos += 16;
                                    break;
                                }
                            case 252:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *((uint*)(bs + 5));
                                    *(aps + 2) = *((uint*)(bs + 9));
                                    *(aps + 3) = *(bs + 13);
                                    bs += 14;
                                    bpos += 14;
                                    break;
                                }
                            case 253:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *((uint*)(bs + 5));
                                    *(aps + 2) = *((uint*)(bs + 9));
                                    *(aps + 3) = *((ushort*)(bs + 13));
                                    bs += 15;
                                    bpos += 15;
                                    break;
                                }
                            case 254:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *((uint*)(bs + 5));
                                    *(aps + 2) = *((uint*)(bs + 9));
                                    *(aps + 3) = (*((uint*)(bs + 12)) >> 8);
                                    bs += 16;
                                    bpos += 16;
                                    break;
                                }
                            case 255:
                                {
                                    *aps = *((uint*)(bs + 1));
                                    *(aps + 1) = *((uint*)(bs + 5));
                                    *(aps + 2) = *((uint*)(bs + 9));
                                    *(aps + 3) = *((uint*)(bs + 13));
                                    bs += 17;
                                    bpos += 17;
                                    break;
                                }
                            default:
                                break;
                        }


                        aps += 4;
                        n++;
                    }
                }
            }
            ap += (n << 2);
            return n << 2;

        }

        /// <summary>
        /// 对数字进行编码
        /// </summary>
        /// <param name="dst"></param>
        /// <param name="dstOffset"></param>
        /// <param name="src"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public unsafe static int Encode(byte[] dst, int dstOffset, uint[] src, int offset, int count)
        {
            int wsize = 0;
            fixed (byte* pd = &dst[dstOffset])
            {
                fixed (uint* ps = &src[offset])
                {
                    fixed (int* p1 = &codeIdx[0], p2 = &lenId2[0])
                    {
                        uint* psrc = ps;
                        byte* pdst = pd;
                        for (int i = 0; i < count; i += 4)
                        {
                            uint v1 = *psrc, v2 = *(psrc + 1), v3 = *(psrc + 2), v4 = *(psrc + 3);
                            int b = GetByteSize(v1, p1, p2) << 6 | GetByteSize(v2, p1, p2) << 4 | GetByteSize(v3, p1, p2) << 2 | GetByteSize(v4, p1, p2);
                            *pdst = (byte)b;
                            switch (b)
                            {
                                case 0:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *(pdst + 2) = (byte)(v2);
                                        *(pdst + 3) = (byte)(v3);
                                        *(pdst + 4) = (byte)(v4);
                                        pdst += 5;
                                        wsize += 5;
                                        break;
                                    }
                                case 1:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *(pdst + 2) = (byte)(v2);
                                        *(pdst + 3) = (byte)(v3);
                                        *((ushort*)(pdst + 4)) = (ushort)v4;
                                        pdst += 6;
                                        wsize += 6;
                                        break;
                                    }
                                case 2:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *(pdst + 2) = (byte)(v2);
                                        *(pdst + 3) = (byte)(v3);
                                        *((uint*)(pdst + 4)) = (uint)v4;
                                        pdst += 7;
                                        wsize += 7;
                                        break;
                                    }
                                case 3:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *(pdst + 2) = (byte)(v2);
                                        *(pdst + 3) = (byte)(v3);
                                        *((uint*)(pdst + 4)) = (uint)v4;
                                        pdst += 8;
                                        wsize += 8;
                                        break;
                                    }
                                case 4:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *(pdst + 2) = (byte)(v2);
                                        *((ushort*)(pdst + 3)) = (ushort)v3;
                                        *(pdst + 5) = (byte)(v4);
                                        pdst += 6;
                                        wsize += 6;
                                        break;
                                    }
                                case 5:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *(pdst + 2) = (byte)(v2);
                                        *((ushort*)(pdst + 3)) = (ushort)v3;
                                        *((ushort*)(pdst + 5)) = (ushort)v4;
                                        pdst += 7;
                                        wsize += 7;
                                        break;
                                    }
                                case 6:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *(pdst + 2) = (byte)(v2);
                                        *((ushort*)(pdst + 3)) = (ushort)v3;
                                        *((uint*)(pdst + 5)) = (uint)v4;
                                        pdst += 8;
                                        wsize += 8;
                                        break;
                                    }
                                case 7:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *(pdst + 2) = (byte)(v2);
                                        *((ushort*)(pdst + 3)) = (ushort)v3;
                                        *((uint*)(pdst + 5)) = (uint)v4;
                                        pdst += 9;
                                        wsize += 9;
                                        break;
                                    }
                                case 8:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *(pdst + 2) = (byte)(v2);
                                        *((uint*)(pdst + 3)) = (uint)v3;
                                        *(pdst + 6) = (byte)(v4);
                                        pdst += 7;
                                        wsize += 7;
                                        break;
                                    }
                                case 9:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *(pdst + 2) = (byte)(v2);
                                        *((uint*)(pdst + 3)) = (uint)v3;
                                        *((ushort*)(pdst + 6)) = (ushort)v4;
                                        pdst += 8;
                                        wsize += 8;
                                        break;
                                    }
                                case 10:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *(pdst + 2) = (byte)(v2);
                                        *((uint*)(pdst + 3)) = (uint)v3;
                                        *((uint*)(pdst + 6)) = (uint)v4;
                                        pdst += 9;
                                        wsize += 9;
                                        break;
                                    }
                                case 11:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *(pdst + 2) = (byte)(v2);
                                        *((uint*)(pdst + 3)) = (uint)v3;
                                        *((uint*)(pdst + 6)) = (uint)v4;
                                        pdst += 10;
                                        wsize += 10;
                                        break;
                                    }
                                case 12:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *(pdst + 2) = (byte)(v2);
                                        *((uint*)(pdst + 3)) = (uint)v3;
                                        *(pdst + 7) = (byte)(v4);
                                        pdst += 8;
                                        wsize += 8;
                                        break;
                                    }
                                case 13:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *(pdst + 2) = (byte)(v2);
                                        *((uint*)(pdst + 3)) = (uint)v3;
                                        *((ushort*)(pdst + 7)) = (ushort)v4;
                                        pdst += 9;
                                        wsize += 9;
                                        break;
                                    }
                                case 14:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *(pdst + 2) = (byte)(v2);
                                        *((uint*)(pdst + 3)) = (uint)v3;
                                        *((uint*)(pdst + 7)) = (uint)v4;
                                        pdst += 10;
                                        wsize += 10;
                                        break;
                                    }
                                case 15:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *(pdst + 2) = (byte)(v2);
                                        *((uint*)(pdst + 3)) = (uint)v3;
                                        *((uint*)(pdst + 7)) = (uint)v4;
                                        pdst += 11;
                                        wsize += 11;
                                        break;
                                    }
                                case 16:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((ushort*)(pdst + 2)) = (ushort)v2;
                                        *(pdst + 4) = (byte)(v3);
                                        *(pdst + 5) = (byte)(v4);
                                        pdst += 6;
                                        wsize += 6;
                                        break;
                                    }
                                case 17:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((ushort*)(pdst + 2)) = (ushort)v2;
                                        *(pdst + 4) = (byte)(v3);
                                        *((ushort*)(pdst + 5)) = (ushort)v4;
                                        pdst += 7;
                                        wsize += 7;
                                        break;
                                    }
                                case 18:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((ushort*)(pdst + 2)) = (ushort)v2;
                                        *(pdst + 4) = (byte)(v3);
                                        *((uint*)(pdst + 5)) = (uint)v4;
                                        pdst += 8;
                                        wsize += 8;
                                        break;
                                    }
                                case 19:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((ushort*)(pdst + 2)) = (ushort)v2;
                                        *(pdst + 4) = (byte)(v3);
                                        *((uint*)(pdst + 5)) = (uint)v4;
                                        pdst += 9;
                                        wsize += 9;
                                        break;
                                    }
                                case 20:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((ushort*)(pdst + 2)) = (ushort)v2;
                                        *((ushort*)(pdst + 4)) = (ushort)v3;
                                        *(pdst + 6) = (byte)(v4);
                                        pdst += 7;
                                        wsize += 7;
                                        break;
                                    }
                                case 21:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((ushort*)(pdst + 2)) = (ushort)v2;
                                        *((ushort*)(pdst + 4)) = (ushort)v3;
                                        *((ushort*)(pdst + 6)) = (ushort)v4;
                                        pdst += 8;
                                        wsize += 8;
                                        break;
                                    }
                                case 22:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((ushort*)(pdst + 2)) = (ushort)v2;
                                        *((ushort*)(pdst + 4)) = (ushort)v3;
                                        *((uint*)(pdst + 6)) = (uint)v4;
                                        pdst += 9;
                                        wsize += 9;
                                        break;
                                    }
                                case 23:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((ushort*)(pdst + 2)) = (ushort)v2;
                                        *((ushort*)(pdst + 4)) = (ushort)v3;
                                        *((uint*)(pdst + 6)) = (uint)v4;
                                        pdst += 10;
                                        wsize += 10;
                                        break;
                                    }
                                case 24:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((ushort*)(pdst + 2)) = (ushort)v2;
                                        *((uint*)(pdst + 4)) = (uint)v3;
                                        *(pdst + 7) = (byte)(v4);
                                        pdst += 8;
                                        wsize += 8;
                                        break;
                                    }
                                case 25:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((ushort*)(pdst + 2)) = (ushort)v2;
                                        *((uint*)(pdst + 4)) = (uint)v3;
                                        *((ushort*)(pdst + 7)) = (ushort)v4;
                                        pdst += 9;
                                        wsize += 9;
                                        break;
                                    }
                                case 26:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((ushort*)(pdst + 2)) = (ushort)v2;
                                        *((uint*)(pdst + 4)) = (uint)v3;
                                        *((uint*)(pdst + 7)) = (uint)v4;
                                        pdst += 10;
                                        wsize += 10;
                                        break;
                                    }
                                case 27:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((ushort*)(pdst + 2)) = (ushort)v2;
                                        *((uint*)(pdst + 4)) = (uint)v3;
                                        *((uint*)(pdst + 7)) = (uint)v4;
                                        pdst += 11;
                                        wsize += 11;
                                        break;
                                    }
                                case 28:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((ushort*)(pdst + 2)) = (ushort)v2;
                                        *((uint*)(pdst + 4)) = (uint)v3;
                                        *(pdst + 8) = (byte)(v4);
                                        pdst += 9;
                                        wsize += 9;
                                        break;
                                    }
                                case 29:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((ushort*)(pdst + 2)) = (ushort)v2;
                                        *((uint*)(pdst + 4)) = (uint)v3;
                                        *((ushort*)(pdst + 8)) = (ushort)v4;
                                        pdst += 10;
                                        wsize += 10;
                                        break;
                                    }
                                case 30:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((ushort*)(pdst + 2)) = (ushort)v2;
                                        *((uint*)(pdst + 4)) = (uint)v3;
                                        *((uint*)(pdst + 8)) = (uint)v4;
                                        pdst += 11;
                                        wsize += 11;
                                        break;
                                    }
                                case 31:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((ushort*)(pdst + 2)) = (ushort)v2;
                                        *((uint*)(pdst + 4)) = (uint)v3;
                                        *((uint*)(pdst + 8)) = (uint)v4;
                                        pdst += 12;
                                        wsize += 12;
                                        break;
                                    }
                                case 32:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((uint*)(pdst + 2)) = (uint)v2;
                                        *(pdst + 5) = (byte)(v3);
                                        *(pdst + 6) = (byte)(v4);
                                        pdst += 7;
                                        wsize += 7;
                                        break;
                                    }
                                case 33:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((uint*)(pdst + 2)) = (uint)v2;
                                        *(pdst + 5) = (byte)(v3);
                                        *((ushort*)(pdst + 6)) = (ushort)v4;
                                        pdst += 8;
                                        wsize += 8;
                                        break;
                                    }
                                case 34:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((uint*)(pdst + 2)) = (uint)v2;
                                        *(pdst + 5) = (byte)(v3);
                                        *((uint*)(pdst + 6)) = (uint)v4;
                                        pdst += 9;
                                        wsize += 9;
                                        break;
                                    }
                                case 35:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((uint*)(pdst + 2)) = (uint)v2;
                                        *(pdst + 5) = (byte)(v3);
                                        *((uint*)(pdst + 6)) = (uint)v4;
                                        pdst += 10;
                                        wsize += 10;
                                        break;
                                    }
                                case 36:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((uint*)(pdst + 2)) = (uint)v2;
                                        *((ushort*)(pdst + 5)) = (ushort)v3;
                                        *(pdst + 7) = (byte)(v4);
                                        pdst += 8;
                                        wsize += 8;
                                        break;
                                    }
                                case 37:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((uint*)(pdst + 2)) = (uint)v2;
                                        *((ushort*)(pdst + 5)) = (ushort)v3;
                                        *((ushort*)(pdst + 7)) = (ushort)v4;
                                        pdst += 9;
                                        wsize += 9;
                                        break;
                                    }
                                case 38:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((uint*)(pdst + 2)) = (uint)v2;
                                        *((ushort*)(pdst + 5)) = (ushort)v3;
                                        *((uint*)(pdst + 7)) = (uint)v4;
                                        pdst += 10;
                                        wsize += 10;
                                        break;
                                    }
                                case 39:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((uint*)(pdst + 2)) = (uint)v2;
                                        *((ushort*)(pdst + 5)) = (ushort)v3;
                                        *((uint*)(pdst + 7)) = (uint)v4;
                                        pdst += 11;
                                        wsize += 11;
                                        break;
                                    }
                                case 40:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((uint*)(pdst + 2)) = (uint)v2;
                                        *((uint*)(pdst + 5)) = (uint)v3;
                                        *(pdst + 8) = (byte)(v4);
                                        pdst += 9;
                                        wsize += 9;
                                        break;
                                    }
                                case 41:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((uint*)(pdst + 2)) = (uint)v2;
                                        *((uint*)(pdst + 5)) = (uint)v3;
                                        *((ushort*)(pdst + 8)) = (ushort)v4;
                                        pdst += 10;
                                        wsize += 10;
                                        break;
                                    }
                                case 42:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((uint*)(pdst + 2)) = (uint)v2;
                                        *((uint*)(pdst + 5)) = (uint)v3;
                                        *((uint*)(pdst + 8)) = (uint)v4;
                                        pdst += 11;
                                        wsize += 11;
                                        break;
                                    }
                                case 43:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((uint*)(pdst + 2)) = (uint)v2;
                                        *((uint*)(pdst + 5)) = (uint)v3;
                                        *((uint*)(pdst + 8)) = (uint)v4;
                                        pdst += 12;
                                        wsize += 12;
                                        break;
                                    }
                                case 44:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((uint*)(pdst + 2)) = (uint)v2;
                                        *((uint*)(pdst + 5)) = (uint)v3;
                                        *(pdst + 9) = (byte)(v4);
                                        pdst += 10;
                                        wsize += 10;
                                        break;
                                    }
                                case 45:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((uint*)(pdst + 2)) = (uint)v2;
                                        *((uint*)(pdst + 5)) = (uint)v3;
                                        *((ushort*)(pdst + 9)) = (ushort)v4;
                                        pdst += 11;
                                        wsize += 11;
                                        break;
                                    }
                                case 46:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((uint*)(pdst + 2)) = (uint)v2;
                                        *((uint*)(pdst + 5)) = (uint)v3;
                                        *((uint*)(pdst + 9)) = (uint)v4;
                                        pdst += 12;
                                        wsize += 12;
                                        break;
                                    }
                                case 47:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((uint*)(pdst + 2)) = (uint)v2;
                                        *((uint*)(pdst + 5)) = (uint)v3;
                                        *((uint*)(pdst + 9)) = (uint)v4;
                                        pdst += 13;
                                        wsize += 13;
                                        break;
                                    }
                                case 48:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((uint*)(pdst + 2)) = (uint)v2;
                                        *(pdst + 6) = (byte)(v3);
                                        *(pdst + 7) = (byte)(v4);
                                        pdst += 8;
                                        wsize += 8;
                                        break;
                                    }
                                case 49:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((uint*)(pdst + 2)) = (uint)v2;
                                        *(pdst + 6) = (byte)(v3);
                                        *((ushort*)(pdst + 7)) = (ushort)v4;
                                        pdst += 9;
                                        wsize += 9;
                                        break;
                                    }
                                case 50:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((uint*)(pdst + 2)) = (uint)v2;
                                        *(pdst + 6) = (byte)(v3);
                                        *((uint*)(pdst + 7)) = (uint)v4;
                                        pdst += 10;
                                        wsize += 10;
                                        break;
                                    }
                                case 51:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((uint*)(pdst + 2)) = (uint)v2;
                                        *(pdst + 6) = (byte)(v3);
                                        *((uint*)(pdst + 7)) = (uint)v4;
                                        pdst += 11;
                                        wsize += 11;
                                        break;
                                    }
                                case 52:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((uint*)(pdst + 2)) = (uint)v2;
                                        *((ushort*)(pdst + 6)) = (ushort)v3;
                                        *(pdst + 8) = (byte)(v4);
                                        pdst += 9;
                                        wsize += 9;
                                        break;
                                    }
                                case 53:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((uint*)(pdst + 2)) = (uint)v2;
                                        *((ushort*)(pdst + 6)) = (ushort)v3;
                                        *((ushort*)(pdst + 8)) = (ushort)v4;
                                        pdst += 10;
                                        wsize += 10;
                                        break;
                                    }
                                case 54:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((uint*)(pdst + 2)) = (uint)v2;
                                        *((ushort*)(pdst + 6)) = (ushort)v3;
                                        *((uint*)(pdst + 8)) = (uint)v4;
                                        pdst += 11;
                                        wsize += 11;
                                        break;
                                    }
                                case 55:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((uint*)(pdst + 2)) = (uint)v2;
                                        *((ushort*)(pdst + 6)) = (ushort)v3;
                                        *((uint*)(pdst + 8)) = (uint)v4;
                                        pdst += 12;
                                        wsize += 12;
                                        break;
                                    }
                                case 56:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((uint*)(pdst + 2)) = (uint)v2;
                                        *((uint*)(pdst + 6)) = (uint)v3;
                                        *(pdst + 9) = (byte)(v4);
                                        pdst += 10;
                                        wsize += 10;
                                        break;
                                    }
                                case 57:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((uint*)(pdst + 2)) = (uint)v2;
                                        *((uint*)(pdst + 6)) = (uint)v3;
                                        *((ushort*)(pdst + 9)) = (ushort)v4;
                                        pdst += 11;
                                        wsize += 11;
                                        break;
                                    }
                                case 58:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((uint*)(pdst + 2)) = (uint)v2;
                                        *((uint*)(pdst + 6)) = (uint)v3;
                                        *((uint*)(pdst + 9)) = (uint)v4;
                                        pdst += 12;
                                        wsize += 12;
                                        break;
                                    }
                                case 59:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((uint*)(pdst + 2)) = (uint)v2;
                                        *((uint*)(pdst + 6)) = (uint)v3;
                                        *((uint*)(pdst + 9)) = (uint)v4;
                                        pdst += 13;
                                        wsize += 13;
                                        break;
                                    }
                                case 60:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((uint*)(pdst + 2)) = (uint)v2;
                                        *((uint*)(pdst + 6)) = (uint)v3;
                                        *(pdst + 10) = (byte)(v4);
                                        pdst += 11;
                                        wsize += 11;
                                        break;
                                    }
                                case 61:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((uint*)(pdst + 2)) = (uint)v2;
                                        *((uint*)(pdst + 6)) = (uint)v3;
                                        *((ushort*)(pdst + 10)) = (ushort)v4;
                                        pdst += 12;
                                        wsize += 12;
                                        break;
                                    }
                                case 62:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((uint*)(pdst + 2)) = (uint)v2;
                                        *((uint*)(pdst + 6)) = (uint)v3;
                                        *((uint*)(pdst + 10)) = (uint)v4;
                                        pdst += 13;
                                        wsize += 13;
                                        break;
                                    }
                                case 63:
                                    {
                                        *(pdst + 1) = (byte)(v1);
                                        *((uint*)(pdst + 2)) = (uint)v2;
                                        *((uint*)(pdst + 6)) = (uint)v3;
                                        *((uint*)(pdst + 10)) = (uint)v4;
                                        pdst += 14;
                                        wsize += 14;
                                        break;
                                    }
                                case 64:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *(pdst + 3) = (byte)(v2);
                                        *(pdst + 4) = (byte)(v3);
                                        *(pdst + 5) = (byte)(v4);
                                        pdst += 6;
                                        wsize += 6;
                                        break;
                                    }
                                case 65:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *(pdst + 3) = (byte)(v2);
                                        *(pdst + 4) = (byte)(v3);
                                        *((ushort*)(pdst + 5)) = (ushort)v4;
                                        pdst += 7;
                                        wsize += 7;
                                        break;
                                    }
                                case 66:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *(pdst + 3) = (byte)(v2);
                                        *(pdst + 4) = (byte)(v3);
                                        *((uint*)(pdst + 5)) = (uint)v4;
                                        pdst += 8;
                                        wsize += 8;
                                        break;
                                    }
                                case 67:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *(pdst + 3) = (byte)(v2);
                                        *(pdst + 4) = (byte)(v3);
                                        *((uint*)(pdst + 5)) = (uint)v4;
                                        pdst += 9;
                                        wsize += 9;
                                        break;
                                    }
                                case 68:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *(pdst + 3) = (byte)(v2);
                                        *((ushort*)(pdst + 4)) = (ushort)v3;
                                        *(pdst + 6) = (byte)(v4);
                                        pdst += 7;
                                        wsize += 7;
                                        break;
                                    }
                                case 69:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *(pdst + 3) = (byte)(v2);
                                        *((ushort*)(pdst + 4)) = (ushort)v3;
                                        *((ushort*)(pdst + 6)) = (ushort)v4;
                                        pdst += 8;
                                        wsize += 8;
                                        break;
                                    }
                                case 70:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *(pdst + 3) = (byte)(v2);
                                        *((ushort*)(pdst + 4)) = (ushort)v3;
                                        *((uint*)(pdst + 6)) = (uint)v4;
                                        pdst += 9;
                                        wsize += 9;
                                        break;
                                    }
                                case 71:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *(pdst + 3) = (byte)(v2);
                                        *((ushort*)(pdst + 4)) = (ushort)v3;
                                        *((uint*)(pdst + 6)) = (uint)v4;
                                        pdst += 10;
                                        wsize += 10;
                                        break;
                                    }
                                case 72:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *(pdst + 3) = (byte)(v2);
                                        *((uint*)(pdst + 4)) = (uint)v3;
                                        *(pdst + 7) = (byte)(v4);
                                        pdst += 8;
                                        wsize += 8;
                                        break;
                                    }
                                case 73:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *(pdst + 3) = (byte)(v2);
                                        *((uint*)(pdst + 4)) = (uint)v3;
                                        *((ushort*)(pdst + 7)) = (ushort)v4;
                                        pdst += 9;
                                        wsize += 9;
                                        break;
                                    }
                                case 74:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *(pdst + 3) = (byte)(v2);
                                        *((uint*)(pdst + 4)) = (uint)v3;
                                        *((uint*)(pdst + 7)) = (uint)v4;
                                        pdst += 10;
                                        wsize += 10;
                                        break;
                                    }
                                case 75:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *(pdst + 3) = (byte)(v2);
                                        *((uint*)(pdst + 4)) = (uint)v3;
                                        *((uint*)(pdst + 7)) = (uint)v4;
                                        pdst += 11;
                                        wsize += 11;
                                        break;
                                    }
                                case 76:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *(pdst + 3) = (byte)(v2);
                                        *((uint*)(pdst + 4)) = (uint)v3;
                                        *(pdst + 8) = (byte)(v4);
                                        pdst += 9;
                                        wsize += 9;
                                        break;
                                    }
                                case 77:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *(pdst + 3) = (byte)(v2);
                                        *((uint*)(pdst + 4)) = (uint)v3;
                                        *((ushort*)(pdst + 8)) = (ushort)v4;
                                        pdst += 10;
                                        wsize += 10;
                                        break;
                                    }
                                case 78:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *(pdst + 3) = (byte)(v2);
                                        *((uint*)(pdst + 4)) = (uint)v3;
                                        *((uint*)(pdst + 8)) = (uint)v4;
                                        pdst += 11;
                                        wsize += 11;
                                        break;
                                    }
                                case 79:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *(pdst + 3) = (byte)(v2);
                                        *((uint*)(pdst + 4)) = (uint)v3;
                                        *((uint*)(pdst + 8)) = (uint)v4;
                                        pdst += 12;
                                        wsize += 12;
                                        break;
                                    }
                                case 80:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((ushort*)(pdst + 3)) = (ushort)v2;
                                        *(pdst + 5) = (byte)(v3);
                                        *(pdst + 6) = (byte)(v4);
                                        pdst += 7;
                                        wsize += 7;
                                        break;
                                    }
                                case 81:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((ushort*)(pdst + 3)) = (ushort)v2;
                                        *(pdst + 5) = (byte)(v3);
                                        *((ushort*)(pdst + 6)) = (ushort)v4;
                                        pdst += 8;
                                        wsize += 8;
                                        break;
                                    }
                                case 82:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((ushort*)(pdst + 3)) = (ushort)v2;
                                        *(pdst + 5) = (byte)(v3);
                                        *((uint*)(pdst + 6)) = (uint)v4;
                                        pdst += 9;
                                        wsize += 9;
                                        break;
                                    }
                                case 83:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((ushort*)(pdst + 3)) = (ushort)v2;
                                        *(pdst + 5) = (byte)(v3);
                                        *((uint*)(pdst + 6)) = (uint)v4;
                                        pdst += 10;
                                        wsize += 10;
                                        break;
                                    }
                                case 84:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((ushort*)(pdst + 3)) = (ushort)v2;
                                        *((ushort*)(pdst + 5)) = (ushort)v3;
                                        *(pdst + 7) = (byte)(v4);
                                        pdst += 8;
                                        wsize += 8;
                                        break;
                                    }
                                case 85:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((ushort*)(pdst + 3)) = (ushort)v2;
                                        *((ushort*)(pdst + 5)) = (ushort)v3;
                                        *((ushort*)(pdst + 7)) = (ushort)v4;
                                        pdst += 9;
                                        wsize += 9;
                                        break;
                                    }
                                case 86:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((ushort*)(pdst + 3)) = (ushort)v2;
                                        *((ushort*)(pdst + 5)) = (ushort)v3;
                                        *((uint*)(pdst + 7)) = (uint)v4;
                                        pdst += 10;
                                        wsize += 10;
                                        break;
                                    }
                                case 87:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((ushort*)(pdst + 3)) = (ushort)v2;
                                        *((ushort*)(pdst + 5)) = (ushort)v3;
                                        *((uint*)(pdst + 7)) = (uint)v4;
                                        pdst += 11;
                                        wsize += 11;
                                        break;
                                    }
                                case 88:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((ushort*)(pdst + 3)) = (ushort)v2;
                                        *((uint*)(pdst + 5)) = (uint)v3;
                                        *(pdst + 8) = (byte)(v4);
                                        pdst += 9;
                                        wsize += 9;
                                        break;
                                    }
                                case 89:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((ushort*)(pdst + 3)) = (ushort)v2;
                                        *((uint*)(pdst + 5)) = (uint)v3;
                                        *((ushort*)(pdst + 8)) = (ushort)v4;
                                        pdst += 10;
                                        wsize += 10;
                                        break;
                                    }
                                case 90:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((ushort*)(pdst + 3)) = (ushort)v2;
                                        *((uint*)(pdst + 5)) = (uint)v3;
                                        *((uint*)(pdst + 8)) = (uint)v4;
                                        pdst += 11;
                                        wsize += 11;
                                        break;
                                    }
                                case 91:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((ushort*)(pdst + 3)) = (ushort)v2;
                                        *((uint*)(pdst + 5)) = (uint)v3;
                                        *((uint*)(pdst + 8)) = (uint)v4;
                                        pdst += 12;
                                        wsize += 12;
                                        break;
                                    }
                                case 92:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((ushort*)(pdst + 3)) = (ushort)v2;
                                        *((uint*)(pdst + 5)) = (uint)v3;
                                        *(pdst + 9) = (byte)(v4);
                                        pdst += 10;
                                        wsize += 10;
                                        break;
                                    }
                                case 93:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((ushort*)(pdst + 3)) = (ushort)v2;
                                        *((uint*)(pdst + 5)) = (uint)v3;
                                        *((ushort*)(pdst + 9)) = (ushort)v4;
                                        pdst += 11;
                                        wsize += 11;
                                        break;
                                    }
                                case 94:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((ushort*)(pdst + 3)) = (ushort)v2;
                                        *((uint*)(pdst + 5)) = (uint)v3;
                                        *((uint*)(pdst + 9)) = (uint)v4;
                                        pdst += 12;
                                        wsize += 12;
                                        break;
                                    }
                                case 95:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((ushort*)(pdst + 3)) = (ushort)v2;
                                        *((uint*)(pdst + 5)) = (uint)v3;
                                        *((uint*)(pdst + 9)) = (uint)v4;
                                        pdst += 13;
                                        wsize += 13;
                                        break;
                                    }
                                case 96:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((uint*)(pdst + 3)) = (uint)v2;
                                        *(pdst + 6) = (byte)(v3);
                                        *(pdst + 7) = (byte)(v4);
                                        pdst += 8;
                                        wsize += 8;
                                        break;
                                    }
                                case 97:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((uint*)(pdst + 3)) = (uint)v2;
                                        *(pdst + 6) = (byte)(v3);
                                        *((ushort*)(pdst + 7)) = (ushort)v4;
                                        pdst += 9;
                                        wsize += 9;
                                        break;
                                    }
                                case 98:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((uint*)(pdst + 3)) = (uint)v2;
                                        *(pdst + 6) = (byte)(v3);
                                        *((uint*)(pdst + 7)) = (uint)v4;
                                        pdst += 10;
                                        wsize += 10;
                                        break;
                                    }
                                case 99:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((uint*)(pdst + 3)) = (uint)v2;
                                        *(pdst + 6) = (byte)(v3);
                                        *((uint*)(pdst + 7)) = (uint)v4;
                                        pdst += 11;
                                        wsize += 11;
                                        break;
                                    }
                                case 100:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((uint*)(pdst + 3)) = (uint)v2;
                                        *((ushort*)(pdst + 6)) = (ushort)v3;
                                        *(pdst + 8) = (byte)(v4);
                                        pdst += 9;
                                        wsize += 9;
                                        break;
                                    }
                                case 101:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((uint*)(pdst + 3)) = (uint)v2;
                                        *((ushort*)(pdst + 6)) = (ushort)v3;
                                        *((ushort*)(pdst + 8)) = (ushort)v4;
                                        pdst += 10;
                                        wsize += 10;
                                        break;
                                    }
                                case 102:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((uint*)(pdst + 3)) = (uint)v2;
                                        *((ushort*)(pdst + 6)) = (ushort)v3;
                                        *((uint*)(pdst + 8)) = (uint)v4;
                                        pdst += 11;
                                        wsize += 11;
                                        break;
                                    }
                                case 103:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((uint*)(pdst + 3)) = (uint)v2;
                                        *((ushort*)(pdst + 6)) = (ushort)v3;
                                        *((uint*)(pdst + 8)) = (uint)v4;
                                        pdst += 12;
                                        wsize += 12;
                                        break;
                                    }
                                case 104:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((uint*)(pdst + 3)) = (uint)v2;
                                        *((uint*)(pdst + 6)) = (uint)v3;
                                        *(pdst + 9) = (byte)(v4);
                                        pdst += 10;
                                        wsize += 10;
                                        break;
                                    }
                                case 105:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((uint*)(pdst + 3)) = (uint)v2;
                                        *((uint*)(pdst + 6)) = (uint)v3;
                                        *((ushort*)(pdst + 9)) = (ushort)v4;
                                        pdst += 11;
                                        wsize += 11;
                                        break;
                                    }
                                case 106:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((uint*)(pdst + 3)) = (uint)v2;
                                        *((uint*)(pdst + 6)) = (uint)v3;
                                        *((uint*)(pdst + 9)) = (uint)v4;
                                        pdst += 12;
                                        wsize += 12;
                                        break;
                                    }
                                case 107:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((uint*)(pdst + 3)) = (uint)v2;
                                        *((uint*)(pdst + 6)) = (uint)v3;
                                        *((uint*)(pdst + 9)) = (uint)v4;
                                        pdst += 13;
                                        wsize += 13;
                                        break;
                                    }
                                case 108:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((uint*)(pdst + 3)) = (uint)v2;
                                        *((uint*)(pdst + 6)) = (uint)v3;
                                        *(pdst + 10) = (byte)(v4);
                                        pdst += 11;
                                        wsize += 11;
                                        break;
                                    }
                                case 109:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((uint*)(pdst + 3)) = (uint)v2;
                                        *((uint*)(pdst + 6)) = (uint)v3;
                                        *((ushort*)(pdst + 10)) = (ushort)v4;
                                        pdst += 12;
                                        wsize += 12;
                                        break;
                                    }
                                case 110:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((uint*)(pdst + 3)) = (uint)v2;
                                        *((uint*)(pdst + 6)) = (uint)v3;
                                        *((uint*)(pdst + 10)) = (uint)v4;
                                        pdst += 13;
                                        wsize += 13;
                                        break;
                                    }
                                case 111:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((uint*)(pdst + 3)) = (uint)v2;
                                        *((uint*)(pdst + 6)) = (uint)v3;
                                        *((uint*)(pdst + 10)) = (uint)v4;
                                        pdst += 14;
                                        wsize += 14;
                                        break;
                                    }
                                case 112:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((uint*)(pdst + 3)) = (uint)v2;
                                        *(pdst + 7) = (byte)(v3);
                                        *(pdst + 8) = (byte)(v4);
                                        pdst += 9;
                                        wsize += 9;
                                        break;
                                    }
                                case 113:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((uint*)(pdst + 3)) = (uint)v2;
                                        *(pdst + 7) = (byte)(v3);
                                        *((ushort*)(pdst + 8)) = (ushort)v4;
                                        pdst += 10;
                                        wsize += 10;
                                        break;
                                    }
                                case 114:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((uint*)(pdst + 3)) = (uint)v2;
                                        *(pdst + 7) = (byte)(v3);
                                        *((uint*)(pdst + 8)) = (uint)v4;
                                        pdst += 11;
                                        wsize += 11;
                                        break;
                                    }
                                case 115:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((uint*)(pdst + 3)) = (uint)v2;
                                        *(pdst + 7) = (byte)(v3);
                                        *((uint*)(pdst + 8)) = (uint)v4;
                                        pdst += 12;
                                        wsize += 12;
                                        break;
                                    }
                                case 116:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((uint*)(pdst + 3)) = (uint)v2;
                                        *((ushort*)(pdst + 7)) = (ushort)v3;
                                        *(pdst + 9) = (byte)(v4);
                                        pdst += 10;
                                        wsize += 10;
                                        break;
                                    }
                                case 117:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((uint*)(pdst + 3)) = (uint)v2;
                                        *((ushort*)(pdst + 7)) = (ushort)v3;
                                        *((ushort*)(pdst + 9)) = (ushort)v4;
                                        pdst += 11;
                                        wsize += 11;
                                        break;
                                    }
                                case 118:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((uint*)(pdst + 3)) = (uint)v2;
                                        *((ushort*)(pdst + 7)) = (ushort)v3;
                                        *((uint*)(pdst + 9)) = (uint)v4;
                                        pdst += 12;
                                        wsize += 12;
                                        break;
                                    }
                                case 119:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((uint*)(pdst + 3)) = (uint)v2;
                                        *((ushort*)(pdst + 7)) = (ushort)v3;
                                        *((uint*)(pdst + 9)) = (uint)v4;
                                        pdst += 13;
                                        wsize += 13;
                                        break;
                                    }
                                case 120:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((uint*)(pdst + 3)) = (uint)v2;
                                        *((uint*)(pdst + 7)) = (uint)v3;
                                        *(pdst + 10) = (byte)(v4);
                                        pdst += 11;
                                        wsize += 11;
                                        break;
                                    }
                                case 121:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((uint*)(pdst + 3)) = (uint)v2;
                                        *((uint*)(pdst + 7)) = (uint)v3;
                                        *((ushort*)(pdst + 10)) = (ushort)v4;
                                        pdst += 12;
                                        wsize += 12;
                                        break;
                                    }
                                case 122:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((uint*)(pdst + 3)) = (uint)v2;
                                        *((uint*)(pdst + 7)) = (uint)v3;
                                        *((uint*)(pdst + 10)) = (uint)v4;
                                        pdst += 13;
                                        wsize += 13;
                                        break;
                                    }
                                case 123:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((uint*)(pdst + 3)) = (uint)v2;
                                        *((uint*)(pdst + 7)) = (uint)v3;
                                        *((uint*)(pdst + 10)) = (uint)v4;
                                        pdst += 14;
                                        wsize += 14;
                                        break;
                                    }
                                case 124:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((uint*)(pdst + 3)) = (uint)v2;
                                        *((uint*)(pdst + 7)) = (uint)v3;
                                        *(pdst + 11) = (byte)(v4);
                                        pdst += 12;
                                        wsize += 12;
                                        break;
                                    }
                                case 125:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((uint*)(pdst + 3)) = (uint)v2;
                                        *((uint*)(pdst + 7)) = (uint)v3;
                                        *((ushort*)(pdst + 11)) = (ushort)v4;
                                        pdst += 13;
                                        wsize += 13;
                                        break;
                                    }
                                case 126:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((uint*)(pdst + 3)) = (uint)v2;
                                        *((uint*)(pdst + 7)) = (uint)v3;
                                        *((uint*)(pdst + 11)) = (uint)v4;
                                        pdst += 14;
                                        wsize += 14;
                                        break;
                                    }
                                case 127:
                                    {
                                        *((ushort*)(pdst + 1)) = (ushort)v1;
                                        *((uint*)(pdst + 3)) = (uint)v2;
                                        *((uint*)(pdst + 7)) = (uint)v3;
                                        *((uint*)(pdst + 11)) = (uint)v4;
                                        pdst += 15;
                                        wsize += 15;
                                        break;
                                    }
                                case 128:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *(pdst + 4) = (byte)(v2);
                                        *(pdst + 5) = (byte)(v3);
                                        *(pdst + 6) = (byte)(v4);
                                        pdst += 7;
                                        wsize += 7;
                                        break;
                                    }
                                case 129:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *(pdst + 4) = (byte)(v2);
                                        *(pdst + 5) = (byte)(v3);
                                        *((ushort*)(pdst + 6)) = (ushort)v4;
                                        pdst += 8;
                                        wsize += 8;
                                        break;
                                    }
                                case 130:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *(pdst + 4) = (byte)(v2);
                                        *(pdst + 5) = (byte)(v3);
                                        *((uint*)(pdst + 6)) = (uint)v4;
                                        pdst += 9;
                                        wsize += 9;
                                        break;
                                    }
                                case 131:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *(pdst + 4) = (byte)(v2);
                                        *(pdst + 5) = (byte)(v3);
                                        *((uint*)(pdst + 6)) = (uint)v4;
                                        pdst += 10;
                                        wsize += 10;
                                        break;
                                    }
                                case 132:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *(pdst + 4) = (byte)(v2);
                                        *((ushort*)(pdst + 5)) = (ushort)v3;
                                        *(pdst + 7) = (byte)(v4);
                                        pdst += 8;
                                        wsize += 8;
                                        break;
                                    }
                                case 133:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *(pdst + 4) = (byte)(v2);
                                        *((ushort*)(pdst + 5)) = (ushort)v3;
                                        *((ushort*)(pdst + 7)) = (ushort)v4;
                                        pdst += 9;
                                        wsize += 9;
                                        break;
                                    }
                                case 134:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *(pdst + 4) = (byte)(v2);
                                        *((ushort*)(pdst + 5)) = (ushort)v3;
                                        *((uint*)(pdst + 7)) = (uint)v4;
                                        pdst += 10;
                                        wsize += 10;
                                        break;
                                    }
                                case 135:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *(pdst + 4) = (byte)(v2);
                                        *((ushort*)(pdst + 5)) = (ushort)v3;
                                        *((uint*)(pdst + 7)) = (uint)v4;
                                        pdst += 11;
                                        wsize += 11;
                                        break;
                                    }
                                case 136:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *(pdst + 4) = (byte)(v2);
                                        *((uint*)(pdst + 5)) = (uint)v3;
                                        *(pdst + 8) = (byte)(v4);
                                        pdst += 9;
                                        wsize += 9;
                                        break;
                                    }
                                case 137:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *(pdst + 4) = (byte)(v2);
                                        *((uint*)(pdst + 5)) = (uint)v3;
                                        *((ushort*)(pdst + 8)) = (ushort)v4;
                                        pdst += 10;
                                        wsize += 10;
                                        break;
                                    }
                                case 138:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *(pdst + 4) = (byte)(v2);
                                        *((uint*)(pdst + 5)) = (uint)v3;
                                        *((uint*)(pdst + 8)) = (uint)v4;
                                        pdst += 11;
                                        wsize += 11;
                                        break;
                                    }
                                case 139:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *(pdst + 4) = (byte)(v2);
                                        *((uint*)(pdst + 5)) = (uint)v3;
                                        *((uint*)(pdst + 8)) = (uint)v4;
                                        pdst += 12;
                                        wsize += 12;
                                        break;
                                    }
                                case 140:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *(pdst + 4) = (byte)(v2);
                                        *((uint*)(pdst + 5)) = (uint)v3;
                                        *(pdst + 9) = (byte)(v4);
                                        pdst += 10;
                                        wsize += 10;
                                        break;
                                    }
                                case 141:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *(pdst + 4) = (byte)(v2);
                                        *((uint*)(pdst + 5)) = (uint)v3;
                                        *((ushort*)(pdst + 9)) = (ushort)v4;
                                        pdst += 11;
                                        wsize += 11;
                                        break;
                                    }
                                case 142:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *(pdst + 4) = (byte)(v2);
                                        *((uint*)(pdst + 5)) = (uint)v3;
                                        *((uint*)(pdst + 9)) = (uint)v4;
                                        pdst += 12;
                                        wsize += 12;
                                        break;
                                    }
                                case 143:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *(pdst + 4) = (byte)(v2);
                                        *((uint*)(pdst + 5)) = (uint)v3;
                                        *((uint*)(pdst + 9)) = (uint)v4;
                                        pdst += 13;
                                        wsize += 13;
                                        break;
                                    }
                                case 144:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((ushort*)(pdst + 4)) = (ushort)v2;
                                        *(pdst + 6) = (byte)(v3);
                                        *(pdst + 7) = (byte)(v4);
                                        pdst += 8;
                                        wsize += 8;
                                        break;
                                    }
                                case 145:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((ushort*)(pdst + 4)) = (ushort)v2;
                                        *(pdst + 6) = (byte)(v3);
                                        *((ushort*)(pdst + 7)) = (ushort)v4;
                                        pdst += 9;
                                        wsize += 9;
                                        break;
                                    }
                                case 146:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((ushort*)(pdst + 4)) = (ushort)v2;
                                        *(pdst + 6) = (byte)(v3);
                                        *((uint*)(pdst + 7)) = (uint)v4;
                                        pdst += 10;
                                        wsize += 10;
                                        break;
                                    }
                                case 147:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((ushort*)(pdst + 4)) = (ushort)v2;
                                        *(pdst + 6) = (byte)(v3);
                                        *((uint*)(pdst + 7)) = (uint)v4;
                                        pdst += 11;
                                        wsize += 11;
                                        break;
                                    }
                                case 148:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((ushort*)(pdst + 4)) = (ushort)v2;
                                        *((ushort*)(pdst + 6)) = (ushort)v3;
                                        *(pdst + 8) = (byte)(v4);
                                        pdst += 9;
                                        wsize += 9;
                                        break;
                                    }
                                case 149:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((ushort*)(pdst + 4)) = (ushort)v2;
                                        *((ushort*)(pdst + 6)) = (ushort)v3;
                                        *((ushort*)(pdst + 8)) = (ushort)v4;
                                        pdst += 10;
                                        wsize += 10;
                                        break;
                                    }
                                case 150:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((ushort*)(pdst + 4)) = (ushort)v2;
                                        *((ushort*)(pdst + 6)) = (ushort)v3;
                                        *((uint*)(pdst + 8)) = (uint)v4;
                                        pdst += 11;
                                        wsize += 11;
                                        break;
                                    }
                                case 151:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((ushort*)(pdst + 4)) = (ushort)v2;
                                        *((ushort*)(pdst + 6)) = (ushort)v3;
                                        *((uint*)(pdst + 8)) = (uint)v4;
                                        pdst += 12;
                                        wsize += 12;
                                        break;
                                    }
                                case 152:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((ushort*)(pdst + 4)) = (ushort)v2;
                                        *((uint*)(pdst + 6)) = (uint)v3;
                                        *(pdst + 9) = (byte)(v4);
                                        pdst += 10;
                                        wsize += 10;
                                        break;
                                    }
                                case 153:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((ushort*)(pdst + 4)) = (ushort)v2;
                                        *((uint*)(pdst + 6)) = (uint)v3;
                                        *((ushort*)(pdst + 9)) = (ushort)v4;
                                        pdst += 11;
                                        wsize += 11;
                                        break;
                                    }
                                case 154:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((ushort*)(pdst + 4)) = (ushort)v2;
                                        *((uint*)(pdst + 6)) = (uint)v3;
                                        *((uint*)(pdst + 9)) = (uint)v4;
                                        pdst += 12;
                                        wsize += 12;
                                        break;
                                    }
                                case 155:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((ushort*)(pdst + 4)) = (ushort)v2;
                                        *((uint*)(pdst + 6)) = (uint)v3;
                                        *((uint*)(pdst + 9)) = (uint)v4;
                                        pdst += 13;
                                        wsize += 13;
                                        break;
                                    }
                                case 156:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((ushort*)(pdst + 4)) = (ushort)v2;
                                        *((uint*)(pdst + 6)) = (uint)v3;
                                        *(pdst + 10) = (byte)(v4);
                                        pdst += 11;
                                        wsize += 11;
                                        break;
                                    }
                                case 157:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((ushort*)(pdst + 4)) = (ushort)v2;
                                        *((uint*)(pdst + 6)) = (uint)v3;
                                        *((ushort*)(pdst + 10)) = (ushort)v4;
                                        pdst += 12;
                                        wsize += 12;
                                        break;
                                    }
                                case 158:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((ushort*)(pdst + 4)) = (ushort)v2;
                                        *((uint*)(pdst + 6)) = (uint)v3;
                                        *((uint*)(pdst + 10)) = (uint)v4;
                                        pdst += 13;
                                        wsize += 13;
                                        break;
                                    }
                                case 159:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((ushort*)(pdst + 4)) = (ushort)v2;
                                        *((uint*)(pdst + 6)) = (uint)v3;
                                        *((uint*)(pdst + 10)) = (uint)v4;
                                        pdst += 14;
                                        wsize += 14;
                                        break;
                                    }
                                case 160:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 4)) = (uint)v2;
                                        *(pdst + 7) = (byte)(v3);
                                        *(pdst + 8) = (byte)(v4);
                                        pdst += 9;
                                        wsize += 9;
                                        break;
                                    }
                                case 161:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 4)) = (uint)v2;
                                        *(pdst + 7) = (byte)(v3);
                                        *((ushort*)(pdst + 8)) = (ushort)v4;
                                        pdst += 10;
                                        wsize += 10;
                                        break;
                                    }
                                case 162:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 4)) = (uint)v2;
                                        *(pdst + 7) = (byte)(v3);
                                        *((uint*)(pdst + 8)) = (uint)v4;
                                        pdst += 11;
                                        wsize += 11;
                                        break;
                                    }
                                case 163:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 4)) = (uint)v2;
                                        *(pdst + 7) = (byte)(v3);
                                        *((uint*)(pdst + 8)) = (uint)v4;
                                        pdst += 12;
                                        wsize += 12;
                                        break;
                                    }
                                case 164:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 4)) = (uint)v2;
                                        *((ushort*)(pdst + 7)) = (ushort)v3;
                                        *(pdst + 9) = (byte)(v4);
                                        pdst += 10;
                                        wsize += 10;
                                        break;
                                    }
                                case 165:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 4)) = (uint)v2;
                                        *((ushort*)(pdst + 7)) = (ushort)v3;
                                        *((ushort*)(pdst + 9)) = (ushort)v4;
                                        pdst += 11;
                                        wsize += 11;
                                        break;
                                    }
                                case 166:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 4)) = (uint)v2;
                                        *((ushort*)(pdst + 7)) = (ushort)v3;
                                        *((uint*)(pdst + 9)) = (uint)v4;
                                        pdst += 12;
                                        wsize += 12;
                                        break;
                                    }
                                case 167:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 4)) = (uint)v2;
                                        *((ushort*)(pdst + 7)) = (ushort)v3;
                                        *((uint*)(pdst + 9)) = (uint)v4;
                                        pdst += 13;
                                        wsize += 13;
                                        break;
                                    }
                                case 168:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 4)) = (uint)v2;
                                        *((uint*)(pdst + 7)) = (uint)v3;
                                        *(pdst + 10) = (byte)(v4);
                                        pdst += 11;
                                        wsize += 11;
                                        break;
                                    }
                                case 169:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 4)) = (uint)v2;
                                        *((uint*)(pdst + 7)) = (uint)v3;
                                        *((ushort*)(pdst + 10)) = (ushort)v4;
                                        pdst += 12;
                                        wsize += 12;
                                        break;
                                    }
                                case 170:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 4)) = (uint)v2;
                                        *((uint*)(pdst + 7)) = (uint)v3;
                                        *((uint*)(pdst + 10)) = (uint)v4;
                                        pdst += 13;
                                        wsize += 13;
                                        break;
                                    }
                                case 171:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 4)) = (uint)v2;
                                        *((uint*)(pdst + 7)) = (uint)v3;
                                        *((uint*)(pdst + 10)) = (uint)v4;
                                        pdst += 14;
                                        wsize += 14;
                                        break;
                                    }
                                case 172:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 4)) = (uint)v2;
                                        *((uint*)(pdst + 7)) = (uint)v3;
                                        *(pdst + 11) = (byte)(v4);
                                        pdst += 12;
                                        wsize += 12;
                                        break;
                                    }
                                case 173:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 4)) = (uint)v2;
                                        *((uint*)(pdst + 7)) = (uint)v3;
                                        *((ushort*)(pdst + 11)) = (ushort)v4;
                                        pdst += 13;
                                        wsize += 13;
                                        break;
                                    }
                                case 174:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 4)) = (uint)v2;
                                        *((uint*)(pdst + 7)) = (uint)v3;
                                        *((uint*)(pdst + 11)) = (uint)v4;
                                        pdst += 14;
                                        wsize += 14;
                                        break;
                                    }
                                case 175:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 4)) = (uint)v2;
                                        *((uint*)(pdst + 7)) = (uint)v3;
                                        *((uint*)(pdst + 11)) = (uint)v4;
                                        pdst += 15;
                                        wsize += 15;
                                        break;
                                    }
                                case 176:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 4)) = (uint)v2;
                                        *(pdst + 8) = (byte)(v3);
                                        *(pdst + 9) = (byte)(v4);
                                        pdst += 10;
                                        wsize += 10;
                                        break;
                                    }
                                case 177:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 4)) = (uint)v2;
                                        *(pdst + 8) = (byte)(v3);
                                        *((ushort*)(pdst + 9)) = (ushort)v4;
                                        pdst += 11;
                                        wsize += 11;
                                        break;
                                    }
                                case 178:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 4)) = (uint)v2;
                                        *(pdst + 8) = (byte)(v3);
                                        *((uint*)(pdst + 9)) = (uint)v4;
                                        pdst += 12;
                                        wsize += 12;
                                        break;
                                    }
                                case 179:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 4)) = (uint)v2;
                                        *(pdst + 8) = (byte)(v3);
                                        *((uint*)(pdst + 9)) = (uint)v4;
                                        pdst += 13;
                                        wsize += 13;
                                        break;
                                    }
                                case 180:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 4)) = (uint)v2;
                                        *((ushort*)(pdst + 8)) = (ushort)v3;
                                        *(pdst + 10) = (byte)(v4);
                                        pdst += 11;
                                        wsize += 11;
                                        break;
                                    }
                                case 181:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 4)) = (uint)v2;
                                        *((ushort*)(pdst + 8)) = (ushort)v3;
                                        *((ushort*)(pdst + 10)) = (ushort)v4;
                                        pdst += 12;
                                        wsize += 12;
                                        break;
                                    }
                                case 182:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 4)) = (uint)v2;
                                        *((ushort*)(pdst + 8)) = (ushort)v3;
                                        *((uint*)(pdst + 10)) = (uint)v4;
                                        pdst += 13;
                                        wsize += 13;
                                        break;
                                    }
                                case 183:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 4)) = (uint)v2;
                                        *((ushort*)(pdst + 8)) = (ushort)v3;
                                        *((uint*)(pdst + 10)) = (uint)v4;
                                        pdst += 14;
                                        wsize += 14;
                                        break;
                                    }
                                case 184:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 4)) = (uint)v2;
                                        *((uint*)(pdst + 8)) = (uint)v3;
                                        *(pdst + 11) = (byte)(v4);
                                        pdst += 12;
                                        wsize += 12;
                                        break;
                                    }
                                case 185:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 4)) = (uint)v2;
                                        *((uint*)(pdst + 8)) = (uint)v3;
                                        *((ushort*)(pdst + 11)) = (ushort)v4;
                                        pdst += 13;
                                        wsize += 13;
                                        break;
                                    }
                                case 186:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 4)) = (uint)v2;
                                        *((uint*)(pdst + 8)) = (uint)v3;
                                        *((uint*)(pdst + 11)) = (uint)v4;
                                        pdst += 14;
                                        wsize += 14;
                                        break;
                                    }
                                case 187:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 4)) = (uint)v2;
                                        *((uint*)(pdst + 8)) = (uint)v3;
                                        *((uint*)(pdst + 11)) = (uint)v4;
                                        pdst += 15;
                                        wsize += 15;
                                        break;
                                    }
                                case 188:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 4)) = (uint)v2;
                                        *((uint*)(pdst + 8)) = (uint)v3;
                                        *(pdst + 12) = (byte)(v4);
                                        pdst += 13;
                                        wsize += 13;
                                        break;
                                    }
                                case 189:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 4)) = (uint)v2;
                                        *((uint*)(pdst + 8)) = (uint)v3;
                                        *((ushort*)(pdst + 12)) = (ushort)v4;
                                        pdst += 14;
                                        wsize += 14;
                                        break;
                                    }
                                case 190:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 4)) = (uint)v2;
                                        *((uint*)(pdst + 8)) = (uint)v3;
                                        *((uint*)(pdst + 12)) = (uint)v4;
                                        pdst += 15;
                                        wsize += 15;
                                        break;
                                    }
                                case 191:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 4)) = (uint)v2;
                                        *((uint*)(pdst + 8)) = (uint)v3;
                                        *((uint*)(pdst + 12)) = (uint)v4;
                                        pdst += 16;
                                        wsize += 16;
                                        break;
                                    }
                                case 192:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *(pdst + 5) = (byte)(v2);
                                        *(pdst + 6) = (byte)(v3);
                                        *(pdst + 7) = (byte)(v4);
                                        pdst += 8;
                                        wsize += 8;
                                        break;
                                    }
                                case 193:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *(pdst + 5) = (byte)(v2);
                                        *(pdst + 6) = (byte)(v3);
                                        *((ushort*)(pdst + 7)) = (ushort)v4;
                                        pdst += 9;
                                        wsize += 9;
                                        break;
                                    }
                                case 194:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *(pdst + 5) = (byte)(v2);
                                        *(pdst + 6) = (byte)(v3);
                                        *((uint*)(pdst + 7)) = (uint)v4;
                                        pdst += 10;
                                        wsize += 10;
                                        break;
                                    }
                                case 195:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *(pdst + 5) = (byte)(v2);
                                        *(pdst + 6) = (byte)(v3);
                                        *((uint*)(pdst + 7)) = (uint)v4;
                                        pdst += 11;
                                        wsize += 11;
                                        break;
                                    }
                                case 196:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *(pdst + 5) = (byte)(v2);
                                        *((ushort*)(pdst + 6)) = (ushort)v3;
                                        *(pdst + 8) = (byte)(v4);
                                        pdst += 9;
                                        wsize += 9;
                                        break;
                                    }
                                case 197:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *(pdst + 5) = (byte)(v2);
                                        *((ushort*)(pdst + 6)) = (ushort)v3;
                                        *((ushort*)(pdst + 8)) = (ushort)v4;
                                        pdst += 10;
                                        wsize += 10;
                                        break;
                                    }
                                case 198:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *(pdst + 5) = (byte)(v2);
                                        *((ushort*)(pdst + 6)) = (ushort)v3;
                                        *((uint*)(pdst + 8)) = (uint)v4;
                                        pdst += 11;
                                        wsize += 11;
                                        break;
                                    }
                                case 199:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *(pdst + 5) = (byte)(v2);
                                        *((ushort*)(pdst + 6)) = (ushort)v3;
                                        *((uint*)(pdst + 8)) = (uint)v4;
                                        pdst += 12;
                                        wsize += 12;
                                        break;
                                    }
                                case 200:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *(pdst + 5) = (byte)(v2);
                                        *((uint*)(pdst + 6)) = (uint)v3;
                                        *(pdst + 9) = (byte)(v4);
                                        pdst += 10;
                                        wsize += 10;
                                        break;
                                    }
                                case 201:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *(pdst + 5) = (byte)(v2);
                                        *((uint*)(pdst + 6)) = (uint)v3;
                                        *((ushort*)(pdst + 9)) = (ushort)v4;
                                        pdst += 11;
                                        wsize += 11;
                                        break;
                                    }
                                case 202:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *(pdst + 5) = (byte)(v2);
                                        *((uint*)(pdst + 6)) = (uint)v3;
                                        *((uint*)(pdst + 9)) = (uint)v4;
                                        pdst += 12;
                                        wsize += 12;
                                        break;
                                    }
                                case 203:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *(pdst + 5) = (byte)(v2);
                                        *((uint*)(pdst + 6)) = (uint)v3;
                                        *((uint*)(pdst + 9)) = (uint)v4;
                                        pdst += 13;
                                        wsize += 13;
                                        break;
                                    }
                                case 204:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *(pdst + 5) = (byte)(v2);
                                        *((uint*)(pdst + 6)) = (uint)v3;
                                        *(pdst + 10) = (byte)(v4);
                                        pdst += 11;
                                        wsize += 11;
                                        break;
                                    }
                                case 205:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *(pdst + 5) = (byte)(v2);
                                        *((uint*)(pdst + 6)) = (uint)v3;
                                        *((ushort*)(pdst + 10)) = (ushort)v4;
                                        pdst += 12;
                                        wsize += 12;
                                        break;
                                    }
                                case 206:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *(pdst + 5) = (byte)(v2);
                                        *((uint*)(pdst + 6)) = (uint)v3;
                                        *((uint*)(pdst + 10)) = (uint)v4;
                                        pdst += 13;
                                        wsize += 13;
                                        break;
                                    }
                                case 207:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *(pdst + 5) = (byte)(v2);
                                        *((uint*)(pdst + 6)) = (uint)v3;
                                        *((uint*)(pdst + 10)) = (uint)v4;
                                        pdst += 14;
                                        wsize += 14;
                                        break;
                                    }
                                case 208:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((ushort*)(pdst + 5)) = (ushort)v2;
                                        *(pdst + 7) = (byte)(v3);
                                        *(pdst + 8) = (byte)(v4);
                                        pdst += 9;
                                        wsize += 9;
                                        break;
                                    }
                                case 209:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((ushort*)(pdst + 5)) = (ushort)v2;
                                        *(pdst + 7) = (byte)(v3);
                                        *((ushort*)(pdst + 8)) = (ushort)v4;
                                        pdst += 10;
                                        wsize += 10;
                                        break;
                                    }
                                case 210:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((ushort*)(pdst + 5)) = (ushort)v2;
                                        *(pdst + 7) = (byte)(v3);
                                        *((uint*)(pdst + 8)) = (uint)v4;
                                        pdst += 11;
                                        wsize += 11;
                                        break;
                                    }
                                case 211:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((ushort*)(pdst + 5)) = (ushort)v2;
                                        *(pdst + 7) = (byte)(v3);
                                        *((uint*)(pdst + 8)) = (uint)v4;
                                        pdst += 12;
                                        wsize += 12;
                                        break;
                                    }
                                case 212:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((ushort*)(pdst + 5)) = (ushort)v2;
                                        *((ushort*)(pdst + 7)) = (ushort)v3;
                                        *(pdst + 9) = (byte)(v4);
                                        pdst += 10;
                                        wsize += 10;
                                        break;
                                    }
                                case 213:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((ushort*)(pdst + 5)) = (ushort)v2;
                                        *((ushort*)(pdst + 7)) = (ushort)v3;
                                        *((ushort*)(pdst + 9)) = (ushort)v4;
                                        pdst += 11;
                                        wsize += 11;
                                        break;
                                    }
                                case 214:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((ushort*)(pdst + 5)) = (ushort)v2;
                                        *((ushort*)(pdst + 7)) = (ushort)v3;
                                        *((uint*)(pdst + 9)) = (uint)v4;
                                        pdst += 12;
                                        wsize += 12;
                                        break;
                                    }
                                case 215:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((ushort*)(pdst + 5)) = (ushort)v2;
                                        *((ushort*)(pdst + 7)) = (ushort)v3;
                                        *((uint*)(pdst + 9)) = (uint)v4;
                                        pdst += 13;
                                        wsize += 13;
                                        break;
                                    }
                                case 216:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((ushort*)(pdst + 5)) = (ushort)v2;
                                        *((uint*)(pdst + 7)) = (uint)v3;
                                        *(pdst + 10) = (byte)(v4);
                                        pdst += 11;
                                        wsize += 11;
                                        break;
                                    }
                                case 217:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((ushort*)(pdst + 5)) = (ushort)v2;
                                        *((uint*)(pdst + 7)) = (uint)v3;
                                        *((ushort*)(pdst + 10)) = (ushort)v4;
                                        pdst += 12;
                                        wsize += 12;
                                        break;
                                    }
                                case 218:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((ushort*)(pdst + 5)) = (ushort)v2;
                                        *((uint*)(pdst + 7)) = (uint)v3;
                                        *((uint*)(pdst + 10)) = (uint)v4;
                                        pdst += 13;
                                        wsize += 13;
                                        break;
                                    }
                                case 219:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((ushort*)(pdst + 5)) = (ushort)v2;
                                        *((uint*)(pdst + 7)) = (uint)v3;
                                        *((uint*)(pdst + 10)) = (uint)v4;
                                        pdst += 14;
                                        wsize += 14;
                                        break;
                                    }
                                case 220:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((ushort*)(pdst + 5)) = (ushort)v2;
                                        *((uint*)(pdst + 7)) = (uint)v3;
                                        *(pdst + 11) = (byte)(v4);
                                        pdst += 12;
                                        wsize += 12;
                                        break;
                                    }
                                case 221:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((ushort*)(pdst + 5)) = (ushort)v2;
                                        *((uint*)(pdst + 7)) = (uint)v3;
                                        *((ushort*)(pdst + 11)) = (ushort)v4;
                                        pdst += 13;
                                        wsize += 13;
                                        break;
                                    }
                                case 222:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((ushort*)(pdst + 5)) = (ushort)v2;
                                        *((uint*)(pdst + 7)) = (uint)v3;
                                        *((uint*)(pdst + 11)) = (uint)v4;
                                        pdst += 14;
                                        wsize += 14;
                                        break;
                                    }
                                case 223:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((ushort*)(pdst + 5)) = (ushort)v2;
                                        *((uint*)(pdst + 7)) = (uint)v3;
                                        *((uint*)(pdst + 11)) = (uint)v4;
                                        pdst += 15;
                                        wsize += 15;
                                        break;
                                    }
                                case 224:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 5)) = (uint)v2;
                                        *(pdst + 8) = (byte)(v3);
                                        *(pdst + 9) = (byte)(v4);
                                        pdst += 10;
                                        wsize += 10;
                                        break;
                                    }
                                case 225:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 5)) = (uint)v2;
                                        *(pdst + 8) = (byte)(v3);
                                        *((ushort*)(pdst + 9)) = (ushort)v4;
                                        pdst += 11;
                                        wsize += 11;
                                        break;
                                    }
                                case 226:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 5)) = (uint)v2;
                                        *(pdst + 8) = (byte)(v3);
                                        *((uint*)(pdst + 9)) = (uint)v4;
                                        pdst += 12;
                                        wsize += 12;
                                        break;
                                    }
                                case 227:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 5)) = (uint)v2;
                                        *(pdst + 8) = (byte)(v3);
                                        *((uint*)(pdst + 9)) = (uint)v4;
                                        pdst += 13;
                                        wsize += 13;
                                        break;
                                    }
                                case 228:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 5)) = (uint)v2;
                                        *((ushort*)(pdst + 8)) = (ushort)v3;
                                        *(pdst + 10) = (byte)(v4);
                                        pdst += 11;
                                        wsize += 11;
                                        break;
                                    }
                                case 229:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 5)) = (uint)v2;
                                        *((ushort*)(pdst + 8)) = (ushort)v3;
                                        *((ushort*)(pdst + 10)) = (ushort)v4;
                                        pdst += 12;
                                        wsize += 12;
                                        break;
                                    }
                                case 230:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 5)) = (uint)v2;
                                        *((ushort*)(pdst + 8)) = (ushort)v3;
                                        *((uint*)(pdst + 10)) = (uint)v4;
                                        pdst += 13;
                                        wsize += 13;
                                        break;
                                    }
                                case 231:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 5)) = (uint)v2;
                                        *((ushort*)(pdst + 8)) = (ushort)v3;
                                        *((uint*)(pdst + 10)) = (uint)v4;
                                        pdst += 14;
                                        wsize += 14;
                                        break;
                                    }
                                case 232:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 5)) = (uint)v2;
                                        *((uint*)(pdst + 8)) = (uint)v3;
                                        *(pdst + 11) = (byte)(v4);
                                        pdst += 12;
                                        wsize += 12;
                                        break;
                                    }
                                case 233:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 5)) = (uint)v2;
                                        *((uint*)(pdst + 8)) = (uint)v3;
                                        *((ushort*)(pdst + 11)) = (ushort)v4;
                                        pdst += 13;
                                        wsize += 13;
                                        break;
                                    }
                                case 234:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 5)) = (uint)v2;
                                        *((uint*)(pdst + 8)) = (uint)v3;
                                        *((uint*)(pdst + 11)) = (uint)v4;
                                        pdst += 14;
                                        wsize += 14;
                                        break;
                                    }
                                case 235:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 5)) = (uint)v2;
                                        *((uint*)(pdst + 8)) = (uint)v3;
                                        *((uint*)(pdst + 11)) = (uint)v4;
                                        pdst += 15;
                                        wsize += 15;
                                        break;
                                    }
                                case 236:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 5)) = (uint)v2;
                                        *((uint*)(pdst + 8)) = (uint)v3;
                                        *(pdst + 12) = (byte)(v4);
                                        pdst += 13;
                                        wsize += 13;
                                        break;
                                    }
                                case 237:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 5)) = (uint)v2;
                                        *((uint*)(pdst + 8)) = (uint)v3;
                                        *((ushort*)(pdst + 12)) = (ushort)v4;
                                        pdst += 14;
                                        wsize += 14;
                                        break;
                                    }
                                case 238:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 5)) = (uint)v2;
                                        *((uint*)(pdst + 8)) = (uint)v3;
                                        *((uint*)(pdst + 12)) = (uint)v4;
                                        pdst += 15;
                                        wsize += 15;
                                        break;
                                    }
                                case 239:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 5)) = (uint)v2;
                                        *((uint*)(pdst + 8)) = (uint)v3;
                                        *((uint*)(pdst + 12)) = (uint)v4;
                                        pdst += 16;
                                        wsize += 16;
                                        break;
                                    }
                                case 240:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 5)) = (uint)v2;
                                        *(pdst + 9) = (byte)(v3);
                                        *(pdst + 10) = (byte)(v4);
                                        pdst += 11;
                                        wsize += 11;
                                        break;
                                    }
                                case 241:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 5)) = (uint)v2;
                                        *(pdst + 9) = (byte)(v3);
                                        *((ushort*)(pdst + 10)) = (ushort)v4;
                                        pdst += 12;
                                        wsize += 12;
                                        break;
                                    }
                                case 242:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 5)) = (uint)v2;
                                        *(pdst + 9) = (byte)(v3);
                                        *((uint*)(pdst + 10)) = (uint)v4;
                                        pdst += 13;
                                        wsize += 13;
                                        break;
                                    }
                                case 243:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 5)) = (uint)v2;
                                        *(pdst + 9) = (byte)(v3);
                                        *((uint*)(pdst + 10)) = (uint)v4;
                                        pdst += 14;
                                        wsize += 14;
                                        break;
                                    }
                                case 244:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 5)) = (uint)v2;
                                        *((ushort*)(pdst + 9)) = (ushort)v3;
                                        *(pdst + 11) = (byte)(v4);
                                        pdst += 12;
                                        wsize += 12;
                                        break;
                                    }
                                case 245:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 5)) = (uint)v2;
                                        *((ushort*)(pdst + 9)) = (ushort)v3;
                                        *((ushort*)(pdst + 11)) = (ushort)v4;
                                        pdst += 13;
                                        wsize += 13;
                                        break;
                                    }
                                case 246:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 5)) = (uint)v2;
                                        *((ushort*)(pdst + 9)) = (ushort)v3;
                                        *((uint*)(pdst + 11)) = (uint)v4;
                                        pdst += 14;
                                        wsize += 14;
                                        break;
                                    }
                                case 247:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 5)) = (uint)v2;
                                        *((ushort*)(pdst + 9)) = (ushort)v3;
                                        *((uint*)(pdst + 11)) = (uint)v4;
                                        pdst += 15;
                                        wsize += 15;
                                        break;
                                    }
                                case 248:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 5)) = (uint)v2;
                                        *((uint*)(pdst + 9)) = (uint)v3;
                                        *(pdst + 12) = (byte)(v4);
                                        pdst += 13;
                                        wsize += 13;
                                        break;
                                    }
                                case 249:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 5)) = (uint)v2;
                                        *((uint*)(pdst + 9)) = (uint)v3;
                                        *((ushort*)(pdst + 12)) = (ushort)v4;
                                        pdst += 14;
                                        wsize += 14;
                                        break;
                                    }
                                case 250:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 5)) = (uint)v2;
                                        *((uint*)(pdst + 9)) = (uint)v3;
                                        *((uint*)(pdst + 12)) = (uint)v4;
                                        pdst += 15;
                                        wsize += 15;
                                        break;
                                    }
                                case 251:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 5)) = (uint)v2;
                                        *((uint*)(pdst + 9)) = (uint)v3;
                                        *((uint*)(pdst + 12)) = (uint)v4;
                                        pdst += 16;
                                        wsize += 16;
                                        break;
                                    }
                                case 252:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 5)) = (uint)v2;
                                        *((uint*)(pdst + 9)) = (uint)v3;
                                        *(pdst + 13) = (byte)(v4);
                                        pdst += 14;
                                        wsize += 14;
                                        break;
                                    }
                                case 253:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 5)) = (uint)v2;
                                        *((uint*)(pdst + 9)) = (uint)v3;
                                        *((ushort*)(pdst + 13)) = (ushort)v4;
                                        pdst += 15;
                                        wsize += 15;
                                        break;
                                    }
                                case 254:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 5)) = (uint)v2;
                                        *((uint*)(pdst + 9)) = (uint)v3;
                                        *((uint*)(pdst + 13)) = (uint)v4;
                                        pdst += 16;
                                        wsize += 16;
                                        break;
                                    }
                                case 255:
                                    {
                                        *((uint*)(pdst + 1)) = (uint)v1;
                                        *((uint*)(pdst + 5)) = (uint)v2;
                                        *((uint*)(pdst + 9)) = (uint)v3;
                                        *((uint*)(pdst + 13)) = (uint)v4;
                                        pdst += 17;
                                        wsize += 17;
                                        break;
                                    }
                                default:
                                    break;
                            }



                            psrc += 4;

                        }
                    }

                }
            }
            return wsize;
        }

        public unsafe static int DecodeForTest(byte[] buffer, int bpos, int bsize, uint[] array, ref int ap)
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
                        //*(aps + 3) = (*((uint*)(bs + 3)) >> 8);
                        //uint v = *((uint*)(bs + 1));
                        //*(aps + 0) = v >> 24;
                        //*(aps + 1) = (byte)(v >> 16);
                        //*(aps + 2) = (byte)(v >> 8);
                        //*(aps + 3) = (byte)v;

                        //byte* v = &(*((uint*)(bs + 1)))[0];
                        //*aps = *(v + 0);
                        //*(aps + 1) = *(v + 1);
                        //*(aps + 2) = *(v + 2);
                        //*(aps + 3) = *(v + 3);

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

        #endregion

    }
}
