using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Draco.DB.QuickDataBase.Utility
{
    /// <summary>
    /// 迷你对象工厂
    /// </summary>
    public class ObjectFactory
    {
        /// <summary>
        /// 获取程序集
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public static Assembly GetAssembly(AssemblyName assemblyName)
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
        /// 获取对象
        /// </summary>
        /// <param name="TypeString"></param>
        /// <returns></returns>
        public static Object CreateObject(string TypeString)
        {
            if (!String.IsNullOrEmpty(TypeString))
            {
                int index = TypeString.IndexOf(",");
                if (index > 0)
                {
                    string TypeName = TypeString.Substring(0, index);
                    string assemblyName = TypeString.Substring(index + 1);
                    Assembly assembly = GetAssembly(new AssemblyName(assemblyName));
                    Type type = assembly.GetType(TypeName);
                    object instance = assembly.CreateInstance(type.FullName, false, BindingFlags.CreateInstance,
                                            Type.DefaultBinder, null, System.Globalization.CultureInfo.CurrentCulture, null);
                    return instance;
                }
            }
            return null;
        }
    }
}
