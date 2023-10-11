using System;
using System.IO;
using LSLib.LS;

namespace CreatePackage
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var path = args[0];
                Util.CreatePackage(path, path+".pak");
            }
            catch(Exception)
            {
                var fileList = Directory.GetDirectories(Path.GetFullPath(".\\"));
                foreach (var file in fileList)
                {
                    var fullFilePath = Path.GetFullPath(file);
                    Util.CreatePackage(fullFilePath, fullFilePath+".pak");
                }
            }
            
        }
    }
}