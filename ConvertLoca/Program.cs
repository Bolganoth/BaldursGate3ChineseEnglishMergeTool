using System.IO;
using LSLib.LS;

namespace ConvertLoca
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var fileList = Directory.GetFiles(".\\", "*.loca", SearchOption.AllDirectories);
            foreach (var file in fileList)
            {
                var fullFilePath = Path.GetFullPath(file);
                Util.LocalizationConvert(fullFilePath, fullFilePath.Substring(0, fullFilePath.Length - 5) + ".xml");
            }
        }
    }
}