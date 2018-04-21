using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupVarint.Tools
{
    /// <summary>
    /// GroupVarint解码模板代码生成器
    /// </summary>
    public class GroupVarintCreater
    {
        #region 公共

        public static string CreateFastEncode()
        {
            int l1 = 0, l2 = 0, l3 = 0, l4 = 0;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("switch (b)");
            sb.AppendLine("{");
            for (int i = 0; i < 256; i++)
            {
                sb.AppendLine(string.Format("case {0}:", i));
                sb.AppendLine("{");

                SizeofEecode(sb, i, ref l1, ref l2, ref l3, ref l4);

                sb.AppendLine("break;");
                sb.AppendLine("}");
            }
            sb.AppendLine("default:");
            sb.AppendLine("break;");
            sb.AppendLine("}");
            return sb.ToString();
        }

        /// <summary>
        /// 创建非指针版解码代码
        /// </summary>
        /// <returns></returns>
        public static string CreateDecode()
        {
            int l1 = 0, l2 = 0, l3 = 0, l4 = 0;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("switch (buffer[bpos])");
            sb.AppendLine("{");
            for (int i = 0; i < 256; i++)
            {
                sb.AppendLine(string.Format("case {0}:", i));
                sb.AppendLine("{");

                int p = 1;
                Sizeof(i, ref l1, ref l2, ref l3, ref l4);
                Write(sb, l1, p, 0);
                p += l1 + 1;
                Write(sb, l2, p, 1);
                p += l2 + 1;
                Write(sb, l3, p, 2);
                p += l3 + 1;
                Write(sb, l4, p, 3);

                int vs = Sizeof(i);
                sb.AppendLine(string.Format("bpos += {0};", vs));
                sb.AppendLine("apos += 4;");
                sb.AppendLine("break;");
                sb.AppendLine("}");
            }
            sb.AppendLine("default:");
            sb.AppendLine("break;");
            sb.AppendLine("}");
            //byte b = buffer[bpos];
            //switch (b)
            //{
            //    case 0:
            //        {

            //            break;
            //        }
            //    case 255:
            //        {
            //            array[apos] = (int)((b + 1) << 24) | (int)(buffer[bpos + 2] << 16) | (int)(buffer[bpos + 3] << 8) | (int)(buffer[bpos + 4]);
            //            array[apos + 1] = (int)((b + 5) << 24) | (int)(buffer[bpos + 6] << 16) | (int)(buffer[bpos + 7] << 8) | (int)(buffer[bpos + 8]);
            //            array[apos + 2] = (int)((b + 9) << 24) | (int)(buffer[bpos + 10] << 16) | (int)(buffer[bpos + 11] << 8) | (int)(buffer[bpos + 12]);
            //            array[apos + 3] = (int)((b + 13) << 24) | (int)(buffer[bpos + 14] << 16) | (int)(buffer[bpos + 15] << 8) | (int)(buffer[bpos + 16]);
            //            bpos += 17;
            //            apos += 4;
            //            break;
            //        }
            //    default:
            //        break;
            //}
            return sb.ToString();
        }

        /// <summary>
        /// 创建指针版解码代码
        /// </summary>
        /// <returns></returns>
        public static string CreateFastDeode()
        {
            int l1 = 0, l2 = 0, l3 = 0, l4 = 0;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("switch (*bs)");
            sb.AppendLine("{");
            for (int i = 0; i < 256; i++)
            {
                sb.AppendLine(string.Format("case {0}:", i));
                sb.AppendLine("{");
                sb.AppendLine("bs++;");
                int p = 1;
                Sizeof(i, ref l1, ref l2, ref l3, ref l4);
                WriteFast(sb, l1, p, 0);
                p += l1 + 1;
                WriteFast(sb, l2, p, 1);
                p += l2 + 1;
                WriteFast(sb, l3, p, 2);
                p += l3 + 1;
                WriteFast(sb, l4, p, 3);

                int vs = Sizeof(i);
                sb.AppendLine(string.Format("bpos += {0};", vs));
                sb.AppendLine("apos += 4;");
                sb.AppendLine("break;");
                sb.AppendLine("}");
            }
            sb.AppendLine("default:");
            sb.AppendLine("break;");
            sb.AppendLine("}");
            return sb.ToString();
        }

        public static string CreateFastDecodeNew()
        {
            int l1 = 0, l2 = 0, l3 = 0, l4 = 0;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("switch (*bs)");
            sb.AppendLine("{");
            for (int i = 0; i < 256; i++)
            {
                sb.AppendLine(string.Format("case {0}:", i));
                sb.AppendLine("{");
                SizeofDecode(sb, i, ref l1, ref l2, ref l3, ref l4);
                sb.AppendLine("break;");
                sb.AppendLine("}");
            }
            sb.AppendLine("default:");
            sb.AppendLine("break;");
            sb.AppendLine("}");
            return sb.ToString();
        }

        #endregion

        #region 私有

        private static void Write(StringBuilder sb, int lv, int p, int n)
        {
            //array[apos + 1] = (int)((b + 5) << 24) | (int)(buffer[bpos + 6] << 16) | (int)(buffer[bpos + 7] << 8) | (int)(buffer[bpos + 8]);
            if (n == 0)
                sb.Append("array[apos] = ");
            else
                sb.Append(string.Format("array[apos + {0}] = ", n));

            string[] temp = new string[lv + 1];
            for (int i = 0; i <= lv; i++)
            {
                if (i == lv)
                    temp[i] = string.Format("(buffer[bpos + {0}])", p + i);
                else
                    temp[i] = string.Format("((buffer[bpos + {0}]) << {1})", p + i, (lv - i) * 8);
                //temp[i] = string.Format("((b + {0}) << {1})", p + i, (lv - i) * 8);
            }
            sb.Append(string.Join(" | ", temp));
            sb.Append(";");
            sb.AppendLine();



            //array[apos] = (buffer[bpos + 1]);
            //array[apos + 1] = (buffer[bpos + 2]);
            //array[apos + 2] = (buffer[bpos + 3]) | ((buffer[bpos + 4]) << 0);
            //array[apos + 3] = (buffer[bpos + 5]) | ((buffer[bpos + 6]) << 0);
            //bpos += 7;
            //apos += 4;
        }

        private static void WriteFast(StringBuilder sb, int lv, int p, int n)
        {
            //array[apos + 1] = (int)((b + 5) << 24) | (int)(buffer[bpos + 6] << 16) | (int)(buffer[bpos + 7] << 8) | (int)(buffer[bpos + 8]);
            //if (n == 0)
            //    sb.Append("array[apos] = ");
            //else
            //    sb.Append(string.Format("array[apos + {0}] = ", n));

            sb.Append("*aps++ = ");
            string[] temp = new string[lv + 1];
            for (int i = 0; i <= lv; i++)
            {
                //if (i == lv)
                //    temp[i] = string.Format("(buffer[bpos + {0}])", p + i);
                //else
                //    temp[i] = string.Format("((buffer[bpos + {0}]) << {1})", p + i, (lv - i) * 8);

                if (i == lv)
                    temp[i] = string.Format("(*bs++)");
                else
                    temp[i] = string.Format("((*bs++) << {0})", (lv - i) * 8);
            }
            sb.Append(string.Join(" | ", temp));
            sb.Append(";");
            sb.AppendLine();
            //array[apos] = (buffer[bpos + 1]);
            //array[apos + 1] = (buffer[bpos + 2]);
            //array[apos + 2] = (buffer[bpos + 3]) | ((buffer[bpos + 4]) << 0);
            //array[apos + 3] = (buffer[bpos + 5]) | ((buffer[bpos + 6]) << 0);
            //bpos += 7;
            //apos += 4;


            //bs++;
            //*aps++ = *bs++;
            //*aps++ = *bs++;
            //*aps++ = *bs++;
            //*aps++ = *bs++;

        }

        private static void WriteFast1(StringBuilder sb, int lv, int p, int n)
        {
            //sb.Append("*aps++ = ");
            string[] temp = new string[lv + 1];
            for (int i = 0; i <= lv; i++)
            {
                //if (i == lv)
                //    temp[i] = string.Format("(buffer[bpos + {0}])", p + i);
                //else
                //    temp[i] = string.Format("((buffer[bpos + {0}]) << {1})", p + i, (lv - i) * 8);

                if (i == lv)
                    temp[i] = string.Format("(*(bs+1))");
                else
                    temp[i] = string.Format("((*bs++) << {0})", (lv - i) * 8);
            }
            sb.Append(string.Join(" | ", temp));
            sb.Append(";");
            sb.AppendLine();
        }

        private static int Sizeof(int prefix)
        {
            int v = 1;
            v += ((prefix & 0xC0) >> 6) + 1;
            v += ((prefix & 0x30) >> 4) + 1;
            v += ((prefix & 0x0C) >> 2) + 1;
            v += ((prefix & 0x03) >> 0) + 1;
            return v;
        }

        private static void Sizeof(int prefix, ref int l1, ref int l2, ref int l3, ref int l4)
        {
            l1 = (prefix & 0xC0) >> 6;
            l2 = (prefix & 0x30) >> 4;
            l3 = (prefix & 0x0C) >> 2;
            l4 = (prefix & 0x03) >> 0;
        }

        private static void SizeofDecode(StringBuilder sb, int prefix, ref int l1, ref int l2, ref int l3, ref int l4)
        {
            int n = 1;
            l1 = (prefix & 0xC0) >> 6;
            l2 = (prefix & 0x30) >> 4;
            l3 = (prefix & 0x0C) >> 2;
            l4 = (prefix & 0x03) >> 0;

            if (l1 == 0)
                sb.AppendLine(string.Format("*aps = *(bs+{0});", n));
            else if (l1 == 1)
                sb.AppendLine(string.Format("*aps = *((ushort*)(bs+{0}));", n));
            else if (l1 == 2)
                if (n - 1 != 0)
                    sb.AppendLine(string.Format("*aps = (*((uint*)(bs+{0})) >> 8);", n - 1));
                else
                    sb.AppendLine(string.Format("*aps = (*((uint*)(bs)) >> 8);", n - 1));
            else if (l1 == 3)
                sb.AppendLine(string.Format("*aps = *((uint*)(bs+{0}));", n));
            n += l1 + 1;

            if (l2 == 0)
                sb.AppendLine(string.Format("*(aps+1) = *(bs+{0});", n));
            else if (l2 == 1)
                sb.AppendLine(string.Format("*(aps+1) = *((ushort*)(bs+{0}));", n));
            else if (l2 == 2)
                sb.AppendLine(string.Format("*(aps+1) = (*((uint*)(bs+{0})) >> 8);", n - 1));
            else if (l2 == 3)
                sb.AppendLine(string.Format("*(aps+1) = *((uint*)(bs+{0}));", n));
            n += l2 + 1;

            if (l3 == 0)
                sb.AppendLine(string.Format("*(aps+2) = *(bs+{0});", n));
            else if (l3 == 1)
                sb.AppendLine(string.Format("*(aps+2) = *((ushort*)(bs+{0}));", n));
            else if (l3 == 2)
                sb.AppendLine(string.Format("*(aps+2) = (*((uint*)(bs+{0})) >> 8);", n - 1));
            else if (l3 == 3)
                sb.AppendLine(string.Format("*(aps+2) = *((uint*)(bs+{0}));", n));
            n += l3 + 1;

            if (l4 == 0)
                sb.AppendLine(string.Format("*(aps+3) = *(bs+{0});", n));
            else if (l4 == 1)
                sb.AppendLine(string.Format("*(aps+3) = *((ushort*)(bs+{0}));", n));
            else if (l4 == 2)
                sb.AppendLine(string.Format("*(aps+3) = (*((uint*)(bs+{0})) >> 8);", n - 1));
            else if (l4 == 3)
                sb.AppendLine(string.Format("*(aps+3) = *((uint*)(bs+{0}));", n));
            n += l4 + 1;
            sb.AppendLine(string.Format("bs += {0};", n));
            sb.AppendLine(string.Format("bpos += {0};", n));



            //if (l1 == 0)
            //    sb.AppendLine(string.Format("*aps = *(bs+{0});", n));
            //else if (l1 == 1)
            //    sb.AppendLine(string.Format("*aps = *((ushort*)(bs+{0}));", n));
            //else if (l1 == 2)
            //    if (n - 1 != 0)
            //        sb.AppendLine(string.Format("*aps = (*((int*)(bs+{0})) >> 8);", n - 1));
            //    else
            //        sb.AppendLine(string.Format("*aps = (*((int*)(bs)) >> 8);", n - 1));
            //else if (l1 == 3)
            //    sb.AppendLine(string.Format("*aps = *((int*)(bs+{0}));", n));
            //n += l1 + 1;

            //if (l2 == 0)
            //    sb.AppendLine(string.Format("*(aps+1) = *(bs+{0});", n));
            //else if (l2 == 1)
            //    sb.AppendLine(string.Format("*(aps+1) = *((ushort*)(bs+{0}));", n));
            //else if (l2 == 2)
            //    sb.AppendLine(string.Format("*(aps+1) = (*((int*)(bs+{0})) >> 8);", n-1));
            //else if (l2 == 3)
            //    sb.AppendLine(string.Format("*(aps+1) = *((int*)(bs+{0}));", n));
            //n += l2 + 1;

            //if (l3 == 0)
            //    sb.AppendLine(string.Format("*(aps+2) = *(bs+{0});", n));
            //else if (l3 == 1)
            //    sb.AppendLine(string.Format("*(aps+2) = *((ushort*)(bs+{0}));", n));
            //else if (l3 == 2)
            //    sb.AppendLine(string.Format("*(aps+2) = (*((int*)(bs+{0})) >> 8);", n-1));
            //else if (l3 == 3)
            //    sb.AppendLine(string.Format("*(aps+2) = *((int*)(bs+{0}));", n));
            //n += l3 + 1;

            //if (l4 == 0)
            //    sb.AppendLine(string.Format("*(aps+3) = *(bs+{0});", n));
            //else if (l4 == 1)
            //    sb.AppendLine(string.Format("*(aps+3) = *((ushort*)(bs+{0}));", n));
            //else if (l4 == 2)
            //    sb.AppendLine(string.Format("*(aps+3) = (*((int*)(bs+{0})) >> 8);", n-1));
            //else if (l4 == 3)
            //    sb.AppendLine(string.Format("*(aps+3) = *((int*)(bs+{0}));", n));
            //n += l4 + 1;
            //sb.AppendLine(string.Format("bs += {0};", n));
            //sb.AppendLine(string.Format("bpos += {0};", n));
            //------------------------------------------------------------------------
            //if (l1 == 0)
            //    sb.AppendLine(string.Format("*aps = *(bs+{0});", n));
            //else if (l1 == 1)
            //    sb.AppendLine(string.Format("*aps = *((short*)(bs+{0}));", n));
            //else if (l1 == 2)
            //    sb.AppendLine(string.Format("*aps = (*((int*)(bs+{0})) >> 8);", n));
            //else if (l1 == 3)
            //    sb.AppendLine(string.Format("*aps = *((int*)(bs+{0}));", n));
            //n += l1+1;

            //if (l2 == 0)
            //    sb.AppendLine(string.Format("*(aps+1) = *(bs+{0});", n));
            //else if (l2 == 1)
            //    sb.AppendLine(string.Format("*(aps+1) = *((short*)(bs+{0}));", n));
            //else if (l2 == 2)
            //    sb.AppendLine(string.Format("*(aps+1) = (*((int*)(bs+{0})) >> 8);", n));
            //else if (l2 == 3)
            //    sb.AppendLine(string.Format("*(aps+1) = *((int*)(bs+{0}));", n));
            //n += l2+1;

            //if (l3 == 0)
            //    sb.AppendLine(string.Format("*(aps+2) = *(bs+{0});", n));
            //else if (l3 == 1)
            //    sb.AppendLine(string.Format("*(aps+2) = *((short*)(bs+{0}));", n));
            //else if (l3 == 2)
            //    sb.AppendLine(string.Format("*(aps+2) = (*((int*)(bs+{0})) >> 8);", n));
            //else if (l3 == 3)
            //    sb.AppendLine(string.Format("*(aps+2) = *((int*)(bs+{0}));", n));
            //n += l3+1;

            //if (l4 == 0)
            //    sb.AppendLine(string.Format("*(aps+3) = *(bs+{0});", n));
            //else if (l4 == 1)
            //    sb.AppendLine(string.Format("*(aps+3) = *((short*)(bs+{0}));", n));
            //else if (l4 == 2)
            //    sb.AppendLine(string.Format("*(aps+3) = (*((int*)(bs+{0})) >> 8);", n));
            //else if (l4 == 3)
            //    sb.AppendLine(string.Format("*(aps+3) = *((int*)(bs+{0}));", n));
            //n += l4 + 1;
            //sb.AppendLine(string.Format("bs += {0};", n));
            //sb.AppendLine(string.Format("bpos += {0};", n));
        }

        private static void SizeofEecode(StringBuilder sb, int prefix, ref int l1, ref int l2, ref int l3, ref int l4)
        {
            int n = 1;
            l1 = (prefix & 0xC0) >> 6;
            l2 = (prefix & 0x30) >> 4;
            l3 = (prefix & 0x0C) >> 2;
            l4 = (prefix & 0x03) >> 0;

            if (l1 == 0)
                sb.AppendLine(string.Format("*(pdst+{0}) = (byte)(v1);", n));
            else if (l1 == 1)
                sb.AppendLine(string.Format("*((ushort*)(pdst + {0})) = (ushort)v1;", n));
            else if (l1 == 2)
                sb.AppendLine(string.Format("*((uint*)(pdst + {0})) = (uint)v1;", n));
            else if (l1 == 3)
                sb.AppendLine(string.Format("*((uint*)(pdst + {0})) = (uint)v1;", n));
            n += l1 + 1;

            if (l2 == 0)
                sb.AppendLine(string.Format("*(pdst+{0}) = (byte)(v2);", n));
            else if (l2 == 1)
                sb.AppendLine(string.Format("*((ushort*)(pdst + {0})) = (ushort)v2;", n));
            else if (l2 == 2)
                sb.AppendLine(string.Format("*((uint*)(pdst + {0})) = (uint)v2;", n));
            else if (l2 == 3)
                sb.AppendLine(string.Format("*((uint*)(pdst + {0})) = (uint)v2;", n));
            n += l2 + 1;

            if (l3 == 0)
                sb.AppendLine(string.Format("*(pdst+{0}) = (byte)(v3);", n));
            else if (l3 == 1)
                sb.AppendLine(string.Format("*((ushort*)(pdst + {0})) = (ushort)v3;", n));
            else if (l3 == 2)
                sb.AppendLine(string.Format("*((uint*)(pdst + {0})) = (uint)v3;", n));
            else if (l3 == 3)
                sb.AppendLine(string.Format("*((uint*)(pdst + {0})) = (uint)v3;", n));
            n += l3 + 1;

            if (l4 == 0)
                sb.AppendLine(string.Format("*(pdst+{0}) = (byte)(v4);", n));
            else if (l4 == 1)
                sb.AppendLine(string.Format("*((ushort*)(pdst + {0})) = (ushort)v4;", n));
            else if (l4 == 2)
                sb.AppendLine(string.Format("*((uint*)(pdst + {0})) = (uint)v4;", n));
            else if (l4 == 3)
                sb.AppendLine(string.Format("*((uint*)(pdst + {0})) = (uint)v4;", n));
            n += l4 + 1;
            sb.AppendLine(string.Format("pdst += {0};", n));
            sb.AppendLine(string.Format("wsize += {0};", n));

        }

        #endregion

    }
}
