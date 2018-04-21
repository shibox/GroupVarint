using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupVarint.Tools
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateEncode();
            CreateDecode();
        }

        /// <summary>
        /// 创建编码的代码
        /// </summary>
        public static void CreateEncode()
        {
            File.WriteAllText("GroupVarintEncode.cs", GroupVarintCreater.CreateFastEncode());
        }

        /// <summary>
        /// 创建解码的代码
        /// </summary>
        public static void CreateDecode()
        {
            File.WriteAllText("GroupVarintDecode.cs", GroupVarintCreater.CreateFastDecodeNew());
        }

    }
}
