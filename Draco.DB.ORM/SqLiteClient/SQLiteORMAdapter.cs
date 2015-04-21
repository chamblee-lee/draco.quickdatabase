using System;
using System.Collections.Generic;
using System.Text;
using Draco.DB.QuickDataBase.SqLiteClient;
using Draco.DB.ORM.Adapter;
using Draco.DB.ORM.Common.AutoSQL;
using Draco.DB.ORM.Common;
using Draco.DB.ORM.Schema.Vendor;
using Draco.DB.QuickDataBase;

namespace Draco.DB.ORM.SqLiteClient
{
    /// <summary>
    /// SQLite的ORM适配器
    /// </summary>
    public class SQLiteORMAdapter : SQLiteAdapter,IORMAdapter
    {

        /// <summary>
        /// 创建SQLBuilder对象
        /// </summary>
        /// <param name="Entity"></param>
        /// <param name="Adapter"></param>
        /// <returns></returns>
        public SQLBuilder CreateSQLBuilder(AbstractEntity Entity, IORMAdapter Adapter)
        {
            return new SQLBuilderSQLite(Entity, Adapter);
        }
        /// <summary>
        /// 创建ISchemaLoader对象
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public ISchemaLoader GetSchemaLoader(IDataBaseHandler handler)
        {
            return new SQLiteSchemaLoader(handler);
        }
    }
}
