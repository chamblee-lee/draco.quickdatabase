using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Draco.DB.QuickDataBase.Configuration
{
    /// <summary>
    /// DbProviderFactories配置
    /// </summary>
    public class DbProviderFactoriesConfiguration
    {
        private Hashtable m_Factories = new Hashtable(StringComparer.CurrentCultureIgnoreCase);

        /// <summary>
        /// 添加配置对象
        /// </summary>
        /// <param name="cfg"></param>
        public void Add(DbProviderFactoryConfiguration cfg)
        {
            if (cfg == null)
                throw new ArgumentNullException(" cfg is null ");
            m_Factories[cfg.Name] = cfg;
        }
        /// <summary>
        /// 移除配置对象
        /// </summary>
        /// <param name="name"></param>
        public void Remove(String name)
        {
            m_Factories.Remove(name);
        }
        /// <summary>
        /// 移除配置对象
        /// </summary>
        /// <param name="cfg"></param>
        public void Remove(DbProviderFactoryConfiguration cfg)
        {
            if (cfg != null)
                Remove(cfg.Name);
        }
        /// <summary>
        /// 获取命名配置对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public DbProviderFactoryConfiguration this[String name]
        {
            get 
            {
                if (!String.IsNullOrEmpty(name))
                    return m_Factories[name] as DbProviderFactoryConfiguration;
                return null;
            }
        }

        /// <summary>
        /// 获取所有配置对象
        /// </summary>
        /// <returns></returns>
        public List<DbProviderFactoryConfiguration> GetAllDbProviderFactoryConfiguration()
        {
            List<DbProviderFactoryConfiguration> list = new List<DbProviderFactoryConfiguration>();
            foreach (String key in m_Factories.Keys)
            {
                list.Add(m_Factories[key] as DbProviderFactoryConfiguration);
            }
            return list;
        }
    }
}
