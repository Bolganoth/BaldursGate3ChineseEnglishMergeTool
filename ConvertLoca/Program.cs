using System;
using System.IO;
using LSLib.LS;

namespace ConvertLoca {
    internal class Program {
        public static void Main(string[] args) {
            try {
                var path = args[0];
                Util.LocalizationConvert(path, path.Substring(0, path.Length - 5) + ".xml");
            }
            catch (Exception) {
                var fileList = Directory.GetFiles(".\\", "*.loca", SearchOption.AllDirectories);
                foreach (var file in fileList) {
                    var fullFilePath = Path.GetFullPath(file);
                    Util.LocalizationConvert(fullFilePath, fullFilePath.Substring(0, fullFilePath.Length - 5) + ".xml");
                }
            }
        }
    }
}