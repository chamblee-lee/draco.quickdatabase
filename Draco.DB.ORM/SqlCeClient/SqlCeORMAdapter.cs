using System;
using System.Collections.Generic;
using System.Text;
using Draco.DB.QuickDataBase.SqlCeClient;
using Draco.DB.ORM.Adapter;
using Draco.DB.ORM.Schema.Vendor;
using Draco.DB.QuickDataBase;
using Draco.DB.ORM.Common.AutoSQL;
using Draco.DB.ORM.Common;

namespace Draco.DB.ORM.SqlCeClient
{
    class SqlCeORMAdapter : SqlCeAdapter, IORMAdapter
    {
        /// <summary>
        /// 创建SQLBuilder对象
        /// </summary>
        /// <param name="Entity"></param>
        /// <param name="Adapter"></param>
        /// <returns></returns>
        public SQLBuilder CreateSQLBuilder(AbstractEntity Entity, IORMAdapter Adapter)
        {
            return new SQLBuilderSqlCe(Entity, Adapter);
        }
        /// <summary>
        /// 创建ISchemaLoader对象
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public ISchemaLoader GetSchemaLoader(IDataBaseHandler handler)
        {
            return new SqlCeSchemaLoader(handler);
        }
    }
}
