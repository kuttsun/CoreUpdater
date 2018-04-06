using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;

using Microsoft.Extensions.CommandLineUtils;

using CoreUpdater;

namespace CoreUpdater.Console
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

            System.Console.WriteLine($"{name} {assemblyInformationalVersion}");

            // Analyze program arguments
            var cla = new CommandLineApplication(throwOnUnexpectedArg: false)
            {
                // Application name
                Name = name,
                Description = "Create CoreUpdaterInfo.json for CoreUpdater.",
            };

            cla.HelpOption("-?|-h|--help");

            var targetDir = cla.Option("-d|--dir", "Target directory", CommandOptionType.SingleValue);
            var targetAppName = cla.Option("-n|--name", "Application name.", CommandOptionType.SingleValue);
            var targetAppVersion = cla.Option("-v|--version", "Application version.", CommandOptionType.SingleValue);
            var outputFilename = cla.Option("-o|--output", "Output file name (Option). Default is CoreUpdaterInfo.json", CommandOptionType.SingleValue);

            // Default behavior
            cla.OnExecute(() =>
            {
                if(targetDir.HasValue() == false || targetAppName.HasValue() == false || targetAppVersion.HasValue() == false)
                {
                    cla.ShowHelp();
                    return 0;
                }

                var files = Directory.GetFiles(targetDir.Value(), "*", SearchOption.AllDirectories);
                var appInfo = new CoreUpdaterInfo()
                {
                    Name = targetAppName.Value(),
                    Version = targetAppVersion.Value()
                };
                appInfo.AddFileInfo(targetDir.Value(), files);
                appInfo.WriteFile(targetDir.Value(), outputFilename.Value() ?? "CoreUpdaterInfo.json");
                return 0;               
            });

            // Execution
            try
            {
                return cla.Execute(args);
            }
            catch (Exception e)
            {
                System.Console.Write(e.Message);
                return 1;
            }
        }
    }
}
