using System;
using Draco.DB.QuickDataBase;
namespace Draco.DB.ORM
{
    /// <summary>
    /// IORMContext接口
    /// </summary>
    public interface IORMContext : IDataBaseContext
    {
        /// <summary>
        /// 创建实体操作类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Entity"></param>
        /// <param name="Handler"></param>
        /// <returns></returns>
        IEntityHandler<T> CreateHandler<T>(T Entity, Draco.DB.QuickDataBase.IDataBaseHandler Handler) where T : Draco.DB.ORM.Common.AbstractEntity, new();
        /// <summary>
        /// 创建实体操作类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Entity"></param>
        /// <returns></returns>
        IEntityHandler<T> CreateHandler<T>(T Entity) where T : Draco.DB.ORM.Common.AbstractEntity, new();
        /// <summary>
        /// 获取构架加载器
        /// </summary>
        Draco.DB.ORM.Schema.Vendor.ISchemaLoader SchemaLoader { get; }
    }
}
