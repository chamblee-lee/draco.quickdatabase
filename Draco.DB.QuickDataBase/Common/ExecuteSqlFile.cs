using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.IO;

namespace Draco.DB.QuickDataBase.Common
{
    /// <summary>
    /// 执行sql文件工具类
    /// </summary>
    public class ExecuteSqlFile
    {
        private IDataBaseHandler m_handler;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="handler">数据库操作接口</param>
        public ExecuteSqlFile(IDataBaseHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException();
            m_handler = handler;
        }

        /// <summary>
        /// Sql语句中用户分割多行Sql语句的分隔符
        /// </summary>
        public const string SqlSplitChars = "--#";//使用SQL注释符作为缺省分隔符

        /// <summary>
        /// 执行sql文件
        /// </summary>
        /// <param name="filepath">文件路径</param>
        public int ExecuteFileSql(string filepath)
        {
            if (String.IsNullOrEmpty(filepath) || !File.Exists(filepath))
                throw new FileNotFoundException("无法找到文件：" + filepath);

            StreamReader sr = new StreamReader(filepath);
            string separator = sr.ReadLine();//首行必须是定义分隔符

            if (String.IsNullOrEmpty(separator))
                separator = SqlSplitChars;

            string sql = sr.ReadToEnd();
            sr.Dispose();//撤销对文件资源的占用
            return ExecuteSqlScript(sql, separator);
        }

        /// <summary>
        /// 执行文件中所有sql语句
        /// </summary>
        /// <param name="SqlText">sql语句集</param>
        /// <param name="separator">分隔符</param>
        /// <returns></returns>
        public int ExecuteSqlScript(string SqlText, string separator)
        {
            if (String.IsNullOrEmpty(SqlText))
                return 0;

            int TotalCount = 0;
            using (DbTransactionScope Scope = new DbTransactionScope(this.m_handler))
            {
                StringBuilder sb = new StringBuilder();
                while (true)
                {
                    //判断剩余待执行的语句集中是否还有语句
                    if (SqlText.Length == 0 || SqlText == null)
                    {
                        //执行最后一个缓存语句
                        if (sb.ToString().Trim().Length > 0)
                        {
                            int tmprtn = this.m_handler.ExecuteNonQuery(sb.ToString());
                            sb.Remove(0, sb.Length);
                            TotalCount += tmprtn;
                        }
                        break;
                    }

                    string spsql = this.ReadLineStr(ref SqlText);
                    int spi = spsql.IndexOf(separator);

                    if (spi != -1)
                    {
                        string temsql = spsql.Substring(0, spi);
                        sb.Append(temsql);

                        //判断是否为无效语句
                        string strtmp = sb.ToString().Replace("\n", "").Replace("\r", "").Replace("\t", "").Replace(" ", "");
                        if (strtmp.Length == 0)
                            continue;

                        int tmprtn = this.m_handler.ExecuteNonQuery(sb.ToString());
                        sb.Remove(0, sb.Length);
                        TotalCount += tmprtn;
                    }
                    else
                    {
                        sb.Append(spsql);
                    }
                }
                Scope.Complete();
            }
            return TotalCount;
        }

        /// <summary>
        /// 读取一行字符串，并且在原始字符串中剪去
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <returns>读取出的字符串</returns>
        private string ReadLineStr(ref string str)
        {
            string s="";
            int ni = str.IndexOf("\n");
            if (ni != -1)
            {
                s = str.Substring(0, ni + 1);
                str = str.Substring(ni + 1);
            }
            else
            {
                //最后一句，没有换行时的处理
                s = str;
                str = "";
            }
            return s;
        }
    }
}
