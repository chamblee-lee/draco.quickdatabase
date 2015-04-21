using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace Draco.DB.QuickDataBase.Configuration
{
    /// <summary>
    /// DbProviderConfiguration加载器
    /// </summary>
    public class DbProviderConfigurationLoader
    {
        /// <summary>
        /// 缺省的配置文件名称
        /// </summary>
        protected const String DefaultConfigurationFileName = "DbAdapter.config";

        /// <summary>
        /// 尝试从文件和从程序集加载配置列表文件
        /// </summary>
        /// <returns></returns>
        public virtual DbProviderFactoriesConfiguration Load()
        {
            String filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DefaultConfigurationFileName);
            if (File.Exists(filePath))
                return Load(filePath);
            Stream stream = this.GetType().Assembly.GetManifestResourceStream("Draco.DB.QuickDataBase.Conf.DbAdapter.config");
            if (stream != null && stream.Length > 0)
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(stream);
                return Load(xDoc);
            }
            return null;
        }
        /// <summary>
        /// 加载配置列表文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public virtual DbProviderFactoriesConfiguration Load(String filePath)
        {
            if (!String.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(filePath);
                return Load(xDoc);
            }
            return null;
        }

        /// <summary>
        /// 加载配置列表文档
        /// </summary>
        /// <param name="xDoc"></param>
        /// <returns></returns>
        public virtual DbProviderFactoriesConfiguration Load(XmlDocument xDoc)
        {
            if (xDoc == null)
                throw new ArgumentNullException("xDoc is null");
            XmlElement xe = xDoc.DocumentElement;
            XmlElement DbProviderFactories = xe.SelectSingleNode("DbProviders") as XmlElement;
            if (DbProviderFactories != null)
                return Load(DbProviderFactories);
            return null;
        }
        /// <summary>
        /// 加载配置列表
        /// </summary>
        /// <param name="conf"></param>
        /// <returns></returns>
        public virtual DbProviderFactoriesConfiguration Load(XmlElement conf)
        {
            if (conf == null)
                throw new ArgumentNullException("conf is Nullable");
            DbProviderFactoriesConfiguration cfgs = new DbProviderFactoriesConfiguration();
            XmlNodeList list = conf.SelectNodes("DbProvider");
            if (list != null && list.Count > 0)
            {
                foreach (XmlNode n in list)
                {
                    DbProviderFactoryConfiguration cfg = LoadConf(n);
                    if (cfg != null)
                        cfgs.Add(cfg);
                }
            }
            return cfgs;
        }

        /// <summary>
        /// 填充一个配置对象
        /// </summary>
        /// <param name="conf"></param>
        /// <returns></returns>
        public virtual DbProviderFactoryConfiguration LoadConf(XmlNode conf)
        {
            if (conf == null)
                throw new ArgumentNullException("conf is Nullable");
            XmlNode xName = conf.SelectSingleNode("Name");
            XmlNode xDbAdapter = conf.SelectSingleNode("DbAdapter");
            String Name = xName == null ? "" : xName.InnerText;
            String DbAdapter = xDbAdapter == null ? "" : xDbAdapter.InnerText;
            if (!String.IsNullOrEmpty(Name) && !String.IsNullOrEmpty(DbAdapter))
            {
                DbProviderFactoryConfiguration cfg = new DbProviderFactoryConfiguration(Name, DbAdapter);
                return cfg;
            }
            return null;
        }
    }
}
