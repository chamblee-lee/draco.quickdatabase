using System;
using System.Data;
using System.Collections;
using Draco.DB.QuickDataBase.Adapter;
namespace Draco.DB.QuickDataBase
{
    /// <summary>
    /// 数据库操作接口
    /// </summary>
    public interface IDataBaseHandler
    {
        /// <summary>
        /// 获取DbContext
        /// </summary>
        IDataBaseContext DbContext { get; }
        /// <summary>
        /// 获取适配器
        /// </summary>
        IDataBaseAdapter DbAdapter { get; }
        /// <summary>
        /// 执行非查询SQL，返回影响的行数
        /// </summary>
        /// <param name="Sql"></param>
        /// <returns></returns>
        int ExecuteNonQuery(string Sql);
        /// <summary>
        /// 执行非查询SQL，返回影响的行数
        /// </summary>
        /// <param name="Sql"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        int ExecuteNonQuery(string Sql, params System.Data.IDataParameter[] commandParameters);
        /// <summary>
        /// 执行查询SQL，返回DataSet
        /// </summary>
        /// <param name="Sql"></param>
        /// <returns></returns>
        System.Data.DataSet ExecuteQuery(string Sql);
        /// <summary>
        /// 执行查询SQL，返回DataSet
        /// </summary>
        /// <param name="Sql"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        System.Data.DataSet ExecuteQuery(string Sql, params System.Data.IDataParameter[] commandParameters);
        /// <summary>
        /// 执行查询SQL，返回IDataReader
        /// </summary>
        /// <param name="Sql"></param>
        /// <returns></returns>
        System.Data.IDataReader ExecuteReader(string Sql);
        /// <summary>
        /// 执行查询SQL，返回IDataReader
        /// </summary>
        /// <param name="Sql"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        System.Data.IDataReader ExecuteReader(string Sql, params System.Data.IDataParameter[] commandParameters);
        /// <summary>
        /// 执行SQL，返回第一行第一列的值
        /// </summary>
        /// <param name="Sql"></param>
        /// <returns></returns>
        object ExecuteScalar(string Sql);
        /// <summary>
        /// 执行SQL，返回第一行第一列的值
        /// </summary>
        /// <param name="Sql"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        object ExecuteScalar(string Sql, params System.Data.IDataParameter[] commandParameters);
        /// <summary>
        /// 执行存储过程，返回第一行的第一列的值
        /// </summary>
        /// <param name="StoreProcedureName">存储过程名称</param>
        /// <param name="commandParameters">参数列表</param>
        /// <returns></returns>
        object ExecuteProcedureScalar(string StoreProcedureName, params IDataParameter[] commandParameters);
        /// <summary>
        /// 执行存储过程，返回IDataReader
        /// </summary>
        /// <param name="StoreProcedureName">存储过程名称</param>
        /// <param name="commandParameters">参数列表</param>
        /// <returns></returns>
        IDataReader ExecuteProcedureReader(string StoreProcedureName, params IDataParameter[] commandParameters);
        /// <summary>
        /// 执行存储过程,返回DataSet
        /// </summary>
        /// <param name="StoreProcedureName">存储过程名称</param>
        /// <param name="commandParameters">参数列表</param>
        /// <returns></returns>
        DataSet ExecuteProcedureQuery(string StoreProcedureName, params IDataParameter[] commandParameters);
        /// <summary>
        /// 执行非查询Procedure
        /// </summary>
        /// <param name="StoreProcedureName">存储过程名称</param>
        /// <param name="commandParameters">参数列表</param>
        /// <returns></returns>
        int ExecuteProcedureNonQuery(string StoreProcedureName, params IDataParameter[] commandParameters);
        /// <summary>
        /// 执行非查询Procedure
        /// </summary>
        /// <param name="StoreProcedureName">存储过程名称</param>
        /// <param name="outParameters">输出参数名称与值对</param>
        /// <param name="commandParameters">参数列表</param>
        /// <returns></returns>
        int ExecuteProcedureNonQuery(string StoreProcedureName, out Hashtable outParameters, params IDataParameter[] commandParameters);
        /// <summary>
        /// 开启新的事务，使用本对象调用的所有SQL将在事务内执行
        /// </summary>
        /// <returns></returns>
        IDbTransaction StartTransaction();
        /// <summary>
        /// 提交事务
        /// </summary>
        void CommitTransaction();
        /// <summary>
        /// 回滚事物
        /// </summary>
        void RollbackTransaction();

        /// <summary>
        /// 标识handler对象的ID
        /// </summary>
        string __HandlerID { get; }
        /// <summary>
        /// 获取属性集合
        /// </summary>
        Hashtable Attributes { get; }
    }
}
