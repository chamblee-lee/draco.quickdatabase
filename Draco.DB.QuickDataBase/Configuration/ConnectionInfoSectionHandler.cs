using System;
using System.Configuration;
using System.Xml;

namespace Draco.DB.QuickDataBase.Configuration
{
    /// <summary>
    /// 数据库连接信息配置小节处理类
    /// </summary>
    public class ConnectionInfoSectionHandler : IConfigurationSectionHandler
    {
        /// <summary>
        /// 读取配置节
        /// </summary>
        /// <example>
        /// <![CDATA[
        /// <configSections>
        ///     <sectionGroup name="draco_DB">
        ///         <section name="connection" type="Draco.DB.QuickDataBase.Configuration.ConnectionInfoSectionHandler,Draco.DB.QuickDataBase"/>
        ///     </sectionGroup>
        /// </configSections>
        /// <draco_DB>
        ///     <connection>
        ///         <add key="dataBaseType" value="SQLSERVER">
        ///         <add key="connectionString" value="Data Source=.;Initial Catalog=Nunit;User Id=sasa;Password=sasa;">
        ///         <add key="providerName" value="System.Data.SqlClient">
        ///     </connection>
        /// </draco_DB>
        /// ]]>
        /// </example>
        /// <remarks>
        /// 使用此类在App.config或web.config中配置configSections/sectionGroup/section节点,然后配置自定义小节。
        /// 其中：
        ///     dataBaseType-------数据库类型
        ///     connectionString---数据库连接串
        ///     providerName-------数据库提供程序(可选)
        /// 以上Key值大小写不敏感
        /// 
        /// 使用方法：
        /// Draco.DB.QuickDataBase.Configuration.ConnectionInfo connInfo = (ConnectionInfo)ConfigurationSettings.GetConfig("draco_DB/connection");
        /// </remarks>
        /// <param name="parent"></param>
        /// <param name="configContext"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public object Create(object parent, object configContext, System.Xml.XmlNode section)
        {
            if (section == null)
                throw new ArgumentNullException("section is empty");
            String dataBaseType = null;
            String connectionString = null;
            String providerName = null;
            foreach (XmlNode cNode in section.ChildNodes)
            {
                XmlAttribute attrKey = cNode.Attributes["key"];
                XmlAttribute attrValue = cNode.Attributes["value"];
                String key = attrKey==null?"":attrKey.Value;
                String Value = attrValue==null?"":attrValue.Value;
                if (!String.IsNullOrEmpty(key))
                {
                    if (String.Compare("dataBaseType", key, true) == 0)
                        dataBaseType = Value;
                    else if (String.Compare("connectionString", key, true) == 0)
                        connectionString = Value;
                    else if (String.Compare("providerName", key, true) == 0)
                        providerName = Value;
                }
            }

            if (String.IsNullOrEmpty(dataBaseType))
                throw new ConfigurationErrorsException("dataBaseType cannot be empty");
            if (String.IsNullOrEmpty(connectionString))
                throw new ConfigurationErrorsException("connectionString cannot be empty");
            ConnectionInfo conn = new ConnectionInfo(connectionString, dataBaseType, providerName);
            return conn;
        }
    }
}
