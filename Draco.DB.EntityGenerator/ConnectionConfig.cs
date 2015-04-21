using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Xml;
using System.IO;
using Draco.DB.QuickDataBase.Configuration;

namespace CodeGenerator
{
    class ConnectionConfig
    {
        /// <summary>
        /// 从文件加载
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public ConnectionInfo LoadFromFile(string filePath)
        {
            if (!String.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(filePath);
                return LoadFromXML(xDoc);
            }
            return null;
        }
        /// <summary>
        /// 从XML文档加载
        /// </summary>
        /// <param name="xDoc"></param>
        /// <returns></returns>
        public ConnectionInfo LoadFromXML(XmlDocument xDoc)
        {
            if (xDoc == null)
                throw new ArgumentNullException("xDoc is null");
            XmlElement root = xDoc.DocumentElement;
            return Load(root);
        }

        /// <summary>
        /// 从XML文档加载
        /// </summary>
        /// <param name="el"></param>
        /// <returns></returns>
        public ConnectionInfo Load(XmlElement el)
        {
            if (el == null)
                throw new ArgumentNullException("el is null");
            XmlNode xConnection = el.SelectSingleNode("ConnectionString");
            XmlNode xDataServerType = el.SelectSingleNode("DataServerType");
            string Connection = xConnection == null ? "" : xConnection.InnerText;
            string DataServerType = xDataServerType == null ? "" : xDataServerType.InnerText;
            return new ConnectionInfo(Connection, DataServerType);
        }

        /// <summary>
        /// 临时连接信息存入XML文档
        /// </summary>
        /// <param name="el"></param>
        /// <returns></returns>
        public void SaveToXml(string contxt, int typeindex, string filePath)
        {
            if (!String.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(filePath);
                if (xDoc == null)
                    throw new ArgumentNullException("xDoc is null");
                XmlElement root = xDoc.DocumentElement;
                if (root == null)
                    throw new ArgumentNullException("el is null");
                XmlNode xConnection = root.SelectSingleNode("ConnectionString");
                XmlNode xDataServerType = root.SelectSingleNode("DataServerType");
                xConnection.InnerText = contxt;
                xDataServerType.InnerText = typeindex.ToString();
                xDoc.Save(filePath);
            }
        }

        /// <summary>
        /// 生成设置信息存入XML文档
        /// </summary>
        /// <param name="el"></param>
        /// <returns></returns>
        public void SaveToXml2(string context, string namespace1, string spkgenerator, string isdao, string outputfath, string filePath)
        {
            if (!String.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(filePath);
                if (xDoc == null)
                    throw new ArgumentNullException("xDoc is null");
                XmlElement root = xDoc.DocumentElement;
                if (root == null)
                    throw new ArgumentNullException("el is null");

                XmlNode xcontext = root.SelectSingleNode("Context");
                XmlNode xnamespace1 = root.SelectSingleNode("Namespace");
                XmlNode xspkgenerator = root.SelectSingleNode("Spkgenerator");
                XmlNode xisdao = root.SelectSingleNode("Isdao");
                XmlNode xoutputfath = root.SelectSingleNode("Outputpath");

                xcontext.InnerText = context;
                xnamespace1.InnerText = namespace1;
                xspkgenerator.InnerText = spkgenerator;
                xisdao.InnerText = isdao;
                xoutputfath.InnerText = outputfath;

                xDoc.Save(filePath);
            }
        }

        /// <summary>
        /// 皮肤信息存入XML文档
        /// </summary>
        /// <param name="el"></param>
        /// <returns></returns>
        public void SaveToXml3(string theme, string filePath)
        {
            if (!String.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(filePath);
                if (xDoc == null)
                    throw new ArgumentNullException("xDoc is null");
                XmlElement root = xDoc.DocumentElement;
                if (root == null)
                    throw new ArgumentNullException("el is null");

                XmlNode xtheme = root.SelectSingleNode("Theme");
                xtheme.InnerText = theme;

                xDoc.Save(filePath);
            }
        }

        /// <summary>
        /// 从XML加载Context
        /// </summary>
        /// <param name="el"></param>
        /// <returns></returns>
        public string LoadContext(string filePath)
        {
            if (!String.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(filePath);
                if (xDoc == null)
                    throw new ArgumentNullException("xDoc is null");
                XmlElement root = xDoc.DocumentElement;
                if (root == null)
                    throw new ArgumentNullException("el is null");
                XmlNode xContext = root.SelectSingleNode("Context");
                string Context = xContext == null ? "" : xContext.InnerText;
                return Context;
            }
            return "";
        }

        /// <summary>
        /// 从XML加载namespace
        /// </summary>
        /// <param name="el"></param>
        /// <returns></returns>
        public string LoadNamespace(string filePath)
        {
            if (!String.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(filePath);
                if (xDoc == null)
                    throw new ArgumentNullException("xDoc is null");
                XmlElement root = xDoc.DocumentElement;
                if (root == null)
                    throw new ArgumentNullException("el is null");
                XmlNode xNamespace = root.SelectSingleNode("Namespace");
                string Namespace = xNamespace == null ? "" : xNamespace.InnerText;
                return Namespace;
            }
            return "";
        }

        /// <summary>
        /// 从XML加载主键生成器
        /// </summary>
        /// <param name="el"></param>
        /// <returns></returns>
        public string LoadSpkgenerator(string filePath)
        {
            if (!String.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(filePath);
                if (xDoc == null)
                    throw new ArgumentNullException("xDoc is null");
                XmlElement root = xDoc.DocumentElement;
                if (root == null)
                    throw new ArgumentNullException("el is null");
                XmlNode xs = root.SelectSingleNode("Spkgenerator");
                string s = xs == null ? "" : xs.InnerText;
                return s;
            }
            return "";
        }

        /// <summary>
        /// 从XML加载是否生成dao
        /// </summary>
        /// <param name="el"></param>
        /// <returns></returns>
        public string LoadIsdao(string filePath)
        {
            if (!String.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(filePath);
                if (xDoc == null)
                    throw new ArgumentNullException("xDoc is null");
                XmlElement root = xDoc.DocumentElement;
                if (root == null)
                    throw new ArgumentNullException("el is null");
                XmlNode xs = root.SelectSingleNode("Isdao");
                string s = xs == null ? "" : xs.InnerText;
                return s;
            }
            return "";
        }

        /// <summary>
        /// 从XML加载输出目录
        /// </summary>
        /// <param name="el"></param>
        /// <returns></returns>
        public string LoadOutputpath(string filePath)
        {
            if (!String.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(filePath);
                if (xDoc == null)
                    throw new ArgumentNullException("xDoc is null");
                XmlElement root = xDoc.DocumentElement;
                if (root == null)
                    throw new ArgumentNullException("el is null");
                XmlNode xs = root.SelectSingleNode("Outputpath");
                string s = xs == null ? "" : xs.InnerText;
                return s;
            }
            return "";
        }

        /// <summary>
        /// 从XML加载皮肤
        /// </summary>
        /// <param name="el"></param>
        /// <returns></returns>
        public string LoadTheme(string filePath)
        {
            if (!String.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(filePath);
                if (xDoc == null)
                    throw new ArgumentNullException("xDoc is null");
                XmlElement root = xDoc.DocumentElement;
                if (root == null)
                    throw new ArgumentNullException("el is null");
                XmlNode xs = root.SelectSingleNode("Theme");
                string s = xs == null ? "" : xs.InnerText;
                return s;
            }
            return "";
        }
    }
}
