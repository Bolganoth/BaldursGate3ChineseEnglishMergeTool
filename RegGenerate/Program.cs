using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace RegGenerate
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            StreamWriter sw = new StreamWriter(".\\register.reg", false, Encoding.GetEncoding("GB2312"));
            var currentPath = Path.GetFullPath(".\\");
            var currentPathR = Path.GetFullPath(".\\").Replace("\\", "\\\\");
            sw.WriteLine("Windows Registry Editor Version 5.00\n");
            sw.WriteLine("[HKEY_CLASSES_ROOT\\.pak]");
            sw.WriteLine("@=\"pak_auto_file\"");
            sw.WriteLine();
            sw.WriteLine("[HKEY_CLASSES_ROOT\\pak_auto_file]");
            sw.WriteLine();
            sw.WriteLine("[HKEY_CLASSES_ROOT\\pak_auto_file\\shell]");
            sw.WriteLine();
            sw.WriteLine("[HKEY_CLASSES_ROOT\\pak_auto_file\\shell\\unpack]");
            sw.WriteLine("@=\"解包pak\"");
            sw.WriteLine();
            sw.WriteLine("[HKEY_CLASSES_ROOT\\pak_auto_file\\shell\\unpack\\command]");
            sw.WriteLine("@=\"\\\""+currentPathR+"ExtractPackage.exe\\\" \\\"%1\\\"\"");
            sw.WriteLine();
            sw.WriteLine("[HKEY_CLASSES_ROOT\\Directory\\shell\\PackToPak]");
            sw.WriteLine("@=\"打包为pak\"");
            sw.WriteLine();
            sw.WriteLine("[HKEY_CLASSES_ROOT\\Directory\\shell\\PackToPak\\command]");
            sw.WriteLine("@=\"\\\""+currentPathR+"CreatePackage.exe\\\" \\\"%1\\\"\"");
            sw.WriteLine();
            sw.WriteLine("[HKEY_CLASSES_ROOT\\.loca]");
            sw.WriteLine("@=\"loca_auto_file\"");
            sw.WriteLine();
            sw.WriteLine("[HKEY_CLASSES_ROOT\\loca_auto_file]");
            sw.WriteLine();
            sw.WriteLine("[HKEY_CLASSES_ROOT\\loca_auto_file\\shell]");
            sw.WriteLine();
            sw.WriteLine("[HKEY_CLASSES_ROOT\\loca_auto_file\\shell\\ToXml]");
            sw.WriteLine("@=\"转换为xml\"");
            sw.WriteLine();
            sw.WriteLine("[HKEY_CLASSES_ROOT\\loca_auto_file\\shell\\ToXml\\command]");
            sw.WriteLine("@=\"\\\""+currentPathR+"ConvertLoca.exe\\\" \\\"%1\\\"\"");
            sw.WriteLine();
            sw.WriteLine("[HKEY_CLASSES_ROOT\\SystemFileAssociations\\.xml\\shell]");
            sw.WriteLine();
            sw.WriteLine("[HKEY_CLASSES_ROOT\\SystemFileAssociations\\.xml\\shell\\ToLoca]");
            sw.WriteLine("@=\"转换为loca\"");
            sw.WriteLine();
            sw.WriteLine("[HKEY_CLASSES_ROOT\\SystemFileAssociations\\.xml\\shell\\ToLoca\\command]");
            sw.WriteLine("@=\"\\\""+currentPathR+"ConvertXml.exe\\\" \\\"%1\\\"\"");
            sw.Close();

            var registerFile = currentPath + "register.reg";
            var regeditProcess = Process.Start("regedit.exe", "/s \"" + registerFile + "\"");
            regeditProcess.WaitForExit();
            File.Delete(registerFile);
            Console.WriteLine("Register complete.");
            Console.ReadKey();
        }
    }
}