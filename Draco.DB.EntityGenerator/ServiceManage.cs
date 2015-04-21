using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceProcess;
using System.IO;
using System.Reflection;

namespace CodeGenerator
{
    /// <summary>
    /// ServiceManage 的摘要说明。
    /// </summary>
    public class ServiceManage
    {
        public static bool ExistServer(string svrName)
        {
            try
            {

                ServiceController sc = new ServiceController(svrName);

                if (sc == null)
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool RestartService(string svrName, out string Errormsg)
        {
            Errormsg = null;
            try
            {
                ServiceController sc = new ServiceController(svrName);
                if (sc != null)
                {
                    if (sc.Status.Equals(ServiceControllerStatus.Stopped))
                    {
                        sc.Start();
                    }
                    else
                    {
                        sc.Stop();
                        sc.Close();

                        while (!sc.Status.Equals(ServiceControllerStatus.Stopped))
                        {
                            System.Threading.Thread.Sleep(50);
                            sc.Refresh();
                        }
                        sc.Start();
                    }
                }
                else
                {
                    Errormsg = "未找到服务[" + svrName + "]。";
                    return false;
                }
            }
            catch (Exception ee)
            {
                Errormsg = ee.ToString();
                return false;
            }
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="InteropPath"></param>
        /// <returns></returns>
        public static string GetComDllPath(string InteropPath)
        {
            string path = InteropPath;

            if (!System.IO.File.Exists(path))
            {
                throw new FileNotFoundException(InteropPath + "文件不存在。");
            }
            AssemblyName assName = System.Reflection.AssemblyName.GetAssemblyName(path);
            Assembly ass = Assembly.Load(assName);

            Type[] types = ass.GetTypes();
            string classID = "";
            foreach (Type type in types)
            {
                string guid = type.GUID.ToString().ToUpper();
                if (guid.Length == 36)
                {
                    classID = guid;
                    break;
                }
            }

            Microsoft.Win32.RegistryKey classRoot, InprocServer;

            classRoot = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey("CLSID");
            classRoot = classRoot.OpenSubKey("{" + classID + "}");
            if (classRoot == null)
                throw new Exception(InteropPath + "未注册.");
            InprocServer = classRoot.OpenSubKey("InprocServer32");
            path = InprocServer.GetValue(null).ToString();
            return path;
        }
    }
}
