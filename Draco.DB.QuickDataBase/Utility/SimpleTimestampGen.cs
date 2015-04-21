using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Draco.DB.QuickDataBase.Utility
{
    /// <summary>
    /// 简单时间戳(可在1ms内产生1千万非重复时间戳)
    /// </summary>
    public class SimpleTimestampGen
    {
        /// <summary>
        /// 生成简单时间戳
        /// </summary>
        /// <returns></returns>
        public static String Generate()
        {
            DateTime date = DateTime.Now;
            string timestamp = date.ToString("yyyyMMddHHmmssfff");
            timestamp += GetPerformanceCounter();
            return timestamp;   //17位时间+7位计数器=24位时间戳
        }

        /// <summary>
        /// 获取当前CPU计数器计数
        /// </summary>
        /// <returns></returns>
        private static string GetPerformanceCounter()
        {
            long Counter = 0;
            if (QueryPerformanceCounter(out Counter))
            {
                string strCounter = Counter.ToString();
                while (strCounter.Length < 7)
                    strCounter = "0" + strCounter;
                return strCounter.Substring(strCounter.Length - 7);
            }
            throw new Exception("获取计数器失败!QueryPerformanceCounter返回失败！");
        }

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);
    }
}
