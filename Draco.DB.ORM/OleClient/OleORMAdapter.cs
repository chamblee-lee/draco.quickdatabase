using System;
using System.Collections.Generic;
using System.Text;
using Draco.DB.ORM.Schema;
using Draco.DB.QuickDataBase.OleClient;
using Draco.DB.ORM.Adapter;
using Draco.DB.ORM.Common.AutoSQL;
using Draco.DB.ORM.Common;
using Draco.DB.ORM.Schema.Vendor;
using Draco.DB.QuickDataBase;

namespace Draco.DB.ORM.OleClient
{
    /// <summary>
    /// Ole的ORM适配器
    /// </summary>
    public class OleORMAdapter : OleAdapter,IORMAdapter
    {

        /// <summary>
        /// 创建SQLBuilder对象
        /// </summary>
        /// <param name="Entity"></param>
        /// <param name="Adapter"></param>
        /// <returns></returns>
        public SQLBuilder CreateSQLBuilder(AbstractEntity Entity, IORMAdapter Adapter) 
        {
            return new SQLBuilderOle(Entity, Adapter);
        }
        /// <summary>
        /// 创建ISchemaLoader
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public ISchemaLoader GetSchemaLoader(IDataBaseHandler handler)
        {
            return new OleSchemaLoader(handler);
        }
    }
}
