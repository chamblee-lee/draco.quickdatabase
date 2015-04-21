using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Collections;
using Draco.DB.QuickDataBase.Adapter;
using Draco.DB.QuickDataBase.Threading;

namespace Draco.DB.QuickDataBase.Common
{
    /// <summary>
    /// 数据库类型无关的操作
    /// </summary>
    public class DataBaseHandler : IDataBaseHandler 
    {
        private string m__HandlerID;
        private Hashtable m_Hash;
        private IDataBaseContext m_Context;
        /// <summary>
        /// 数据库连接串
        /// </summary>
        protected string m_ConnectString;
        /// <summary>
        /// 对象工厂
        /// </summary>
        protected IDataBaseAdapter m_DBaseAdapter;
        /// <summary>
        /// 获取适配器
        /// </summary>
        public IDataBaseAdapter DbAdapter
        {
            get { return m_DBaseAdapter; }
        }
        /// <summary>
        /// 获取上下文
        /// </summary>
        public IDataBaseContext DbContext
        {
            get { return m_Context; }
        }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="context"></param>
        /// <param name="Adapter"></param>
        public DataBaseHandler(IDataBaseContext context, IDataBaseAdapter Adapter)
        {
            if (Adapter == null)
                throw new ArgumentException("Adapter is null");
            m_Context = context;
            m_ConnectString = m_Context.ConnectionInfo.ConnectionString;
            m_DBaseAdapter = Adapter;
            m__HandlerID = Guid.NewGuid().ToString();
            
            m_Hash = new Hashtable();
        }

        #region ExecuteNonQuery
        //*********************************************************************
        /// <summary>
        /// 执行非查询的Sql语句
        /// </summary>
        /// <param name="Sql">sql语句</param>
        //*********************************************************************
        public int ExecuteNonQuery(string Sql)
        {
            return ExecuteNonQuery(Sql, null);
        }
        //*********************************************************************
        /// <summary>
        /// 执行非查询的参数化SQL语句
        /// </summary>
        /// <param name="Sql">sql语句</param>
        /// <param name="commandParameters">参数</param>
        //*********************************************************************
        public int ExecuteNonQuery(string Sql, params IDataParameter[] commandParameters)
        {
            return ExecuteNonQuery(CommandType.Text, Sql, null, commandParameters);
        }
        /// <summary>
        /// 执行非查询的Sql语句,如果传入的事物对象
        /// 不为null,则使用此连接,如果为null则创建新的连接,使用后关闭.
        /// </summary>
        /// <param name="Tra">事务对象</param>
        /// <param name="Sql">sql语句</param>
        /// <param name="commandParameters">参数</param>
        /// <returns></returns>
        public int ExecuteNonQuery(IDbTransaction Tra,string Sql, params IDataParameter[] commandParameters)
        {
            return ExecuteNonQuery(CommandType.Text, Sql, Tra, commandParameters);
        }
        /// <summary>
        /// 执行非查询的Sql语句,如果传入的事物对象
        /// 不为null,则使用此连接,如果为null则创建新的连接,使用后关闭.
        /// </summary>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">命令内容</param>
        /// <param name="Tra">事物对象</param>
        /// <param name="commandParameters">参数对象数组</param>
        /// <returns>影响行数</returns>
        public int ExecuteNonQuery(CommandType cmdType, string cmdText, IDbTransaction Tra, params IDataParameter[] commandParameters)
        {
            if (Tra == null) Tra = ContextTransaction;//内部事务对象优先级较低
            using (IDbCommand cmd = m_DBaseAdapter.DbFactory.CreateCommand())
            {
                if (Tra == null)
                {
                    IDbConnection dbConnection = m_DBaseAdapter.DbFactory.CreateConnection();
                    dbConnection.ConnectionString = m_ConnectString;
                    dbConnection.Open();
                    DataBaseEntry.PrepareCommand(dbConnection, cmd, cmdType, cmdText, commandParameters);
                }
                else
                {
                    DataBaseEntry.PrepareCommand(Tra.Connection, cmd, cmdType, cmdText, commandParameters);
                    cmd.Transaction = Tra is DataBaseTransaction?((DataBaseTransaction)Tra).Tra:Tra;
                }
                return ExecuteNonQuery(cmd);
            }
        }
        //*********************************************************************
        /// <summary>执行非查询的Sql语句,如果传入的TraConnection,和SqlTra
        /// 不为null,则使用此连接,如果为null则创建新的连接,使用后关闭.
        /// </summary>
        /// <param name="sqlCmd">SqlCommand</param>
        //*********************************************************************
        public int ExecuteNonQuery(IDbCommand sqlCmd)
        {
            if (sqlCmd == null)
                throw new ArgumentNullException("sqlCmd");
            return DataBaseEntry.IExecuteNonQuery(sqlCmd);
        }
        #endregion

        #region 查询,填充DataSet

        //********************************************************************
        /// <summary>功能：执行SQL查询 
        /// </summary>
        /// <param name="Sql">SQl语句</param>,
        //********************************************************************
        public DataSet ExecuteQuery(string Sql)
        {
            return ExecuteQuery(Sql, null);
        }
        /// <summary>
        /// 功能：执行参数化SQL查询
        /// </summary>
        /// <param name="Sql">SQl语句</param>
        /// <param name="commandParameters">参数</param>
        /// <returns></returns>
        public DataSet ExecuteQuery(string Sql, params IDataParameter[] commandParameters)
        {
            return ExecuteQuery(CommandType.Text, Sql,null, commandParameters);
        }
        /// <summary>
        /// 功能：执行带事物的SQL查询
        /// </summary>
        /// <param name="Tra">事物对象</param>
        /// <param name="Sql">sql语句</param>
        /// <param name="commandParameters">参数列表</param>
        /// <returns></returns>
        public DataSet ExecuteQuery(IDbTransaction Tra, string Sql, params IDataParameter[] commandParameters)
        {
             return ExecuteQuery(CommandType.Text, Sql, Tra, commandParameters);
        }

        //*********************************************************************
        /// <summary>
        /// 执行SQL查询，返回成功添加或刷新的行数
        /// </summary>
        /// <param name="cmdType">命令行类型</param>
        ///<param name="cmdText">命令内容</param>
        /// <param name="Tra">事物对象</param>
        /// <param name="commandParameters">参数对象数组</param>
        /// <returns>返回成功获取的行数</returns>
        //*********************************************************************
        public DataSet ExecuteQuery(CommandType cmdType, string cmdText, IDbTransaction Tra, params IDataParameter[] commandParameters)
        {
            if (String.IsNullOrEmpty(cmdText))
                throw new ArgumentNullException("cmdText");

            if (Tra == null) Tra = ContextTransaction;//内部事务对象优先级较低
            using (IDbCommand cmd = m_DBaseAdapter.DbFactory.CreateCommand())
            {
                if (Tra == null)
                {
                    IDbConnection dbConnection = m_DBaseAdapter.DbFactory.CreateConnection();
                    dbConnection.ConnectionString = m_ConnectString;
                    //dbConnection.Open();//这里不要打开，避免连接池被用尽
                    DataBaseEntry.PrepareCommand(dbConnection, cmd, cmdType, cmdText, commandParameters);
                //    SetReadIsolationLevel(dbConnection);
                }
                else
                {
                    DataBaseEntry.PrepareCommand(Tra.Connection, cmd, cmdType, cmdText, commandParameters);
                    cmd.Transaction = Tra is DataBaseTransaction ? ((DataBaseTransaction)Tra).Tra : Tra;
                }
                return ExecuteQuery(cmd);
            }
        }
        //********************************************************************
        /// <summary>
        /// 执行带事务的SQl查询
        /// </summary>
        /// <param name="Cmd">带事务的SqlCommand,不能为空</param>
        /// <returns>成功添加或刷新的行数</returns>
        //********************************************************************
        public DataSet ExecuteQuery(IDbCommand Cmd)
        {
            if (Cmd == null)
                throw new ArgumentNullException("SqlCmd");
            DataSet ds = null;
            {
                IDbDataAdapter dataAdapter = m_DBaseAdapter.DbFactory.CreateDataAdapter();
                if (Cmd.Transaction == null && m_DBaseAdapter.GetIsolationLevelSql()!=null )
                    Cmd.CommandText = m_DBaseAdapter.GetIsolationLevelSql() + Cmd.CommandText;
                dataAdapter.SelectCommand = Cmd;
                DataBaseEntry.IExecuteDataAdapter(out ds, dataAdapter);
            }
            return ds;
        }
        #endregion

        #region 执行DataReader
        /// <summary>
        /// 执行SQL查询
        /// </summary>
        /// <param name="Sql">SQL语句</param>
        /// <returns>SqlDataReader对象,注意要自己关闭</returns>
        public IDataReader ExecuteReader(string Sql)
        {
            return ExecuteReader(Sql, null);
        }
        /// <summary>
        /// 执行参数化SQL查询
        /// </summary>
        /// <param name="Sql">SQL语句</param>
        /// <param name="commandParameters">参数列表</param>
        /// <returns>SqlDataReader对象,注意要自己关闭</returns>
        public IDataReader ExecuteReader(string Sql, params IDataParameter[] commandParameters)
        {
            return ExecuteReader(CommandType.Text, Sql, null, commandParameters);
        }
        /// <summary>
        /// 执行带事物的SQL查询
        /// </summary>
        /// <param name="Tra">事物对象</param>
        /// <param name="cmdText">SQL语句</param>
        /// <param name="commandParameters">事物对象</param>
        /// <returns>SqlDataReader对象,注意要自己关闭</returns>
        public IDataReader ExecuteReader(IDbTransaction Tra, string cmdText, params IDataParameter[] commandParameters)
        {
            return ExecuteReader(CommandType.Text, cmdText, Tra, commandParameters);
        }
        //*********************************************************************
        /// <summary>执行SQl查询
        /// </summary>
        /// <param name="cmdType">命令类型</param>,
        /// <param name="cmdText">命令内容</param>,
        /// <param name="Tra">数据库事务对象</param>,
        /// <param name="commandParameters">参数对象数组</param>
        /// <returns>SqlDataReader对象,注意要自己关闭</returns>
        //*********************************************************************
        public IDataReader ExecuteReader(CommandType cmdType, string cmdText, IDbTransaction Tra, params IDataParameter[] commandParameters)
        {
            if (String.IsNullOrEmpty(cmdText))
                throw new ArgumentNullException("cmdText");
            if (Tra == null) Tra = ContextTransaction;//内部事务对象优先级较低
            using (IDbCommand cmd = m_DBaseAdapter.DbFactory.CreateCommand())
            {
                if (Tra == null)
                {
                    IDbConnection dbConnection = m_DBaseAdapter.DbFactory.CreateConnection();
                    dbConnection.ConnectionString = m_ConnectString;
                    dbConnection.Open();
                    SetReadIsolationLevel(dbConnection);
                    DataBaseEntry.PrepareCommand(dbConnection, cmd, cmdType, cmdText, commandParameters);
                }
                else
                {
                    DataBaseEntry.PrepareCommand(Tra.Connection, cmd, cmdType, cmdText, commandParameters);
                    cmd.Transaction = Tra is DataBaseTransaction ? ((DataBaseTransaction)Tra).Tra : Tra;
                }
                return DataBaseEntry.IExecuteReader(cmd);
            }
        }
        #endregion

        #region ExecuteScalar
        //*********************************************************************
        /// <summary>功能：执行SQl查询,返回第一行的第一列的值
        /// </summary>
        /// <param name="Sql">SQL语句</param>
        //*********************************************************************
        public object ExecuteScalar(string Sql)
        {
            return ExecuteScalar(Sql, null);
        }
        /// <summary>
        /// 功能：执行SQl查询,返回第一行的第一列的值
        /// </summary>
        /// <param name="Sql">SQL语句</param>
        /// <param name="commandParameters">参数列表</param>
        /// <returns></returns>
        public object ExecuteScalar(string Sql, params IDataParameter[] commandParameters)
        {
            return ExecuteScalar(CommandType.Text, Sql, null, commandParameters);
        }
        /// <summary>
        /// 功能：执行SQl查询,返回第一行的第一列的值
        /// </summary>
        /// <param name="Tra">事物对象</param>
        /// <param name="Sql">SQL语句</param>
        /// <param name="commandParameters">参数列表</param>
        /// <returns></returns>
        public object ExecuteScalar(IDbTransaction Tra, string Sql, params IDataParameter[] commandParameters)
        {
            return ExecuteScalar(CommandType.Text, Sql, Tra, commandParameters);
        }

       /// <summary>
       /// 
       /// </summary>
       /// <param name="dbConnection"></param>
        public virtual void SetReadIsolationLevel(IDbConnection dbConnection)
        {
            String str = this.DbAdapter.GetIsolationLevelSql();
            if (str == null || str.Length == 0)
                return;
            IDbCommand cmd = dbConnection.CreateCommand();
            cmd.CommandText = str;// "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;";
            cmd.ExecuteNonQuery();
            return;
        }
        //*********************************************************************
        /// <summary>
        /// 执行SQl查询,返回第一行的第一列的值
        /// </summary>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">命令内容</param>
        /// <param name="Tra">事物对象</param>
        /// <param name="commandParameters">参数对象数组</param>
        /// <returns>第一行的第一列的值</returns>
        //*********************************************************************
        public object ExecuteScalar(CommandType cmdType, string cmdText, IDbTransaction Tra, params IDataParameter[] commandParameters)
        {
            if (String.IsNullOrEmpty(cmdText))
                throw new ArgumentNullException("cmdText");
            if (Tra == null) Tra = ContextTransaction;//内部事务对象优先级较低
            using (IDbCommand cmd = m_DBaseAdapter.DbFactory.CreateCommand())
            {
                if (Tra == null)
                {
                    IDbConnection dbConnection = m_DBaseAdapter.DbFactory.CreateConnection();
                    dbConnection.ConnectionString = m_ConnectString;
                    dbConnection.Open();
                    SetReadIsolationLevel(dbConnection);
                   // cmdText = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;"+cmdText;
                    DataBaseEntry.PrepareCommand(dbConnection, cmd, cmdType, cmdText, commandParameters);
                }
                else
                {
                    DataBaseEntry.PrepareCommand(Tra.Connection, cmd, cmdType, cmdText, commandParameters);
                    cmd.Transaction = Tra is DataBaseTransaction ? ((DataBaseTransaction)Tra).Tra : Tra;
                }
                return DataBaseEntry.IExecuteScalar(cmd);
            }
        }        
        #endregion

        #region 执行存储过程
        /// <summary>
        /// 执行存储过程，返回第一行的第一列的值
        /// </summary>
        /// <param name="StoreProcedureName">存储过程名称</param>
        /// <param name="commandParameters">参数列表</param>
        /// <returns></returns>
        public object ExecuteProcedureScalar(string StoreProcedureName, params IDataParameter[] commandParameters)
        {
            return ExecuteScalar(CommandType.StoredProcedure, StoreProcedureName, null, commandParameters);
        }
        /// <summary>
        /// 执行存储过程，返回IDataReader
        /// </summary>
        /// <param name="StoreProcedureName">存储过程名称</param>
        /// <param name="commandParameters">参数列表</param>
        /// <returns></returns>
        public IDataReader ExecuteProcedureReader(string StoreProcedureName, params IDataParameter[] commandParameters)
        {
            return ExecuteReader(CommandType.StoredProcedure, StoreProcedureName,null, commandParameters);
        }
        /// <summary>
        /// 执行存储过程,返回DataSet
        /// </summary>
        /// <param name="StoreProcedureName">存储过程名称</param>
        /// <param name="commandParameters">参数列表</param>
        /// <returns></returns>
        public DataSet ExecuteProcedureQuery(string StoreProcedureName, params IDataParameter[] commandParameters)
        {
            return ExecuteQuery(CommandType.StoredProcedure, StoreProcedureName, null, commandParameters);
        }
        /// <summary>
        /// 执行非查询Procedure
        /// </summary>
        /// <param name="StoreProcedureName">存储过程名称</param>
        /// <param name="commandParameters">参数列表</param>
        /// <returns></returns>
        public int ExecuteProcedureNonQuery(string StoreProcedureName, params IDataParameter[] commandParameters)
        {
            return ExecuteNonQuery(CommandType.StoredProcedure, StoreProcedureName, null, commandParameters);
        }
        /// <summary>
        /// 执行非查询Procedure
        /// </summary>
        /// <param name="StoreProcedureName">存储过程名称</param>
        /// <param name="outParameters">输出参数名称与值对</param>
        /// <param name="commandParameters">参数列表</param>
        /// <returns></returns>
        public int ExecuteProcedureNonQuery(string StoreProcedureName,out Hashtable outParameters, params IDataParameter[] commandParameters)
        {
            outParameters = new Hashtable();
            using (IDbCommand cmd = m_DBaseAdapter.DbFactory.CreateCommand())
            {
                IDbConnection dbConnection = m_DBaseAdapter.DbFactory.CreateConnection();
                dbConnection.ConnectionString = m_ConnectString;
                dbConnection.Open();
                DataBaseEntry.PrepareCommand(dbConnection, cmd, CommandType.StoredProcedure, StoreProcedureName, commandParameters);
                int result = ExecuteNonQuery(cmd);
                foreach (IDataParameter para in cmd.Parameters)
                {
                    if (para.Direction == ParameterDirection.Output || para.Direction == ParameterDirection.InputOutput)
                    {
                        outParameters.Add(para.ParameterName, para.Value);
                    }
                }
                return result;
            }
        }
        #endregion


        #region 内部事务处理
        private const string TransactionKey = "Draco.DB.QuickDataBase.Common.DataBaseHandler.ContextTransaction";
        /// <summary>
        /// 上下文事务对象
        /// </summary>
        protected DataBaseTransaction ContextTransaction
        {
            get { return LogicalThreadContext.GetData(TransactionKey) as DataBaseTransaction; }
            set 
            {
                if(value==null)
                    LogicalThreadContext.FreeNamedDataSlot(TransactionKey);
                else
                    LogicalThreadContext.SetData(TransactionKey, value);
            }
        }
        /// <summary>
        /// 开启新的事务，使用本对象调用的所有SQL将在事务内执行
        /// </summary>
        /// <returns></returns>
        public IDbTransaction StartTransaction()
        {
            DataBaseTransaction tra = ContextTransaction;
            if (tra == null)
            {
                tra = new DataBaseTransaction(m_ConnectString, m_DBaseAdapter);
                ContextTransaction = tra;
                tra.StartTransaction(IsolationLevel.ReadCommitted);
            }
            tra.ReferenceCount++;
            return tra;
        }
        /// <summary>
        /// 提交事务
        /// </summary>
        public void CommitTransaction()
        {
            DataBaseTransaction tra = ContextTransaction;
            if (tra != null)
            {
                tra.ReferenceCount--;
                if (tra.ReferenceCount == 0)
                {
                    tra.Commit();
                    tra.Dispose();
                    ContextTransaction = null;
                }
            }
        }
        /// <summary>
        /// 回滚事物
        /// </summary>
        public void RollbackTransaction()
        {
            DataBaseTransaction tra = ContextTransaction;
            if (tra != null)
            {
                tra.ReferenceCount--;
                if (tra.ReferenceCount == 0)
                {
                    tra.Rollback();
                    tra.Dispose();
                    ContextTransaction = null;
                }
            }
        }
        #endregion

        #region 扩展
        /// <summary>
        /// 用于标识当前对象的ID
        /// </summary>
        public string __HandlerID
        {
            get { return m__HandlerID; }
        }
        /// <summary>
        /// 获取属性集合
        /// </summary>
        public Hashtable Attributes
        {
            get { return m_Hash; }
        }
        #endregion
    }
}
