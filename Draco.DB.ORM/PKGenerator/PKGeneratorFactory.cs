using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Data;
using Draco.DB.ORM.Common;
using Draco.DB.QuickDataBase;
using Draco.DB.ORM.PKGenerator.Common;
using System.Collections;

namespace Draco.DB.ORM.PKGenerator
{
    /// <summary>
    /// 主键生成器工厂
    /// </summary>
    public class PKGeneratorFactory
    {
        //存储名称--类型 键值对
        private static Hashtable m_PKGTypeTable = new Hashtable(StringComparer.CurrentCultureIgnoreCase);
        //存储名称--对象 键值对
        private static Hashtable m_PKGTable = new Hashtable(StringComparer.CurrentCultureIgnoreCase);
        
        /// <summary>
        /// 获取主键生成器
        /// </summary>
        /// <param name="PKGeneratorName">主键生成器名称</param>
        /// <param name="DbHandle"></param>
        /// <returns></returns>
        public static IPKGenerator GetPKGenerator(string PKGeneratorName, IDataBaseHandler DbHandle)
        {
            if (String.IsNullOrEmpty(PKGeneratorName))
                throw new ArgumentNullException("PKGeneratorName can not be empty!");
            if (m_PKGTable.ContainsKey(PKGeneratorName))
            {
                IPKGenerator PKG = m_PKGTable[PKGeneratorName] as IPKGenerator;
                if (PKG != null && PKG.IsReusable)
                {
                    return PKG;
                }
            }
            IPKGenerator NewPKG = CreatePKGenerator(PKGeneratorName, DbHandle);
            m_PKGTable[PKGeneratorName] = NewPKG;
            return NewPKG;
        }
        /// <summary>
        /// 创建主键生成器
        /// </summary>
        /// <param name="PKGeneratorName">主键生成器名称</param>
        /// <param name="DbHandle"></param>
        /// <returns></returns>
        private static IPKGenerator CreatePKGenerator(String PKGeneratorName, IDataBaseHandler DbHandle)
        {
            Type PKGType = m_PKGTypeTable[PKGeneratorName] as Type;
            if (PKGType == null)
            {
                PKGType = GetPKGeneratorType(PKGeneratorName);
                m_PKGTypeTable[PKGeneratorName] = PKGType;
            }
            Object obj = Activator.CreateInstance(PKGType);
            IDBReferencePKGenerator DBReferencePKG = obj as IDBReferencePKGenerator;
            if (DBReferencePKG != null)
            {
                DBReferencePKG.DBHandler = DbHandle;
            }
            return obj as IPKGenerator;
        }
        /// <summary>
        /// 获取主键生成器类型
        /// </summary>
        /// <param name="PKGeneratorName">主键生成器名称</param>
        /// <returns></returns>
        private static Type GetPKGeneratorType(String PKGeneratorName)
        {
            switch (PKGeneratorName.ToUpper())
            {
                case PKGeneratorEnum.GUID: return typeof(GUIDPKGenerator);
                case PKGeneratorEnum.SIMPLEGUID: return typeof(SimpleGUIDPKGenerator);
                case PKGeneratorEnum.TIMESTAMP: return typeof(TimestampPKGenerator);
                case "INTEGER-SEQUENCE-COMPATIBLE": return GetPKGeneratorType("Draco.DB.ORM.Compatible.PKGeneratorCompatible,Draco.DB.ORM");
            }
            //自定义的生成器
            AssemblyName assemblyName = GetAssemblyName(PKGeneratorName);
            string ClassName = GetClassName(PKGeneratorName);
            if (assemblyName != null && !String.IsNullOrEmpty(ClassName))
            {
                Assembly assembly = GetAssembly(assemblyName);
                Type type = assembly.GetType(ClassName);
                return type;
            }
            throw new ArgumentException("无法解析的主键生成器名称PKGeneratorName=" + PKGeneratorName);
        }

        #region 反射获取
        /// <summary>
        /// 解析程序集名称
        /// </summary>
        /// <param name="longName"></param>
        /// <returns></returns>
        private static AssemblyName GetAssemblyName(string longName)
        {
            if (!String.IsNullOrEmpty(longName))
            {
                /*
                 //* 第一种形式--自定义串：Draco.Security.dll:Draco.Security.MemberRuntime.SetAllUserOffline
                 //* 第二种形式--标准程序集名称:System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
                 //* 第三种形式--带类型的强命名程序集：System.Web.Script.Services.ScriptHandlerFactory, System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
                 //* 第三种形式--带类型的弱命名程序集：Draco.HttpModule.MyHttpModule, Draco.HttpModule
                */
                if (longName.Contains(":"))
                {
                    string name = longName.Substring(0, longName.IndexOf(":"));
                    name = name.Trim();
                    if (name.ToLower().EndsWith(".dll"))
                        name = name.Substring(0, name.Length - 4);
                    return new AssemblyName(name);
                }
                if (longName.Contains(","))//标准程序集名称
                {
                    int index = longName.IndexOf(",");//第一个逗号
                    string firstName = longName.Substring(0, index);

                    string secondName = longName.Substring(index + 1);
                    int index2 = secondName.IndexOf(",");
                    if (index2 > 0)
                        secondName = secondName.Substring(0, index2);

                    if (secondName.Contains("="))
                    {
                        return new AssemblyName(longName);//第二种形式--标准程序集名称
                    }
                    else//第三种形式，firstName是类型
                    {
                        string name = longName.Substring(index + 1);
                        return new AssemblyName(name);
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 获取程序集
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        private static Assembly GetAssembly(AssemblyName assemblyName)
        {
            if (assemblyName != null)
            {
                //先从当前域查找
                Assembly[] allass = System.AppDomain.CurrentDomain.GetAssemblies();
                for (int i = 0; i < allass.Length; i++)
                {
                    if (allass[i].FullName == assemblyName.FullName)
                    {
                        return allass[i];
                    }
                }
                //没有找到就加载
                try
                {
                    //反射程序集获取程序集和类对象
                    Assembly assb = Assembly.Load(assemblyName);
                    return assb;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            return null;
        }
        /// <summary>
        /// 解析类全名称
        /// </summary>
        /// <param name="longName"></param>
        /// <returns></returns>
        private static string GetClassName(string longName)
        {
            if (!String.IsNullOrEmpty(longName))
            {
                /*
                 * 第一种形式：Draco.Security.dll:Draco.Security.MemberRuntime.SetAllUserOffline
                 * 第二种形式：System.Web.Script.Services.ScriptHandlerFactory, System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
                 * */

                if (longName.Contains(":"))
                {
                    string name = longName.Substring(longName.IndexOf(":") + 1);
                    name = name.Substring(0, name.LastIndexOf("."));
                    return name;
                }
                if (longName.Contains(","))//标准程序集名称
                {
                    int index = longName.IndexOf(",");//第一个逗号
                    string name = longName.Substring(0, index);
                    return name;
                }
            }
            return "";
        }
        #endregion
    }
}
