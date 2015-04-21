using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Data;

namespace Draco.DB.QuickDataBase.Common
{
    /// <summary>
    /// 数据库操作入口
    /// </summary>
    [ComVisible(true)]
    public abstract class DataBaseEntry
    {
        /// <summary>
        /// 把参数对象中的参数填充到cmd对象，并且设置cmd对象的连接对象，CommandType，cmdText
        /// </summary>
        /// <param name="conn">连接对象，可以为空</param>
        /// <param name="cmd">DataCommand对象，不能为空</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">命令</param>
        /// <param name="commandParameters">参数对象</param>
        public static void PrepareCommand(IDbConnection conn, IDbCommand cmd, CommandType cmdType, string cmdText,
            IDataParameter[] commandParameters)
        {
            if (cmd == null)
                throw new ArgumentNullException("cmd");

            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            cmd.CommandType = cmdType;

            if (commandParameters != null)
            {
                foreach (IDataParameter parm in commandParameters)
                {
                    if (parm != null)
                    {
                        cmd.Parameters.Add(parm);
                    }
                }
            }
        }

        /// <summary>
        /// 执行cmd的Reader
        /// </summary>
        /// <param name="cmd">命令对象</param>
        /// <returns>IDataReader</returns>
        public static IDataReader IExecuteReader(IDbCommand cmd)
        {
            if (cmd == null)
                throw new ArgumentNullException("cmd");
            try
            {
                if (cmd.Transaction == null)
                    return cmd.ExecuteReader(CommandBehavior.CloseConnection);
                else
                    return cmd.ExecuteReader();

            }
            catch (Exception ee)
            {
                throw new System.Data.DataException(cmd.CommandText, ee);
            }
            finally
            {
                cmd.Dispose();//不能在这里关闭.
            }

        }
        /// <summary>
        /// 执行DataAdapter，填充DataSet，返回取得行数
        /// </summary>
        /// <param name="Ds">传出的DataSet</param>
        /// <param name="dataAdapter">配适器</param>
        /// <returns>取得行数</returns>
        public static int IExecuteDataAdapter(out DataSet Ds, IDbDataAdapter dataAdapter)
        {
            if (dataAdapter == null)
                throw new ArgumentNullException("dataAdapter");
            Ds = new DataSet();
            Ds.Locale = System.Globalization.CultureInfo.InvariantCulture;
            try
            {
                //Fill 之前 IDbConnection 已关闭，则将其打开以检索数据，然后再将其关闭。
                //如果调用 Fill 之前连接已打开，它将保持打开状态。
                return dataAdapter.Fill(Ds);
            }
            catch (Exception ee)
            {
                throw new System.Data.DataException(dataAdapter.SelectCommand.CommandText, ee);
            }
        }
        /// <summary>
        /// 执行Scalar
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public static object IExecuteScalar(IDbCommand cmd)
        {
            if (cmd == null)
                throw new ArgumentNullException("cmd");
            try
            {
                return cmd.ExecuteScalar();
            }
            catch (Exception ee)
            {
                throw new System.Data.DataException(cmd.CommandText, ee);
            }
            finally
            {
                if (cmd.Transaction == null)//没有事物时关闭连接
                {
                    cmd.Connection.Close();
                    cmd.Dispose();
                }
                cmd.Dispose();
            }
        }
        /// <summary>
        /// 执行非查询Sql
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public static int IExecuteNonQuery(IDbCommand cmd)
        {
            if (cmd == null)
                throw new ArgumentNullException("cmd");
            try
            {
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ee)
            {
                throw new System.Data.DataException(cmd.CommandText, ee);
            }
            finally
            {
                if (cmd.Transaction == null)//没有事物时关闭连接
                {
                    cmd.Connection.Close();
                    cmd.Dispose();
                }
            }
        }
    }
}
