using System;
using System.Collections.Generic;
using System.Text;

namespace Draco.DB.QuickDataBase.Configuration
{
    /// <summary>
    /// 数据库链接配置
    /// </summary>
    public class ConnectionInfo
    {
        private String m_ConnectionString;
        private String m_DataServerType;
        private String m_ProviderName;

        /// <summary>
        /// 获取数据库连接串
        /// </summary>
        public String ConnectionString
        {
            get { return m_ConnectionString; }
        }
        /// <summary>
        /// 获取数据库类型
        /// </summary>
        public String DataServerType
        {
            get { return m_DataServerType; }
        }
        /// <summary>
        /// 获取数据库提供者类型
        /// </summary>
        public String ProviderName
        {
            get { return m_ProviderName; }
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="connString">数据库连接串</param>
        /// <param name="dataServerType">数据库类型</param>
        public ConnectionInfo(String connString, String dataServerType)
            : this(connString, dataServerType,null)
        {
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="connString">数据库连接串</param>
        /// <param name="dataServerType">数据库类型</param>
        /// <param name="providerName">数据库provider名称</param>
        public ConnectionInfo(String connString, String dataServerType, String providerName)
        {
            if (String.IsNullOrEmpty(connString) || String.IsNullOrEmpty(dataServerType))
                throw new ArgumentNullException("Argument is null");
            m_ConnectionString = connString;
            m_DataServerType = dataServerType;
            m_ProviderName = providerName;
        }
    }
}
