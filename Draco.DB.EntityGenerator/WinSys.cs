using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace CodeGenerator
{
    /// <summary>
    /// WinSys
    /// </summary>
    public abstract class WinSys
    {
        /// <summary>
        /// 取得windows路径
        /// </summary>
        /// <param name="WinDir"></param>
        /// <param name="count"></param>
        [DllImport("kernel32")]
        private static extern void GetWindowsDirectory(StringBuilder WinDir, int count);
        /// <summary>
        /// 取得文件系统信息
        /// </summary>
        /// <param name="RootPathName"></param>
        /// <param name="VolumeNameBuffer"></param>
        /// <param name="VolumeNameSize"></param>
        /// <param name="VolumeSerialNumber"></param>
        /// <param name="MaximumComponentLength"></param>
        /// <param name="FileSystemFlags"></param>
        /// <param name="FileSystemNameBuffer"></param>
        /// <param name="nFileSystemNameSize"></param>
        /// <returns></returns>
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        extern static bool GetVolumeInformation(
            string RootPathName,
            StringBuilder VolumeNameBuffer,
            int VolumeNameSize,
            out uint VolumeSerialNumber,
            out uint MaximumComponentLength,
            out uint FileSystemFlags,
            StringBuilder FileSystemNameBuffer,
            int nFileSystemNameSize);

        /// <summary>
        /// 取得windows版本
        /// </summary>
        /// <returns></returns>
        public static OperatingSystem GetWindowVersion()
        {
            return System.Environment.OSVersion;
        }
        /// <summary>
        /// 取得磁盘文件系统格式
        /// </summary>
        /// <param name="Diskname"></param>
        /// <returns></returns>
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        public static string GetDiskFileSystem(char Diskname)
        {

            StringBuilder volname = new StringBuilder(256);
            StringBuilder fsname = new StringBuilder(256);
            uint sernum, maxlen, flags;
            if (!GetVolumeInformation(Diskname + @":\", volname, volname.Capacity, out sernum, out maxlen, out flags, fsname, fsname.Capacity))
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            return fsname.ToString();
        }
        /// <summary>
        /// 取得windows路径
        /// </summary>
        /// <returns></returns>
        public static string GetWindowsDirectory()
        {
            StringBuilder sb = new StringBuilder(1024);
            GetWindowsDirectory(sb, 1024);
            return sb.ToString();
        }
        /// <summary>
        /// 取得系统路径
        /// </summary>
        /// <returns></returns>
        public static string GetSystemDirectory()
        {
            return System.Environment.SystemDirectory;
        }
        [DllImport("mscoree.dll")]
        static extern uint GetCORSystemDirectory([MarshalAs(UnmanagedType.LPTStr)]System.Text.StringBuilder Buffer,
            int BufferLength, ref int Length);
        /// <summary>
        /// 获取当前运行的Clr的路径
        /// </summary>
        /// <returns></returns>
        public static string GetCORSystemDirectory()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(1024);
            int size = 0;
            GetCORSystemDirectory(sb, sb.Capacity, ref size);
            return sb.ToString();
        }
    }
}
