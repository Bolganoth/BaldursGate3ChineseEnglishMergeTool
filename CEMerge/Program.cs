using System;
using System.IO;
using LSLib.LS;

namespace CEMerge
{
    internal class Program
    {
        // public static void CreatePackage(String createSrcPath, String createPackagePath)
        // {
        //     try
        //     {
        //         var options = new PackageCreationOptions();
        //         options.Version = PackageVersion.V18;
        //         options.Compression = CompressionMethod.LZ4;
        //         options.Priority = 0;
        //
        //         var packager = new Packager();
        //         packager.CreatePackage(createPackagePath, createSrcPath, options);
        //
        //         Console.WriteLine("Package created successfully.");
        //     }
        //     catch (Exception exc)
        //     {
        //         Console.WriteLine($"Internal error!{Environment.NewLine}{Environment.NewLine}{exc}");
        //     }
        // }

        private static void ExtractPackage(String extractPackagePath, String extractionPath)
        {
            try
            {
                Console.WriteLine("Extracting Package:\n" + extractPackagePath);
                var packager = new Packager();
                packager.UncompressPackage(extractPackagePath, extractionPath);
                Console.WriteLine("Package extracted successfully.\n");
            }
            catch (NotAPackageException)
            {
                Console.WriteLine(
                    $"The specified file ({extractPackagePath}) is not an PAK package or savegame archive.",
                    "Extraction Failed");
            }
            catch (Exception exc)
            {
                Console.WriteLine($"Internal error!{Environment.NewLine}{Environment.NewLine}{exc}",
                    "Extraction Failed");
            }
        }

        public static void LocalizationConvert(String inputFile, String outputFile)
        {
            try
            {
                Console.WriteLine("Converting Localization File:\n" + inputFile);
                var resource = LocaUtils.Load(inputFile);
                var format = LocaUtils.ExtensionToFileFormat(outputFile);
                LocaUtils.Save(resource, outputFile, format);
                Console.WriteLine("Localization file converted successfully.\n");
            }
            catch (Exception exc)
            {
                Console.WriteLine($"Internal error!{Environment.NewLine}{Environment.NewLine}{exc}");
            }
        }

        public static void Main(string[] args)
        {
            var dataPath = Path.GetFullPath("..\\Data");
            var englishPakCheck = "\\Localization\\English.pak";
            if (!File.Exists(Path.GetFullPath(dataPath + englishPakCheck)))
            {
                Console.WriteLine("Cannot find localization package, please manually enter absolute path of \'Data\' directory.");
                dataPath = Console.ReadLine();
                if (!File.Exists(Path.GetFullPath(dataPath + englishPakCheck)))
                {
                    Console.WriteLine("Failed to locate \'Data\' directory.\nPress any key to exit.");
                    Console.ReadKey();
                    return;
                }
            }
            var englishPackagePath = dataPath + "\\Localization\\English.pak";
            var englishPackageExtractPath = dataPath + "\\Localization\\English";
            var englishLocaPath = dataPath + "\\Localization\\English\\Localization\\English\\english.loca";
            var englishXmlPath = dataPath + "\\Localization\\English\\Localization\\English\\english.xml";
            var chinesePackagePath = dataPath + "\\Localization\\Chinese\\Chinese.pak";
            var chinesePackageExtractPath = dataPath + "\\Localization\\Chinese\\Chinese";
            var chineseLocaPath = dataPath + "\\Localization\\Chinese\\Chinese\\Localization\\Chinese\\chinese.loca";
            var chineseXmlPath = dataPath + "\\Localization\\Chinese\\Chinese\\Localization\\Chinese\\chinese.xml";
            var mergeLocaPath = dataPath + "\\Localization\\Chinese\\chinese.loca";
            var mergeXmlPath = dataPath + "\\Localization\\Chinese\\chinese.xml";
            var personalRevisePath = Path.GetFullPath("BG3MergeChange.xml");
            
            ExtractPackage(englishPackagePath, englishPackageExtractPath);
            ExtractPackage(chinesePackagePath, chinesePackageExtractPath);
            LocalizationConvert(englishLocaPath, englishXmlPath);
            LocalizationConvert(chineseLocaPath, chineseXmlPath);
            CemBaldursGate3.MergeTranslations(englishXmlPath, chineseXmlPath, mergeXmlPath, personalRevisePath);
            LocalizationConvert(mergeXmlPath, mergeLocaPath);
            Console.WriteLine("Deleting extra files...");
            File.Delete(mergeXmlPath);
            Directory.Delete(englishPackageExtractPath, true);
            Directory.Delete(chinesePackageExtractPath, true);
            Console.WriteLine("Files deleted.\n");
            Console.WriteLine("Merge complete, merged file at:\n" + mergeLocaPath + "\n");
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
        }
    }
}