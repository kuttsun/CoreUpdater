using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;

using Microsoft.Extensions.CommandLineUtils;

using CoreUpdater.Updates;

namespace CoreUpdater
{
    class Program
    {
        static int Main(string[] args)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);

            var name = assembly.GetName().Name;
            var assemblyName = Path.GetFileName(assembly.Location);
            var assemblyVersion = assembly.GetName().Version.ToString();
            var assemblyFileVersion = fvi.FileVersion;
            var assemblyInformationalVersion = fvi.ProductVersion;

            Console.WriteLine($"{name} {assemblyInformationalVersion}");

            // Analyze program arguments
            var cla = new CommandLineApplication(throwOnUnexpectedArg: false)
            {
                // Application name
                Name = name,
            };

            cla.HelpOption("-?|-h|--help");

            var targetDir = cla.Option("-d|--dir", "Target directory", CommandOptionType.SingleValue);
            var targetAppName = cla.Option("-n|--name", "Application name. Default is assembly name", CommandOptionType.SingleValue);
            var targetAppVersion = cla.Option("-v|--version", "Application version. Default is assembly version", CommandOptionType.SingleValue);
            var outputFilename = cla.Option("-o|--output", "Output file name (Option). Default is AppInfo.json", CommandOptionType.SingleValue);

            // Default behavior
            cla.OnExecute(() =>
            {
                if (targetDir.HasValue() == false || targetAppName.HasValue() == false || targetAppVersion.HasValue() == false)
                {
                    cla.ShowHelp();
                    return 0;
                }

                // Create a AppInfo
                var files = Directory.GetFiles(targetDir.Value(), "*", SearchOption.AllDirectories);
                var appInfo = new AppInfo()
                {
                    Name = targetAppName.Value(),
                    Version = targetAppVersion.Value()
                };
                appInfo.AddFileInfo(targetDir.Value(), files);
                appInfo.WriteFile(targetDir.Value(), outputFilename.Value() ?? "AppInfo.json");
                return 0;
            });

            try
            {
                return cla.Execute(args);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                return 1;
            }
        }
    }
}
