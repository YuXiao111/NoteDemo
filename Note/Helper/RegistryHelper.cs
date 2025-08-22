using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Note.Helper
{
    public class RegistryHelper
    {
        public static void RegistryAutoStart(bool isAutoStart)
        {
            try
            {
                // 使用 WPF 获取应用程序路径的正确方式
                string executablePath = System.Reflection.Assembly.GetExecutingAssembly().Location;

                // 确保路径包含在引号中，防止空格问题
                executablePath = "\"" + executablePath + "\"";

                using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(
                    "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                {
                    if (registryKey == null)
                    {
                        MessageBox.Show("无法访问注册表项，可能是权限不足。");
                        return;
                    }

                    if (isAutoStart)
                    {
                        registryKey.SetValue("Note", executablePath);
                        //new OKDialogView("已设置开机自启：" + executablePath).ShowDialog();
                        IniHelper.WriteSetting("1");
                        registryKey.Close();
                    }
                    else
                    {
                        if (registryKey.GetValue("Note") != null)
                        {
                            registryKey.DeleteValue("Note", false);
                            //new OKDialogView("已取消开机自启").ShowDialog();
                            IniHelper.WriteSetting("0");
                            registryKey.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
               MessageBox.Show("设置开机自启时出错：" + ex.Message);
            }
        }
    }
}
