using System.IO;
using LSLib.LS;

namespace ExtractPackage
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var fileList = Directory.GetFiles(".\\", "*.pak", SearchOption.AllDirectories);
            foreach (var file in fileList)
            {
                var fullFilePath = Path.GetFullPath(file);
                Util.ExtractPackage(fullFilePath, fullFilePath.Substring(0, fullFilePath.Length - 4));
            }
        }
    }
}