using System.Collections;
using System.Data;
using Draco.DB.ORM.Mapping;
using Draco.DB.ORM.Mapping.AttrMapping;
using Draco.DB.ORM.OracleClient;

using Draco.DB.ORM.SqlServerClient;
using Draco.DB.QuickDataBase.Adapter;
using System;
using Draco.DB.QuickDataBase.Utility;
using Draco.DB.QuickDataBase.Configuration;
using System.IO;
using System.Xml;
using Draco.DB.ORM.Conf;
using Draco.DB.QuickDataBase;
using Draco.DB.ORM.Common;
using Draco.DB.ORM.Schema;

namespace Draco.DB.ORM.Adapter
{
    /// <summary>
    /// ORM适配器工厂
    /// </summary>
    public class ORMAdapterCreator
    {
        private static Hashtable m_FactoryPool = new Hashtable();
        /// <summary>
        /// 获取ORM适配器
        /// </summary>
        /// <param name="dataServerType"></param>
        /// <returns></returns>
        public static IORMAdapter GetORMAdapter(string dataServerType)
        {
            IORMAdapter factory = m_FactoryPool[dataServerType] as IORMAdapter;
            if (factory == null)
            {
                dataServerType = ParseAdapterString(dataServerType);
                factory = ObjectFactory.CreateObject(dataServerType) as IORMAdapter;
                if (factory == null)
                    throw new Exception("不被支持的数据库链接类型!");
                m_FactoryPool[dataServerType] = factory;
            }
            return factory;
        }
        /// <summary>
        /// 解析工厂字符串
        /// </summary>
        /// <param name="AdapterString"></param>
        /// <returns></returns>
        protected static string ParseAdapterString(string AdapterString)
        {
            if (String.IsNullOrEmpty(AdapterString))
                throw new ArgumentNullException("FactoryString");
            ORMDbProviderConfigurationLoader Loader = new ORMDbProviderConfigurationLoader();
            DbProviderFactoriesConfiguration Factories = Loader.Load();
            if (Factories != null)
            {
                DbProviderFactoryConfiguration Factory = Factories[AdapterString];
                String TypeString = Factory.DbAdapter;
                return TypeString;
            }
            return "";
        }

        #region 静态属性
        /// <summary>
        /// 获取映射管理器
        /// </summary>
        public static MappingManager MappingManager
        {
            get { return MappingManager.GetMappingManager(new AttrMappingLoader()); }
        }
        #endregion
    }
}
