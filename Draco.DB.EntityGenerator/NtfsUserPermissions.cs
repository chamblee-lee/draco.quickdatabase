using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Win32.Security;

namespace CodeGenerator
{
    /// <summary>
    /// NtfsUserPermissions 的摘要说明。
    /// </summary>
    public class NtfsUserPermissions
    {
        /// <summary>
        /// 在XML中存放要赋权的目录的表名
        /// </summary>
        public const string DirsTblName = "DirsTblName";
        /// <summary>
        /// 对用户 strUserName 赋予对文件夹strSitePath 所有的访问权限
        /// </summary>
        /// <param name="strSitePath"></param>
        /// <param name="strUserName"></param>
        /// <returns></returns>
        public static Boolean SetDirPermission(String strSitePath, String strUserName)
        {
            bool IsDir = false;
            if (System.IO.File.Exists(strSitePath))
                IsDir = false;
            else if (!IsDir && !System.IO.Directory.Exists(strSitePath))
                return false;
            else
                IsDir = true;
            Boolean bOk;

            try
            {

                //	Directory.CreateDirectory(strSitePath);

                SecurityDescriptor secDesc = SecurityDescriptor.GetFileSecurity(strSitePath,
                    SECURITY_INFORMATION.DACL_SECURITY_INFORMATION);

                Dacl dacl = secDesc.Dacl;//The discretionary access control list (DACL) of an object 

                Sid sidUser = new Sid(strUserName);
                dacl.RemoveAces(sidUser);

                AccessType AType = AccessType.GENERIC_ALL;
                AceFlags flag = AceFlags.OBJECT_INHERIT_ACE | AceFlags.CONTAINER_INHERIT_ACE | AceFlags.SUCCESSFUL_ACCESS_ACE_FLAG;
                AceAccessAllowed ace = new AceAccessAllowed(sidUser, AType, flag);
                dacl.AddAce(ace);

                secDesc.SetDacl(dacl);
                secDesc.SetFileSecurity(strSitePath, SECURITY_INFORMATION.DACL_SECURITY_INFORMATION);

                bOk = true;

            }
            catch (Exception ee)
            {

                throw ee;

            }
            //对所有的子文件和子文件夹附权
            if (IsDir)
            {
                string[] files = System.IO.Directory.GetFiles(strSitePath);
                if (files != null && files.Length > 0)
                {
                    foreach (string file in files)
                    {
                        SetDirPermission(file, strUserName);
                    }
                }

                string[] dirs = System.IO.Directory.GetDirectories(strSitePath);
                if (dirs != null && dirs.Length > 0)
                {
                    foreach (string dir in dirs)
                    {
                        SetDirPermission(dir, strUserName);
                    }
                }
            }
            return bOk;

        } /* CreateDir */


        public static bool NtfsUserPermise(string WebAppPath, string[] Directorys, out string msg)
        {
            msg = "";
            System.OperatingSystem osvi = WinSys.GetWindowVersion();
            if (osvi.Version.Major < 5)//非NT系统//dwMajorVersion
            {
                msg = "错误，非NT系统！";
                return false;
            }
            if (Directorys == null || Directorys.Length == 0)
                return true;

            foreach (string dir in Directorys)
            {
                if (dir.Trim(new char[] { ' ', '\t' }).Length == 0)
                    continue;
                if (!Directory.Exists(WebAppPath + "\\" + dir))
                {
                    Directory.CreateDirectory(WebAppPath + "\\" + dir);
                }
                NtfsUserPermise(WebAppPath + "\\" + dir, out msg);

            }
            return true;

        }

        public static bool NtfsUserPermise(string dirpath, out string msg)
        {
            msg = "";
            System.OperatingSystem osvi = WinSys.GetWindowVersion();
            if (osvi.Version.Major < 5)//非NT系统//dwMajorVersion
            {
                msg = "错误，非NT系统！";
                return false;
            }
            if (dirpath == null || dirpath.Length == 0)
                return true;
            if (!Directory.Exists(dirpath))
            {
                return true;
            }
            if (osvi.Version.MinorRevision == 0)//2000//dwMinorVersion
            {
                try
                {
                    NtfsUserPermissions.SetDirPermission(dirpath,
                        "ASPNET");
                    NtfsUserPermissions.SetDirPermission(dirpath,
                        "Everyone");
                }
                catch (Exception ee)
                {
                    msg = ee.Message;
                    return false;
                }
            }
            else
            {
                try
                {
                    NtfsUserPermissions.SetDirPermission(dirpath,
                        "NETWORK SERVICE");
                    NtfsUserPermissions.SetDirPermission(dirpath,
                        "Everyone");
                }
                catch (Exception ee)
                {
                    msg = ee.Message;
                    return false;
                }
            }
            return true;
        }
    }
}
