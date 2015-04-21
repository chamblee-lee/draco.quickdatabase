using System;
namespace Draco.DB.QuickDataBase
{
    /// <summary>
    /// 数据库参数操作集合
    /// </summary>
    public interface IDataParameters
    {
        /// <summary>
        /// 适配参数名称
        /// </summary>
        /// <param name="paraName"></param>
        /// <returns></returns>
        string AdaptParameterName(string paraName);
        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="paraName"></param>
        /// <param name="val"></param>
        void AddParameterValue(string paraName, object val);
        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="paraName"></param>
        /// <param name="val"></param>
        /// <param name="dataType"></param>
        void AddParameterValue(string paraName, object val, System.Data.DbType dataType);
        /// <summary>
        /// 清除参数
        /// </summary>
        void Clear();
        /// <summary>
        /// 获取参数数组
        /// </summary>
        System.Data.IDataParameter[] Parameters { get; }
    }
}
