using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace RegGenerate {
    internal class Program {
        public static void Main(string[] args) {
            Console.Write("1.添加右键菜单\n2.删除右键菜单\n选择:");
            var input = Console.ReadLine();
            int.TryParse(input, out var option);
            if (option != 1 && option != 2) {
                Console.WriteLine("错误选项.");
                Console.ReadKey();
                return;
            }

            var registerTag = option == 2 ? "-" : "";
            var sw = new StreamWriter(".\\register.reg", false, Encoding.GetEncoding("GB2312"));
            var currentPath = Path.GetFullPath(".\\");
            var currentPathR = Path.GetFullPath(".\\").Replace("\\", "\\\\");
            sw.WriteLine("Windows Registry Editor Version 5.00\n");
            sw.WriteLine("[" + registerTag + "HKEY_CLASSES_ROOT\\.pak]");
            sw.WriteLine("@=\"pak_auto_file\"");
            sw.WriteLine();
            sw.WriteLine("[" + registerTag + "HKEY_CLASSES_ROOT\\pak_auto_file]");
            sw.WriteLine();
            sw.WriteLine("[" + registerTag + "HKEY_CLASSES_ROOT\\pak_auto_file\\shell]");
            sw.WriteLine();
            sw.WriteLine("[" + registerTag + "HKEY_CLASSES_ROOT\\pak_auto_file\\shell\\unpack]");
            sw.WriteLine("@=\"解包pak\"");
            sw.WriteLine();
            sw.WriteLine("[" + registerTag + "HKEY_CLASSES_ROOT\\pak_auto_file\\shell\\unpack\\command]");
            sw.WriteLine("@=\"\\\"" + currentPathR + "ExtractPackage.exe\\\" \\\"%1\\\"\"");
            sw.WriteLine();
            sw.WriteLine("[" + registerTag + "HKEY_CLASSES_ROOT\\Directory\\shell\\PackToPak]");
            sw.WriteLine("@=\"打包为pak\"");
            sw.WriteLine();
            sw.WriteLine("[" + registerTag + "HKEY_CLASSES_ROOT\\Directory\\shell\\PackToPak\\command]");
            sw.WriteLine("@=\"\\\"" + currentPathR + "CreatePackage.exe\\\" \\\"%1\\\"\"");
            sw.WriteLine();
            sw.WriteLine("[" + registerTag + "HKEY_CLASSES_ROOT\\.loca]");
            sw.WriteLine("@=\"loca_auto_file\"");
            sw.WriteLine();
            sw.WriteLine("[" + registerTag + "HKEY_CLASSES_ROOT\\loca_auto_file]");
            sw.WriteLine();
            sw.WriteLine("[" + registerTag + "HKEY_CLASSES_ROOT\\loca_auto_file\\shell]");
            sw.WriteLine();
            sw.WriteLine("[" + registerTag + "HKEY_CLASSES_ROOT\\loca_auto_file\\shell\\ToXml]");
            sw.WriteLine("@=\"转换为xml\"");
            sw.WriteLine();
            sw.WriteLine("[" + registerTag + "HKEY_CLASSES_ROOT\\loca_auto_file\\shell\\ToXml\\command]");
            sw.WriteLine("@=\"\\\"" + currentPathR + "ConvertLoca.exe\\\" \\\"%1\\\"\"");
            sw.WriteLine();
            sw.WriteLine("[" + registerTag + "HKEY_CLASSES_ROOT\\SystemFileAssociations\\.xml\\shell]");
            sw.WriteLine();
            sw.WriteLine("[" + registerTag + "HKEY_CLASSES_ROOT\\SystemFileAssociations\\.xml\\shell\\ToLoca]");
            sw.WriteLine("@=\"转换为loca\"");
            sw.WriteLine();
            sw.WriteLine("[" + registerTag + "HKEY_CLASSES_ROOT\\SystemFileAssociations\\.xml\\shell\\ToLoca\\command]");
            sw.WriteLine("@=\"\\\"" + currentPathR + "ConvertXml.exe\\\" \\\"%1\\\"\"");
            sw.Close();

            var registerFile = currentPath + "register.reg";
            var regeditProcess = Process.Start("regedit.exe", "/s \"" + registerFile + "\"");
            Trace.Assert(regeditProcess != null, nameof(regeditProcess) + " != null");
            regeditProcess.WaitForExit();
            File.Delete(registerFile);
            switch (option) {
                case 1:
                    Console.WriteLine("添加成功.");
                    break;
                case 2:
                    Console.WriteLine("删除成功.");
                    break;
            }

            Console.ReadKey();
        }
    }
}