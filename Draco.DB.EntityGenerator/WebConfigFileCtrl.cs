using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Collections;
using Draco.DB.QuickDataBase.Utility;

namespace CodeGenerator
{
    class WebConfigFileCtrl
    {
        /// <summary>
        /// Xml Document
        /// </summary>
        protected System.Xml.XmlDocument m_xml;
        /// <summary>
        /// Xml File path
        /// </summary>
        protected string m_FilePath;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="FilePath">xml文件绝对路径</param>
        public WebConfigFileCtrl(string FilePath)
        {
            if (!File.Exists(FilePath))
            {
                throw new ArgumentException(FilePath + " 不存在。");
            }
            m_xml = new XmlDocument();
            m_xml.Load(FilePath);
            this.m_FilePath = FilePath;
        }

        /// <summary>
        /// 取得appsettings的配置项keyName的值
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public string GetAttibuteFormXml(string keyName)
        {
            XmlNodeList nodelist = m_xml.DocumentElement.ChildNodes;
            string[] nodeNames = new string[] { "appsettings", "add" };
            return GetAttibuteFormXml(nodelist, nodeNames, "key", "value", keyName);
        }

        /// <summary>
        /// 设置appsettings的配置项值
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="keyValue"></param>
        public void SetAttibuteToXml(string keyName, string keyValue)
        {

            XmlNodeList nodelist = m_xml.DocumentElement.ChildNodes;
            string[] nodeNames = new string[] { "appsettings", "add" };
            SetAttibuteToXml(nodelist, nodeNames, "key", "value", keyName, keyValue);
        }


        /// <summary>
        /// 更改连接字符串中的信息
        /// </summary>
        /// <param name="Connstr"></param>
        /// <param name="Svrname"></param>
        /// <param name="DbName"></param>
        /// <param name="IsTrust"></param>
        /// <param name="Uid"></param>
        /// <param name="psw"></param>
        /// <param name="DbType"></param>
        /// <returns></returns>
        public static string ChangeDbNameUIdPsw(string Connstr, string Svrname, string DbName, bool IsTrust,
            string Uid, string psw, string DbType)
        {
            Hashtable hsb = ConnectionStringAnalyzer.GetConnctionStringAtt(Connstr);
            //数据库地址
            if (hsb.ContainsKey("DATA SOURCE"))
                hsb["DATA SOURCE"] = Svrname;
            else if (hsb.ContainsKey("SERVER"))
                hsb["SERVER"] = Svrname;
            else if (hsb.ContainsKey("ADDRESS "))
                hsb["ADDRESS "] = Svrname;
            else if (hsb.ContainsKey("ADDR"))
                hsb["ADDR"] = Svrname;
            else if (hsb.ContainsKey("NETWORK ADDRESS"))
                hsb["NETWORK ADDRESS"] = Svrname;
            else
                hsb.Add("DATA SOURCE", Svrname);
            if (DbType.ToUpper().Equals("SQLSERVER"))
            {
                //数据库名称
                if (hsb.ContainsKey("INITIAL CATALOG"))
                    hsb["INITIAL CATALOG"] = DbName;
                else if (hsb.ContainsKey("DATABASE"))
                    hsb["DATABASE"] = DbName;
                else
                    hsb.Add("INITIAL CATALOG", DbName);
                string StrProvider = "PROVIDER";
                if (hsb.ContainsKey(StrProvider))
                {
                    hsb[StrProvider] = "SQLOLEDB";
                }
            }
            else if (DbType.ToUpper().Equals("ORACLE"))
            {
                if (hsb.ContainsKey("INITIAL CATALOG"))
                    hsb.Remove("INITIAL CATALOG");
                string StrProvider = "PROVIDER";
                if (hsb.ContainsKey(StrProvider))
                {
                    hsb[StrProvider] = "OraOLEDB.Oracle.1";
                }
            }
            //用户名
            if (hsb.ContainsKey("USER ID"))
                hsb["USER ID"] = Uid;
            else
                hsb.Add("USER ID", Uid);

            //密码
            if (hsb.ContainsKey("PASSWORD"))
                hsb["PASSWORD"] = psw;
            else
                hsb.Add("PASSWORD", psw);

            Connstr = null;
            if (IsTrust)//信任链接,不需要用户名,密码
            {
                hsb.Remove("USER ID");
                hsb.Remove("PASSWORD");
                if (!hsb.ContainsKey("INTEGRATED SECURITY"))
                    hsb.Add("INTEGRATED SECURITY", "SSPI");
                if (!hsb.ContainsKey("PERSIST SECURITY INFO"))
                    hsb.Add("PERSIST SECURITY INFO", "False");
            }
            else//非信任链接
            {
                if (hsb.ContainsKey("INTEGRATED SECURITY"))
                    hsb.Remove("INTEGRATED SECURITY");
                if (hsb.ContainsKey("PERSIST SECURITY INFO"))
                    hsb.Remove("PERSIST SECURITY INFO");
            }
            foreach (string key in hsb.Keys)
            {
                Connstr += key + "=" + ConnStringValueEncoding(hsb[key].ToString()) + ";";
            }
            return Connstr;
        }
       
        /// <summary>
        ///  构建数据库链接串
        /// </summary>
        /// <param name="MyDbType"></param>
        /// <param name="server"></param>
        /// <param name="dbname"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="Encrypt"></param>
        /// <param name="trust"></param>
        /// <returns></returns>
        public string GetConnectString(string MyDbType, string server, string dbname, string user, string password, bool Encrypt, bool trust)
        {
            //------------------------构建连接串----------------------------
            string pwd = password;
            if (Encrypt)
            {
                //pwd =    
            }
            string ConnectString = "";
            if (MyDbType.Trim().ToUpper().Equals("SQLSERVER"))
            {
                ConnectString = "Data Source=" + WebConfigFileCtrl.ConnStringValueEncoding(server) +
                ";Initial Catalog=" + WebConfigFileCtrl.ConnStringValueEncoding(dbname);
                if (!trust)
                {
                    ConnectString += ";User Id=" + WebConfigFileCtrl.ConnStringValueEncoding(user) +
                     ";Password=" + WebConfigFileCtrl.ConnStringValueEncoding(pwd) + ";";
                }
                else
                {//信任连接
                    ConnectString += "; Integrated Security=SSPI;Persist Security Info=False;";
                }
            }
            else if (MyDbType.Trim().ToUpper().Equals("ORACLE"))
            {
                ConnectString = "Data Source=" + WebConfigFileCtrl.ConnStringValueEncoding(server) +
                ";User Id=" + WebConfigFileCtrl.ConnStringValueEncoding(user) +
                ";Password=" + WebConfigFileCtrl.ConnStringValueEncoding(pwd) + ";";
            }
            else if (MyDbType.Trim().ToUpper().Equals("SQLITE"))
            {
                ConnectString = "";
            }
            return ConnectString;
        }

        /// <summary>
        /// 取得连接字符串中的信息
        /// </summary>
        /// <param name="Connstr"></param>
        /// <param name="Svrname"></param>
        /// <param name="DbName"></param>
        /// <param name="Uid"></param>
        /// <param name="psw"></param>
        public static void GetDbNameUIdPsw(string Connstr, out string Svrname, out string DbName,
            out string Uid, out string psw)
        {
            Svrname = DbName = Uid = psw = null;

            Hashtable hsb = ConnectionStringAnalyzer.GetConnctionStringAtt(Connstr);

            //数据库地址
            if (hsb.ContainsKey("DATA SOURCE"))
                Svrname = hsb["DATA SOURCE"].ToString();
            else if (hsb.ContainsKey("SERVER"))
                Svrname = hsb["SERVER"].ToString();
            else if (hsb.ContainsKey("ADDRESS "))
                Svrname = hsb["ADDRESS"].ToString();
            else if (hsb.ContainsKey("ADDR"))
                Svrname = hsb["ADDR"].ToString();
            else if (hsb.ContainsKey("NETWORK ADDRESS"))
                Svrname = hsb["NETWORK ADDRESS"].ToString();

            //数据库名称
            if (hsb.ContainsKey("INITIAL CATALOG"))
                DbName = hsb["INITIAL CATALOG"].ToString();
            else if (hsb.ContainsKey("DATABASE"))
                DbName = hsb["DATABASE"].ToString();

            //用户名
            if (hsb.ContainsKey("USER ID"))
                Uid = hsb["USER ID"].ToString();

            //密码
            if (hsb.ContainsKey("PASSWORD"))
                psw = hsb["PASSWORD"].ToString();

        }

        /// <summary>
        /// 取得配置项的配置值如GetAttibuteFormXml(new string[]{"system.web","trace"},"enabled")
        /// 是取得system.web/trace 节点的属性enabled的属性值
        /// </summary>
        /// <param name="nodeNames">节点递进名称</param>
        /// <param name="AttValname">要返回的属性名称</param>
        /// <returns></returns>
        public string GetAttibuteFormXml(string[] nodeNames, string AttValname)
        {
            if (this.m_xml == null)
                return null;
            XmlNodeList nodelist = this.m_xml.DocumentElement.ChildNodes;
            return GetAttibuteFormXml(nodelist, nodeNames, null, AttValname, null);
        }
        /// <summary>
        /// 取得配置项的配置值如GetAttibuteFormXml(new string[]{"appsettings","add"},
        /// "key","value","connectionString")
        /// 是取得appsettings/add 节点的属性key = connectionString 的Value属性值
        /// </summary>
        /// <param name="nodeNames">节点递进名称</param>
        /// <param name="AttName">属性名称</param>
        /// <param name="AttValname">要返回的属性名称</param>
        /// <param name="keyName">滤出AttName为此的节点</param>
        /// <returns></returns>
        public string GetAttibuteFormXml(string[] nodeNames, string AttName, string AttValname,
            string keyName)
        {
            if (this.m_xml == null)
                return null;
            XmlNodeList nodelist = this.m_xml.DocumentElement.ChildNodes;
            return GetAttibuteFormXml(nodelist, nodeNames, AttName, AttValname, keyName);
        }
        /// <summary>
        /// 取得配置项的配置值
        /// </summary>
        ///<param name="AttName"></param>
        ///<param name="AttValname"></param>
        ///<param name="keyName"></param>
        ///<param name="nodelist"></param>
        ///<param name="nodeNames"></param>
        /// <returns></returns>
        protected string GetAttibuteFormXml(XmlNodeList nodelist, string[] nodeNames,
            string AttName, string AttValname, string keyName)
        {
            if (nodeNames == null || nodeNames.Length == 0)
                return null;
            if (nodelist == null || nodelist.Count == 0)
                return null;

            string nodeName = nodeNames[0];
            string[] subNodes = new string[nodeNames.Length - 1];
            for (int i = 0; i < nodeNames.Length - 1; i++)
            {
                subNodes[i] = nodeNames[i + 1];
            }
            for (int i = 0; i < nodelist.Count; i++)
            {
                XmlNode node = nodelist[i];
                if (String.Compare(node.Name, nodeName, true) == 0)
                {
                    if (nodeNames.Length == 1)
                    {//最后一层节点
                        foreach (XmlNode el in node.Attributes)
                        {
                            if ((AttName == null || AttName.Length == 0) &&
                                (keyName == null || keyName.Length == 0))
                            {
                                if (node.Attributes[AttValname] != null)
                                    return node.Attributes[AttValname].InnerText;
                                return null;
                            }
                            else if (String.Compare(el.Name, AttName, true) == 0 &&
                                String.Compare(el.InnerText, keyName, true) == 0)
                            {
                                return node.Attributes[AttValname].InnerText;

                            }
                        }
                    }
                    else
                    {
                        return GetAttibuteFormXml(node.ChildNodes, subNodes, AttName, AttValname, keyName);
                    }

                }

            }
            return null;
        }

        /// <summary>
        /// 取得node的值
        /// </summary>
        /// <param name="NodeName"></param>
        /// <returns></returns>
        public string GetNodeValue(string NodeName)
        {
            if (NodeName == null || NodeName.Length == 0)
                return NodeName;

            XmlNodeList nodelist = m_xml.DocumentElement.ChildNodes;
            if (nodelist == null || nodelist.Count == 0)
                return null;
            for (int i = 0; i < nodelist.Count; i++)
            {
                XmlNode node = nodelist[i];
                if (String.Compare(node.Name, NodeName, true) == 0)
                {
                    return node.InnerText;
                }
            }
            return "";
        }
        /// <summary>
        /// 设置配置项的值，如：SetAttibuteFormXml(new string[]{"appsettings","add"},
        /// "key","value","connectionString","val")
        /// 是设置appsettings/add 节点的属性key = connectionString 的Value属性值
        /// </summary>
        /// <param name="nodeNames">节点递进名称</param>
        /// <param name="AttName">属性名称</param>
        /// <param name="AttValname">要返回的属性名称</param>
        /// <param name="keyName">滤出AttName为此的节点</param>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public void SetAttibuteToXml(string[] nodeNames, string AttName,
            string AttValname, string keyName, string keyValue)
        {
            if (this.m_xml == null)
                return;
            XmlNodeList nodelist = this.m_xml.DocumentElement.ChildNodes;
            SetAttibuteToXml(nodelist, nodeNames, AttName, AttValname, keyName, keyValue);
        }
        /// <summary>
        /// 设置节点属性值
        /// </summary>
        /// <param name="nodelist"></param>
        /// <param name="nodeNames"></param>
        /// <param name="AttName"></param>
        /// <param name="AttValname"></param>
        /// <param name="keyName"></param>
        /// <param name="keyValue"></param>
        protected void SetAttibuteToXml(XmlNodeList nodelist, string[] nodeNames,
            string AttName, string AttValname, string keyName, string keyValue)
        {
            if (nodeNames == null || nodeNames.Length == 0)
                return;

            if (nodelist == null || nodelist.Count == 0)
                return;
            string nodeName = nodeNames[0];
            string[] subNodes = new string[nodeNames.Length - 1];
            for (int i = 0; i < nodeNames.Length - 1; i++)
            {
                subNodes[i] = nodeNames[i + 1];
            }
            for (int i = 0; i < nodelist.Count; i++)
            {
                XmlNode node = nodelist[i];
                if (String.Compare(node.Name, nodeName, true) == 0)
                {
                    if (nodeNames.Length == 1)
                    {//最后一层节点
                        foreach (XmlNode el in node.Attributes)
                        {
                            if ((AttName == null || AttName.Length == 0) &&
                                (keyName == null || keyName.Length == 0))
                            {
                                if (node.Attributes[AttValname] != null)
                                    node.Attributes[AttValname].InnerText = keyValue;
                                else
                                {
                                    XmlAttribute tmpatt = m_xml.CreateAttribute(AttValname);
                                    tmpatt.InnerText = keyValue;
                                    node.Attributes.Append(tmpatt);
                                }
                                return;
                            }
                            else if (String.Compare(el.Name, AttName, true) == 0 &&
                                String.Compare(el.InnerText, keyName, true) == 0)
                            {
                                node.Attributes[AttValname].InnerText = keyValue;
                                return;

                            }
                        }
                    }
                    else
                    {
                        SetAttibuteToXml(node.ChildNodes, subNodes, AttName, AttValname, keyName, keyValue);
                        return;
                    }
                }

            }
            //没有此属性

            XmlNode newnode = m_xml.CreateNode(XmlNodeType.Element, nodeName, this.m_xml.DocumentElement.NamespaceURI);
            newnode.RemoveAll();

            XmlAttribute attkey = m_xml.CreateAttribute(AttName);
            attkey.InnerText = keyName;

            newnode.Attributes.Append(attkey);

            XmlAttribute attvalue = m_xml.CreateAttribute(AttValname);
            attvalue.InnerText = keyValue;
            newnode.Attributes.Append(attvalue);

            nodelist[0].ParentNode.AppendChild(newnode);

            //			return;

        }

        /// <summary>
        /// 设置配置项值，例：config.SetNodeValue(new string[] { "ServiceList", "Service", "Address" }, "192.168.60.223");
        /// 如果相关的键不存在，会自动创建
        /// </summary>
        public void SetNodeValue(string[] nodeNames, string Value)
        {
            if (nodeNames == null || nodeNames.Length == 0)
                return;
            XmlNodeList nodelist = m_xml.DocumentElement.ChildNodes;
            if (!SetNodeValue(nodelist, nodeNames, Value))
            {
                XmlNode RootNode = m_xml.CreateNode(XmlNodeType.Element, nodeNames[0], null);
                XmlNode tmpnode = RootNode;
                for (int i = 1; i < nodeNames.Length - 1; i++)
                {
                    XmlNode newNode = m_xml.CreateNode(XmlNodeType.Element, nodeNames[i], null);
                    tmpnode.AppendChild(newNode);
                    tmpnode = newNode;
                }
                if (nodeNames.Length > 1)
                {
                    XmlNode lastNode = m_xml.CreateNode(XmlNodeType.Element, nodeNames[nodeNames.Length - 1], null);
                    lastNode.InnerText = Value;
                    tmpnode.AppendChild(lastNode);
                }
                else
                {
                    RootNode.InnerText = Value;
                }
                m_xml.DocumentElement.AppendChild(RootNode);
            }
        }
        /// <summary>
        /// 设置节点值
        /// </summary>
        /// <param name="nodelist"></param>
        /// <param name="nodeNames"></param>
        /// <param name="nodeValue"></param>
        /// <returns></returns>
        protected bool SetNodeValue(XmlNodeList nodelist, string[] nodeNames, string nodeValue)
        {
            if (nodeNames == null || nodeNames.Length == 0)
                return false;
            string[] subNodes = new string[nodeNames.Length - 1];
            for (int i = 0; i < nodeNames.Length - 1; i++)
            {
                subNodes[i] = nodeNames[i + 1];
            }
            string nodeName = nodeNames[0];
            if (nodelist != null && nodelist.Count > 0)
            {


                for (int i = 0; i < nodelist.Count; i++)
                {
                    XmlNode node = nodelist[i];

                    if (String.Compare(node.Name, nodeName, true) == 0)
                    {
                        if (nodeNames.Length == 1)
                        {
                            node.InnerText = nodeValue;
                            return true;
                        }
                        else
                        {
                            return SetNodeValue(node.ChildNodes, subNodes, nodeValue);
                        }
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 设置节点值
        /// </summary>
        /// <param name="nodeName"></param>
        /// <param name="nodeValue"></param>
        public void SetNodeValue(string nodeName, string nodeValue)
        {
            if (nodeName == null || nodeName.Length == 0)
                return;
            XmlNodeList nodelist = m_xml.DocumentElement.ChildNodes;
            if (nodelist != null && nodelist.Count > 0)
            {

                for (int i = 0; i < nodelist.Count; i++)
                {
                    XmlNode node = nodelist[i];
                    if (String.Compare(node.Name, nodeName, true) == 0)
                    {
                        node.InnerText = nodeValue;
                        return;
                    }

                }
            }
            XmlNode newnode = m_xml.CreateNode(XmlNodeType.Element, nodeName, null);
            newnode.InnerText = nodeValue;
            m_xml.DocumentElement.AppendChild(newnode);
        }

        /// <summary>
        /// 保存更改
        /// </summary>
        public void Save()
        {
            if (File.Exists(this.m_FilePath))
            {
                if ((File.GetAttributes(this.m_FilePath) & FileAttributes.ReadOnly)
                    == FileAttributes.ReadOnly)
                {
                    File.SetAttributes(this.m_FilePath, FileAttributes.Normal);
                }
            }
            m_xml.Save(this.m_FilePath);
        }

        /// <summary>
        /// 连接字符串属性值编码
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string ConnStringValueEncoding(string val)
        {
            if (val == null)
                return null;
            return "\"" + val.Replace("\"", "\"\"") + "\"";
        }
    }
}
