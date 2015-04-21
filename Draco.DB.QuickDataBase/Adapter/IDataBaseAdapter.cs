using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;
using Draco.DB.QuickDataBase.Configuration;
using Draco.DB.QuickDataBase.Utility;

namespace Draco.DB.QuickDataBase.Adapter
{
    /// <summary>
    /// 数据库类型工厂
    /// </summary>
    public interface IDataBaseAdapter
    {
        /// <summary>
        /// 获取数据库类型名称
        /// </summary>
        String DbTypeName { get; }
        /// <summary>
        /// 数据库提供器名称
        /// </summary>
        String DbProviderName { get; }
        /// <summary>
        /// 获取DbFactory
        /// </summary>
        DbProviderFactory DbFactory { get; }
        /// <summary>
        /// 获取DbHandler
        /// </summary>
        /// <param name="Context"></param>
        /// <returns></returns>
        IDataBaseHandler GetDbHandler(IDataBaseContext Context);
        /// <summary>
        /// 适配参数名称
        /// </summary>
        /// <param name="paraName"></param>
        /// <returns></returns>
        string AdaptParameterName(string paraName);
        /// <summary>
        /// 适配参数名称
        /// </summary>
        /// <param name="paraName">参数名</param>
        /// <param name="reservedNames">保留的参数名(防止参数同名)</param>
        /// <returns></returns>
        string AdaptParameterName(string paraName, String[] reservedNames);
        /// <summary>
        /// 创建DataParameter对象
        /// </summary>
        /// <param name="paraName"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        IDataParameter CreateParameter(string paraName, object val);
        /// <summary>
        /// 适配列名称
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <returns></returns>
        string AdaptColumnName(string ColumnName);
        /// <summary>
        /// 适配SQL中的匿名参数
        /// </summary>
        /// <param name="SQL">包含匿名参数的SQL语句</param>
        /// <param name="param">参数对象列表</param>
        /// <returns></returns>
        ParameterizedSQL AdaptSQLAnonymousParams(String SQL, params object[] param);
        /// <summary>
        /// 获取数据库构架信息操作接口类型
        /// </summary>
        /// <param name="Handler">数据库操作Handler</param>
        /// <returns></returns>
        IDataBaseSchemaHandler GetSchemaHandler(IDataBaseHandler Handler);
        /// <summary>
        /// 获取SQL生成器
        /// </summary>
        /// <returns></returns>
        ISQLGenerator GetSQLGenerator();
        
        /// <summary>
        ///获取无事务状态下的隔离级别
        /// </summary>
        /// <returns>SQL语句</returns>
        String GetIsolationLevelSql();

        /// <summary>
        /// 获取
        /// </summary>
        INamedSQLConfig NamedSQLs { get; }
    }
}
