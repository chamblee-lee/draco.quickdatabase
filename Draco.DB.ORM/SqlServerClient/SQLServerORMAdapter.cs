using Draco.DB.QuickDataBase.SqlClient;
using Draco.DB.ORM.Adapter;
using Draco.DB.ORM.Common.AutoSQL;
using Draco.DB.ORM.Common;
using Draco.DB.ORM.Schema.Vendor;
using Draco.DB.QuickDataBase;

namespace Draco.DB.ORM.SqlServerClient
{
    /// <summary>
    /// SQLServer的ORM适配器
    /// </summary>
    public class SQLServerORMAdapter : SqlServerAdapter, IORMAdapter
    {
        /// <summary>
        /// 创建SQLBuilder对象
        /// </summary>
        /// <param name="Entity"></param>
        /// <param name="Adapter"></param>
        /// <returns></returns>
        public SQLBuilder CreateSQLBuilder(AbstractEntity Entity,IORMAdapter Adapter)
        {
            return new SQLBuilderSQLServer(Entity, Adapter);
        }
        /// <summary>
        /// 创建ISchemaLoader对象
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public ISchemaLoader GetSchemaLoader(IDataBaseHandler handler)
        {
            return new SqlSchemaLoader(handler);
        }
    }
}
