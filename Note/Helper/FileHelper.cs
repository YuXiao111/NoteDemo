using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Note.Helper
{
    public class FileHelper
    {
        public static void TryCreateParentDir(string path)
        {
            string parentPath = Path.Combine(System.Environment.CurrentDirectory, path);
            if (!string.IsNullOrEmpty(parentPath) && !Directory.Exists(parentPath))
            {
                Directory.CreateDirectory(parentPath);
            }
        }

        public static void CopyFile(string destFile)
        {
            string fileName = "请在此输入标题.rtf";

            // 1. 目标文件的完整路径 = 目标文件夹 + 文件名
            string destFilePath = Path.Combine(destFile, fileName);

            // 如果目标文件已存在，就不复制
            if (File.Exists(destFilePath))
            {
                Console.WriteLine($"✅ 目标文件已存在，无需复制：{destFilePath}");
                return;
            }

            // 2. 源文件路径 = 程序运行目录下的 Config 文件夹 + 文件名
            string sourceFilePath = Path.Combine("Config", fileName);

            // 3. 检查源文件是否存在
            if (!File.Exists(sourceFilePath))
            {
                Console.WriteLine($"❌ 源文件不存在：{sourceFilePath}");
                return;
            }

            try
            {
                // 4. 执行复制（目标必须是完整文件路径！）
                File.Copy(sourceFilePath, destFilePath, overwrite: true);
                Console.WriteLine($"✅ 文件已复制到：{destFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 复制失败：{ex.Message}");
            }
        }
    }
}
