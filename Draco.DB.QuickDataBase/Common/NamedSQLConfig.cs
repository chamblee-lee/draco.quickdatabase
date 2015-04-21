using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Xml;
using System.Reflection;
using System.IO;

namespace Draco.DB.QuickDataBase.Common
{
    /// <summary>
    /// 命名SQL配置
    /// </summary>
    public class NamedSQLConfig : INamedSQLConfig
    {
        private Hashtable m_NamedSQLTable = new Hashtable(StringComparer.CurrentCultureIgnoreCase);

        /// <summary>
        /// 添加命名SQL
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="SQL">SQL</param>
        public void AddNamedSQL(String name,String SQL) 
        {
            if (!String.IsNullOrEmpty(name))
                m_NamedSQLTable[name] = SQL;
        }
        /// <summary>
        /// 添加命名SQL配置
        /// </summary>
        /// <param name="nconf"></param>
        public void AddNamedSQL(INamedSQLConfig nconf)
        {
            if (nconf == null || Object.ReferenceEquals(this, nconf))
                return;
            Dictionary<String, String> Dict = nconf.GetNamedSQL();
            if (Dict != null && Dict.Count > 0)
            {
                foreach (String key in Dict.Keys)
                    m_NamedSQLTable[key] = Dict[key];
            }
        }
        /// <summary>
        /// 添加命名SQL配置
        /// </summary>
        /// <param name="thiz"></param>
        /// <param name="nconf"></param>
        /// <returns></returns>
        public static INamedSQLConfig operator +(NamedSQLConfig thiz, INamedSQLConfig nconf)
        {
            thiz.AddNamedSQL(nconf);
            return thiz;
        }
        /// <summary>
        /// 移除命名SQL
        /// </summary>
        /// <param name="name">名称</param>
        public void RemoveNamedSQL(String name)
        {
            if (m_NamedSQLTable.ContainsKey(name))
                m_NamedSQLTable.Remove(name);
        }
        /// <summary>
        /// 获取命名SQL
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public String this[String name]
        {
            get { return m_NamedSQLTable[name] as String; }
        }
        /// <summary>
        /// 获取命名SQL数目
        /// </summary>
        public int Count
        {
            get { return m_NamedSQLTable.Count; }
        }
        /// <summary>
        /// 获取所有的命名SQL
        /// </summary>
        /// <returns></returns>
        public Dictionary<String, String> GetNamedSQL()
        {
            Dictionary<String, String> dict = new Dictionary<string, string>();
            foreach (String key in m_NamedSQLTable.Keys)
                dict.Add(key, m_NamedSQLTable[key] as String);
            return dict;
        }

        /// <summary>
        /// 从XML文档加载
        /// </summary>
        /// <param name="xDoc"></param>
        /// <returns></returns>
        public INamedSQLConfig Load(System.Xml.XmlDocument xDoc)
        {
            if (xDoc == null)
                throw new ArgumentNullException("xDoc is null ");
            XmlElement root = xDoc.DocumentElement;
            XmlNodeList list = root.SelectNodes("SQL");
            if (list != null && list.Count > 0)
            {
                foreach (XmlNode n in list)
                {
                    XmlAttribute xAttr = n.Attributes["name"];
                    String SQLName = xAttr == null ? "" : xAttr.Value;
                    if (!String.IsNullOrEmpty(SQLName))
                        this.AddNamedSQL(SQLName, n.InnerText);
                }
            }
            return this;
        }

        #region 零配置化加载
        /// <summary>
        /// 非配置化加载
        /// </summary>
        internal void NoConfigurationLoad(String DbType)
        {
            Assembly[] Asses = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly Ass in Asses)
            {
                Object[] Attrs = Ass.GetCustomAttributes(typeof(NamedSQLAttribute), true);
                if (Attrs != null && Attrs.Length > 0)
                    LoadConfigFromAssembly(Ass, DbType);
            }
        }
        private void LoadConfigFromAssembly(Assembly assembly, String DbType)
        {
            String[] files = assembly.GetManifestResourceNames();
            if (files != null && files.Length > 0)
            {
                String match = "." + DbType.ToLower() + ".xml";
                foreach (String fileName in files)
                {
                    if (fileName.Trim().ToLower().EndsWith(match))
                    {
                        Stream stream = assembly.GetManifestResourceStream(fileName);
                        if (stream != null)
                        {
                            XmlDocument xDoc = new XmlDocument();
                            xDoc.Load(stream);
                            Load(xDoc);
                        }
                    }
                }
            }
        }
        #endregion
    }
}
