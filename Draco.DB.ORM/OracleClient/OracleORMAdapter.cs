using System;
using System.Collections.Generic;
using System.Text;
using Draco.DB.QuickDataBase.OracleClient;
using Draco.DB.ORM.Adapter;
using Draco.DB.ORM.Common.AutoSQL;
using Draco.DB.ORM.Common;
using Draco.DB.ORM.Schema.Vendor;
using Draco.DB.QuickDataBase;

namespace Draco.DB.ORM.OracleClient
{
    /// <summary>
    /// Oracle的ORM适配器
    /// </summary>
    public class OracleORMAdapter : OracleAdapter,IORMAdapter
    {

        /// <summary>
        /// 创建SQLBuilder
        /// </summary>
        /// <param name="Entity"></param>
        /// <param name="Adapter"></param>
        /// <returns></returns>
        public SQLBuilder CreateSQLBuilder(AbstractEntity Entity, IORMAdapter Adapter) 
        {
            return new SQLBuilderOracle(Entity, Adapter);
        }
        /// <summary>
        /// 创建ISchemaLoader
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public ISchemaLoader GetSchemaLoader(IDataBaseHandler handler)
        {
            return new OracleSchemaLoader(handler);
        }
    }
}
