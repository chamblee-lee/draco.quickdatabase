using System;
using System.Collections.Generic;
using System.Text;
using Draco.DB.QuickDataBase.Adapter;
using Draco.DB.ORM.Common.AutoSQL;
using Draco.DB.ORM.Common;
using Draco.DB.ORM.Schema.Vendor;
using Draco.DB.QuickDataBase;

namespace Draco.DB.ORM.Adapter
{
    /// <summary>
    /// 适配器接口
    /// </summary>
    public interface IORMAdapter:IDataBaseAdapter
    {
        /// <summary>
        /// 创建SQLBuilder对象
        /// </summary>
        /// <param name="Entity"></param>
        /// <param name="Adapter"></param>
        /// <returns></returns>
        SQLBuilder CreateSQLBuilder(AbstractEntity Entity, IORMAdapter Adapter);
        /// <summary>
        /// 获取构架加载器
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        ISchemaLoader GetSchemaLoader(IDataBaseHandler handler);
    }
}
