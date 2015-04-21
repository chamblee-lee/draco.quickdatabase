using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Draco.DB.ORM.PKGenerator.Common
{
    /// <summary>
    /// 简单GUID主键生成器
    /// </summary>
    public class SimpleGUIDPKGenerator : IPKGenerator
    {
        private static String kHexChars = "0123456789abcdefABCDEF";

        /// <summary>
        /// 是否可复用
        /// </summary>
        public bool IsReusable
        {
            get { return true; }
        }
        /// <summary>
        /// 创建下一主键值
        /// </summary>
        /// <param name="Mapping"></param>
        /// <returns></returns>
        public Hashtable GeneratNextPrimaryKey(Draco.DB.ORM.Mapping.ITableMapping Mapping)
        {
            Hashtable table = new Hashtable();
            foreach (var key in Mapping.PrimaryKeyCollection)
            {
                table.Add(key.PropertyName, GetSimpleGuid());
            }
            return table;
        }

        /**
	 * 得到基于时间的简化GUID,由GUID的0-8，13-15字节组成
	 * @return
	 */
        public static String GetSimpleGuid()
        {
            //byte[] b = GetSimpleGuidBytes();
            //return bytesToHex(b);
            //用时间戳的简化方式作为简化GUID
            return TimestampPKGenerator.Generate().Substring(2, 24);
        }
        /**
	     * 得到简化的GUID,由0-8，13-15位组成
	     * @return
	     */
        private static byte[] GetSimpleGuidBytes()
        {
            byte[] b = GetGuidBytes();
            byte[] ret = new byte[12];

            for (int i = 0, j = 0; i < 16; ++i)
            {
                if (i > 8 && i < 13)
                    continue;
                ret[j] = b[i];
                j++;
            }
            return ret;
        }
        private static byte[] GetGuidBytes()
        {
            byte[] b = Guid.NewGuid().ToByteArray();

            byte[] ret = new byte[16];
            // 进行转换 313a1021-ae8b-11df-8e91-dbe2312c1bef->1df ae8b 313a11021 8e91
            // dbe2312c1bef,保证第12个字符是1，且年份排列在前
            ret[0] = getByte(b[6], b[7]);
            ret[1] = getByte(b[7], b[4]);
            ret[2] = getByte(b[4], b[5]);
            ret[3] = getByte(b[5], b[0]);
            ret[4] = getByte(b[0], b[1]);
            ret[5] = getByte(b[1], b[8]);
            ret[6] = (byte)(b[6] & 0xf0 | b[8] & 0x0f);

            ret[14] = b[2];
            ret[15] = b[3];
            for (int i = 7; i < 14; i++)
                ret[i] = b[i + 2];
            return ret;
        }
        /**
	     * 取a的低位，b高位生成字符，如 a:0xabcd b:0x1234 =>0xcd12
	     * 
	     * @param a
	     * @param b
	     * @return
	     */
        private static byte getByte(byte a, byte b)
        {
            a = (byte)((byte)(a << 4) & 0xF0);
            b = (byte)((byte)(b >> 4) & 0x0F);

            return (byte)(a | b);
        }
        /**
	     * 字节转为16进制
	     * @param b
	     * @return
	     */
        public static String bytesToHex(byte[] b)
        {
            StringBuilder sb = new StringBuilder(36);
            for (int i = 0; i < b.Length; ++i)
            {
                int hex = b[i] & 0xFF;
                sb.Append(kHexChars[hex >> 4]);
                sb.Append(kHexChars[hex & 0x0f]);
            }
            return sb.ToString();
        }
    }
}
