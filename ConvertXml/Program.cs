using System.IO;
using LSLib.LS;

namespace ConvertXml
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var fileList = Directory.GetFiles(".\\", "*.xml", SearchOption.AllDirectories);
            foreach (var file in fileList)
            {
                var fullFilePath = Path.GetFullPath(file);
                Util.LocalizationConvert(fullFilePath, fullFilePath.Substring(0, fullFilePath.Length - 4) + ".loca");
            }
        }
    }
}