using System;
using System.Collections.Generic;
using System.Text;

namespace Draco.DB.QuickDataBase.Configuration
{
    /// <summary>
    /// DbProviderFactory配置
    /// </summary>
    public class DbProviderFactoryConfiguration
    {
        private String m_Name;
        private String m_DbAdapter;

        /// <summary>
        /// 获取数据库类型名称
        /// </summary>
        public String Name
        {
            get { return m_Name; }
        }
        /// <summary>
        /// 获取数据库类型适配器
        /// </summary>
        public String DbAdapter
        {
            get { return m_DbAdapter; }
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="name">数据库类型名称</param>
        /// <param name="dbAdapter">数据库类型适配器</param>
        public DbProviderFactoryConfiguration(String name, String dbAdapter)
        {
            if (String.IsNullOrEmpty(name) || String.IsNullOrEmpty(dbAdapter))
                throw new ArgumentNullException("Argument is null ");
            m_Name = name;
            m_DbAdapter = dbAdapter;
        }
    }
}
