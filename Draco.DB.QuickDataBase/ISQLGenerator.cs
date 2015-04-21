using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Draco.DB.QuickDataBase
{
    /// <summary>
    /// 简单SQL生成器
    /// </summary>
    public interface ISQLGenerator
    {
        /// <summary>
        /// 创建查询SQL
        /// </summary>
        /// <param name="paramz">输出的参数数组</param>
        /// <param name="Fields">查询的字段数组</param>
        /// <param name="TableName">查询的表名</param>
        /// <param name="Condition">查询的限制条件</param>
        /// <returns>SQL</returns>
        String CreateSelectSQL(out IDataParameter[] paramz,List<String> Fields,String TableName,Dictionary<String,Object> Condition);
        /// <summary>
        /// 创建插入SQL
        /// </summary>
        /// <param name="paramz">输出的参数数组</param>
        /// <param name="TableName">插入的表名</param>
        /// <param name="FieldValues">插入的键值对</param>
        /// <returns>SQL</returns>
        String CreateInsertSQL(out IDataParameter[] paramz, String TableName, Dictionary<String, Object> FieldValues);
        /// <summary>
        /// 创建更新SQL
        /// </summary>
        /// <param name="paramz">输出的参数数组</param>
        /// <param name="TableName">更新的表名</param>
        /// <param name="FieldValues">更新的键值对</param>
        /// <param name="Condition">更新的限制条件</param>
        /// <returns>SQL</returns>
        String CreateUpdateSQL(out IDataParameter[] paramz, String TableName, Dictionary<String, Object> FieldValues, Dictionary<String, Object> Condition);
        /// <summary>
        /// 创建删除SQL
        /// </summary>
        /// <param name="paramz">输出的参数数组</param>
        /// <param name="TableName">删除的表名</param>
        /// <param name="Condition">删除的限制条件</param>
        /// <returns>SQL</returns>
        String CreateDeleteSQL(out IDataParameter[] paramz, String TableName, Dictionary<String, Object> Condition);
        /// <summary>
        /// 创建分页SQL
        /// </summary>
        /// <param name="SQL">SQL语句</param>
        /// <param name="PageIndex">分页索引(从0开始)</param>
        /// <param name="PageSize">分页大小</param>
        /// <param name="KeyField">关键字段</param>
        /// <param name="OrderField">排序字段</param>
        /// <param name="OrderByDesc">是否为降序</param>
        /// <param name="paras">参数列表</param>
        /// <returns></returns>
        ParameterizedSQL CreatePagedSQL(string SQL, int PageIndex, int PageSize, string KeyField, string OrderField, bool OrderByDesc, params System.Data.IDataParameter[] paras);
        /// <summary>
        /// 获取数据库时间的SQL
        /// </summary>
        /// <returns></returns>
        String GetDataBaseTimeSQL();
        /// <summary>
        /// 把时间转换为SQL中可识别的时间字符串
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        string ConvertDateTimeToSQL(DateTime dt);
        /// <summary>
        /// 创建时间查询的SQL片段
        /// </summary>
        /// <param name="FieldName">查询字段名称</param>
        /// <param name="Op">查询操作符</param>
        /// <param name="dt">日期时间</param>
        /// <param name="IgnoreTime">是否忽略时间</param>
        /// <returns></returns>
        string CreateDateTimeSQLSegment(string FieldName, QueryOperatorEnum Op, DateTime dt, bool IgnoreTime);
    }
}
