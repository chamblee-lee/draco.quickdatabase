using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Draco.DB.QuickDataBase.Utility
{
    /// <summary>
    /// 时间戳生成器
    /// </summary>
    public class TimestampGenerator
    {
        /// <summary>
        /// 生成时间戳
        /// </summary>
        /// <returns></returns>
        public static String Generate()
        {
            DateTime date = DateTime.Now;
            string timestamp = date.ToString("yyyyMMddHHmmssfff"); ;

            Random rnd = new Random(Environment.TickCount);
            Double x = rnd.NextDouble();

            String sx = x.ToString().Substring(3, 3);

            timestamp += getSequenceNo(date.Ticks) + GetRandomNumberFromGUID(8) + sx;

            Console.WriteLine(timestamp);
            return timestamp;
        }
        private static long lastTime;
        private static int SequenceNo = 0;
        private static String getSequenceNo(long time)
        {
            if (lastTime == time)
            {
                SequenceNo++;
                if (SequenceNo > 9999)
                {
                    try
                    {
                        Thread.Sleep(1);
                        return getSequenceNo(DateTime.Now.Ticks);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            else
            {
                lastTime = time;
                SequenceNo = new Random().Next(0,256);
            }
            String v = Convert.ToString(SequenceNo);
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
