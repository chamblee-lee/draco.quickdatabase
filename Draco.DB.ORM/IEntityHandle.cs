using System;
using Draco.DB.ORM.Common;
using Draco.DB.ORM.Common.AutoSQL;
namespace Draco.DB.ORM
{
    /// <summary>
    /// 实体操作接口
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    public interface IEntityHandler<T> where T : AbstractEntity, new()
    {
        /// <summary>
        /// 添加表达式
        /// </summary>
        /// <param name="exp">表达式</param>
        /// <returns></returns>
        EntityHandler<T> AddExpression(Expression exp);
        /// <summary>
        /// 清除表达式
        /// </summary>
        void ClearExpression();
        /// <summary>
        /// 删除记录
        /// </summary>
        /// <returns></returns>
        int Delete();
        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <param name="Where">where条件语句(不包含where关键字)</param>
        /// <param name="paras">参数列表</param>
        /// <returns></returns>
        System.Collections.Generic.List<T> GetArrayList(string Where, params System.Data.IDataParameter[] paras);
        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <returns></returns>
        System.Collections.Generic.List<T> GetArrayList();
        /// <summary>
        /// 获取DataSet
        /// </summary>
        /// <returns>DataSet</returns>
        System.Data.DataSet GetDataSet();
        /// <summary>
        /// 获取DataSet
        /// </summary>
        /// <param name="Where">where条件语句(不包含where关键字)</param>
        /// <param name="paras">参数列表</param>
        /// <returns></returns>
        System.Data.DataSet GetDataSet(string Where, params System.Data.IDataParameter[] paras);
        /// <summary>
        /// 获取一个实体对象
        /// </summary>
        /// <returns>实体对象</returns>
        T GetOne();
        /// <summary>
        /// 获取单值
        /// </summary>
        /// <param name="ReturnField"></param>
        /// <param name="Where">where条件语句(不包含where关键字)</param>
        /// <param name="paras">参数列表</param>
        /// <returns></returns>
        object GetOneField(string ReturnField, string Where, params System.Data.IDataParameter[] paras);
        /// <summary>
        /// 获取单值
        /// </summary>
        /// <param name="ReturnField"></param>
        /// <returns></returns>
        object GetOneField(string ReturnField);
        /// <summary>
        /// 插入记录
        /// </summary>
        /// <returns></returns>
        int Insert();
        /// <summary>
        /// 保存记录(插入或更新)
        /// </summary>
        /// <returns></returns>
        int Save();
        /// <summary>
        /// 设置实体对象
        /// </summary>
        /// <param name="Entity"></param>
        IEntityHandler<T> SetEntity(T Entity);
        /// <summary>
        /// 获取SQL构造器
        /// </summary>
        SQLBuilder SQLBuilder { get; }
        /// <summary>
        /// 更新记录
        /// </summary>
        /// <returns></returns>
        int Update();
    }
}
