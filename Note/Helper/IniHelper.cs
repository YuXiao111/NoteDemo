using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Note.Helper
{
    public class IniHelper
    {
        private static string Path = ".\\Config\\Note.ini";

        /// <summary>
        /// 写入ini文件
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        private static extern int WritePrivateProfileString(string section, string key, string val, string filePath);


        /// <summary>
        /// 读取ini文件
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="def"></param>
        /// <param name="retVal"></param>
        /// <param name="size"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        private static extern bool GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileInt(string section, string key, int def, string filePath);

        public static void WriteSetting(string path)
        {
            WritePrivateProfileString("Setting", "PowerOnStartup", path, Path);
        }


        public static int ReadSetting()
        {
            int result = GetPrivateProfileInt("Setting", "PowerOnStartup", 0, Path);
            return result;
        }
    }
}
