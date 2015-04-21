using Draco.DB.QuickDataBase.Configuration;
using Draco.DB.QuickDataBase;
using Draco.DB.QuickDataBase.Adapter;
using Draco.DB.ORM.Adapter;
using Draco.DB.ORM.Common;
using Draco.DB.ORM.Schema.Vendor;

namespace Draco.DB.ORM
{
    /// <summary>
    /// 获取数据操作对象
    /// </summary>
    public class ORMContext : DataBaseContext,IORMContext
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="ConnctionString"></param>
        /// <param name="DbType"></param>
        public ORMContext(string ConnctionString, string DbType)
            : this(new ConnectionInfo(ConnctionString, DbType)){}
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="ConnctionString"></param>
        /// <param name="DbType"></param>
        /// <param name="DbProviderName"></param>
        public ORMContext(string ConnctionString, string DbType,string DbProviderName)
            : this(new ConnectionInfo(ConnctionString, DbType, DbProviderName)) { }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="_ORMConnectionInfo"></param>
        public ORMContext(ConnectionInfo _ORMConnectionInfo)
            : base(_ORMConnectionInfo)
        {
            m_ConnectionInfo = _ORMConnectionInfo;
        }
        /// <summary>
        /// 获取数据实体操作类
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public IEntityHandler<T> CreateHandler<T>(T Entity) where T : AbstractEntity, new()
        {
            return new EntityHandler<T>(Entity, this);
        }
        /// <summary>
        /// 获取数据实体操作类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Entity"></param>
        /// <param name="Handler"></param>
        /// <returns></returns>
        public IEntityHandler<T> CreateHandler<T>(T Entity,IDataBaseHandler Handler) where T : AbstractEntity, new()
        {
            return new EntityHandler<T>(Entity, Handler);
        }
        /// <summary>
        /// 获取构架加载器
        /// </summary>
        /// <returns></returns>
        public ISchemaLoader SchemaLoader
        {
            get
            {
                IORMAdapter adaper = ORMAdapterCreator.GetORMAdapter(m_ConnectionInfo.DataServerType);
                return adaper.GetSchemaLoader(adaper.GetDbHandler(this));
            }
        }
    }
}
