using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;

using Microsoft.Extensions.CommandLineUtils;

using CoreUpdater.Updates;

namespace CoreUpdater.Console
{
    class Program
    {
        static int Main(string[] args)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var appName = assembly.GetName().Name;
            var appVersion = "";

            System.Console.WriteLine($"{appName} {appVersion}");

            // Analyze program arguments

            var cla = new CommandLineApplication(throwOnUnexpectedArg: false)
            {
                // Application name
                Name = appName,
            };

            cla.HelpOption("-?|-h|--help");

            // Create a CoreUpdaterInfo
            cla.Command("create", command =>
            {
                command.Description = "Create a file list.";
                command.HelpOption("-?|-h|--help");

                var targetDir = command.Option("-d|--dir", "Target directory", CommandOptionType.SingleValue);
                var targetAppName = command.Option("-n|--name", "Application name. Default is assembly name", CommandOptionType.SingleValue);
                var targetAppVersion = command.Option("-v|--version", "Application version. Default is assembly version", CommandOptionType.SingleValue);
                var outputFilename = command.Option("-o|--output", "Output file name (Option). Default is CoreUpdaterInfo.json", CommandOptionType.SingleValue);

                command.OnExecute(() =>
                {
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
            });

            // Start update
            cla.Command("update", command =>
            {
                command.Description = "Start update.";
                command.HelpOption("-?|-h|--help");

                var pid = command.Option("--pid", "Process ID of target application", CommandOptionType.SingleValue);
                var targetAppName = command.Option("-n|--name", "Application name. Default is assembly name", CommandOptionType.SingleValue);
                var sourceDir = command.Option("-d|--dir", "Source directory of latest application", CommandOptionType.SingleValue);

                command.OnExecute(() =>
                {
                    //UpdateManager.Update(pid.Value(), targetAppName.Value(), sourceDir.Value());

                    return 0;
                });
            });

            // Default behavior
            cla.OnExecute(() =>
            {
                cla.ShowHelp();
                return 0;
            });

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
