using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using Draco.DB.QuickDataBase.Utility;
using System.Collections;
using Draco.DB.QuickDataBase.Adapter;
using Draco.DB.QuickDataBase.Configuration;
using Draco.DB.QuickDataBase.Common;

namespace Draco.DB.QuickDataBase.Adapter
{
    /// <summary>
    /// 数据库类型适配器
    /// </summary>
    public class DataBaseAdapterCreator
    {
        private static Hashtable m_FactoryPool = new Hashtable();

        /// <summary>
        /// 适配数据库类型工厂
        /// </summary>
        /// <param name="connectionInfo"></param>
        /// <returns></returns>
        public IDataBaseAdapter GetDataBaseAdapter(ConnectionInfo connectionInfo)
        {
            string ShortDBType = connectionInfo.DataServerType;
            IDataBaseAdapter IAdaper = m_FactoryPool[ShortDBType] as IDataBaseAdapter;
            if (IAdaper == null)
            {
                String DBType = ParseAdapterString(ShortDBType);
                IAdaper = ObjectFactory.CreateObject(DBType) as IDataBaseAdapter;
                if (IAdaper == null)
                    throw new Exception("不被支持的数据库链接类型!");
                DataBaseAdapter AbsAdapter = IAdaper as DataBaseAdapter;
                if (AbsAdapter != null)
                {
                    AbsAdapter.DbTypeName = ShortDBType;
                    AbsAdapter.DbProviderName = connectionInfo.ProviderName;
                }
                m_FactoryPool[ShortDBType] = IAdaper;
            }
            return IAdaper;
        }
        /// <summary>
        /// 解析工厂字符串
        /// </summary>
        /// <param name="AdapterString"></param>
        /// <returns></returns>
        protected virtual string ParseAdapterString(string AdapterString)
        {
            if (String.IsNullOrEmpty(AdapterString))
                throw new ArgumentNullException("FactoryString");
            DbProviderConfigurationLoader Loader = new DbProviderConfigurationLoader();
            DbProviderFactoriesConfiguration Factories = Loader.Load();
            if (Factories != null)
            {
                DbProviderFactoryConfiguration Factory = Factories[AdapterString];
                String TypeString = Factory.DbAdapter;
                return TypeString;
            }
            return "";
        }
    }
}
