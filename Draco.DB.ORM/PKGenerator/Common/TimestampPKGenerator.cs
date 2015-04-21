using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Threading;

namespace Draco.DB.ORM.PKGenerator.Common
{
    /// <summary>
    /// 时间戳主键生成器
    /// </summary>
    public class TimestampPKGenerator : IPKGenerator
    {
        /// <summary>
        /// 是否可复用
        /// </summary>
        public bool IsReusable
        {
            get { return true; }
        }
        
        /// <summary>
        /// 生成下一个主键
        /// </summary>
        /// <param name="Mapping"></param>
        /// <returns></returns>
        public Hashtable GeneratNextPrimaryKey(Draco.DB.ORM.Mapping.ITableMapping Mapping)
        {
            Hashtable table = new Hashtable();
            foreach (var key in Mapping.PrimaryKeyCollection)
            {
                table.Add(key.PropertyName, Generate());
            }
            return table;
        }

        
        /// <summary>
        /// 生成时间戳(32位数字)
        /// </summary>
        /// <returns></returns>
        public static String Generate()
        {
            DateTime date = DateTime.Now;
            String timestamp = date.ToString("yyyyMMddHHmmssfff");

            Random rnd = new Random(Environment.TickCount);//伪随机数
            Double x = rnd.NextDouble();

            String sx = x.ToString().Substring(3, 3); //(这个值同一机器在同一毫秒内会产生相同的值)

            timestamp += GetSequenceNo(DateTime.Now.Ticks) + GetRandomNumberFromGUID(8) + sx;
            return timestamp;
        }
        private static long lastTime;
        private static int SequenceNo = 0;
        private static String GetSequenceNo(long time)
        {
            if (lastTime == time)
            {
                SequenceNo++;
                if (SequenceNo > 9999)
                {
                    try
                    {
                        Thread.Sleep(1);
                        return GetSequenceNo(DateTime.Now.Ticks);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            else
            {
                lastTime = time;
                Random r = new Random();
                SequenceNo = (int)(r.NextDouble() * 256);
            }
            String v = SequenceNo.ToString();
            while (v.Length < 4)
            {
                v = "0" + v;
            }
            return v;
        }

        private static string GetRandomNumberFromGUID(int length)
        {
            byte[] GByte = Guid.NewGuid().ToByteArray();
            byte[] b = new byte[4] { GByte[0], GByte[1], GByte[2], GByte[3] };
            string Num = Math.Abs(BitConverter.ToInt32(b, 0)).ToString();
            while (Num.Length < length)
            {
                Num = "0" + Num;
            }
            Num = Num.Substring(Num.Length - 5);
            return Num;
        }
    }
}
