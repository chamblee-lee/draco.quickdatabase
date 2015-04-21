using System;
using System.Collections.Generic;
using System.Text;
using Draco.DB.QuickDataBase.Configuration;
using Draco.DB.QuickDataBase;
using System.IO;
using System.Xml;
using System.Configuration;
using Draco.DB.ORM;

namespace Draco.DB.DbConfiguration
{
    /// <summary>
    /// 数据库配置信息获取类型，现在直接从文件读取，后期可修改为从web注入
    /// </summary>
    public class DbConfigInfo : IDbConfigInfo
    {
        private static IDbConfigInfo m_Instance;
        /// <summary>
        /// 获取实例
        /// </summary>
        public static IDbConfigInfo Instance 
        { 
            get 
            {
                if (m_Instance == null)
                    m_Instance = new DbConfigInfo();
                return m_Instance; 
            } 
        }

/*---------------------实例-----------------------------*/

        private ConnectionInfo m_ConnectionInfo;

        /// <summary>
        /// 获取数据库链接信息
        /// </summary>
        public ConnectionInfo ConnectionInfo
        {
            get 
            {
                if (m_ConnectionInfo == null)
                    m_ConnectionInfo = new ConnectionInfo(ConnectionString, DataServerType, DbProviderName);
                return m_ConnectionInfo;
            }
        }
        /// <summary>
        /// 获取数据库操作上下文
        /// </summary>
        public IDataBaseContext DataBaseContext
        {
            get { return new DataBaseContext(ConnectionInfo); }
        }

        /// <summary>
        /// 获取ORM上下文
        /// </summary>
        public IORMContext ORMContext
        {
            get { return new ORMContext(ConnectionInfo); }
        }

        /// <summary>
        /// 注入数据库链接信息
        /// </summary>
        /// <param name="connInfo"></param>
        public void InjectConnectionInfo(ConnectionInfo connInfo)
        {
            m_ConnectionInfo = connInfo;      
        }

        /// <summary>
        /// 尝试获取数据库时间
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public DateTime GetDateTimeFromDB(IDataBaseHandler handler)
        {
            string SQL = handler.DbAdapter.GetSQLGenerator().GetDataBaseTimeSQL();
            if (!String.IsNullOrEmpty(SQL))
            {
                Object oDt = handler.ExecuteScalar(SQL);
                if (oDt != null && oDt != DBNull.Value)
                {
                    return DateTime.Parse(Convert.ToString(oDt));
                }
            }
            return DateTime.Now;
        }

        #region 获取数据库配置
        /// <summary>
        /// 获取数据库连接串
        /// </summary>
        private String ConnectionString
        {
            get { return GetConfigValue("ConnectionString"); }
        }
        /// <summary>
        /// 获取数据库类型
        /// </summary>
        private String DataServerType
        {
            get { return GetConfigValue("DataServerType"); }
        }
        /// <summary>
        /// 获取数据库Provider名称
        /// </summary>
        private String DbProviderName
        {
            get { return GetConfigValue("ProviderName"); }
        }
        #endregion

        #region 读取文件
        /// <summary>
        /// 获取配置值
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        private String GetConfigValue(String keyName)
        {
            String Value = ConfigurationManager.AppSettings[keyName];
            if (String.IsNullOrEmpty(Value))
                Value = GetConfigValueFromFile(keyName);
            return Value;
        }
        /// <summary>
        /// 尝试从配置文件中读取所需的配置值
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        private string GetConfigValueFromFile(string keyName)
        {
            //系统配置文件
            string configfile = System.AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            if (!File.Exists(configfile))
            {
                configfile = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory+ "web.config");
            }
            if (File.Exists(configfile))
            {
                StringReader srd = new StringReader(File.ReadAllText(configfile));
                XmlTextReader xrd = new XmlTextReader(srd);
                xrd.Namespaces = false; //去掉命名空间，否则下面就查不到节点

                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(xrd);
                XmlElement root = xDoc.DocumentElement;
                string xPath = "appSettings/add[translate(@key,'ABCDEFGHIJKLMNOPQRSTUVWXYZ ','abcdefghijklmnopqrstuvwxyz ')='" + keyName.ToLower() + "']";
                XmlNode Node = root.SelectSingleNode(xPath);
                if (Node != null && Node.Attributes["value"] != null)
                    return Node.Attributes["value"].Value;
            }
            return null;
        }
        #endregion
    }
}
