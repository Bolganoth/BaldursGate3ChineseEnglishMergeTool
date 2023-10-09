using System;
using LSLib.LS.Enums;

namespace LSLib.LS
{
    public class Util
    {
        public static void CreatePackage(String createSrcPath, String createPackagePath)
        {
            try
            {
                Console.WriteLine("Creating package:\n" + createSrcPath);
                var options = new PackageCreationOptions();
                options.Version = PackageVersion.V18;
                options.Compression = CompressionMethod.LZ4;
                options.Priority = 0;
        
                var packager = new Packager();
                packager.CreatePackage(createPackagePath, createSrcPath, options);
        
                Console.WriteLine("Package created successfully.");
            }
            catch (Exception exc)
            {
                Console.WriteLine($"Internal error!{Environment.NewLine}{Environment.NewLine}{exc}");
            }
        }

        public static void ExtractPackage(String extractPackagePath, String extractionPath)
        {
            try
            {
                Console.WriteLine("Extracting package:\n" + extractPackagePath);
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
                Console.WriteLine("Converting localization file:\n" + inputFile);
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
    }
}