using System;
using System.IO;
using LSLib.LS;

namespace ExtractPackage {
    internal class Program {
        public static void Main(string[] args) {
            try {
                var path = args[0];
                Util.ExtractPackage(path, path.Substring(0, path.Length - 4));
            }
            catch (Exception) {
                var fileList = Directory.GetFiles(".\\", "*.pak", SearchOption.AllDirectories);
                foreach (var file in fileList) {
                    var fullFilePath = Path.GetFullPath(file);
                    Util.ExtractPackage(fullFilePath, fullFilePath.Substring(0, fullFilePath.Length - 4));
                }
            }
        }
    }
}