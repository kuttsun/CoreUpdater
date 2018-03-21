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
            var appName = assembly.GetName().Name;
            var appVersion = "";

            Console.WriteLine($"{appName} {appVersion}");

            // Analyze program arguments

            var cla = new CommandLineApplication(throwOnUnexpectedArg: false)
            {
                // Application name
                Name = appName,
            };

            cla.HelpOption("-?|-h|--help");

            // Create a AppInfo
            cla.Command("create", command =>
            {
                command.Description = "Create a file list.";
                command.HelpOption("-?|-h|--help");

                var targetDir = command.Option("-d|--dir", "Target directory", CommandOptionType.SingleValue);
                var targetAppName = command.Option("-n|--name", "Application name. Default is assembly name", CommandOptionType.SingleValue);
                var targetAppVersion = command.Option("-v|--version", "Application version. Default is assembly version", CommandOptionType.SingleValue);
                var outputFilename = command.Option("-o|--output", "Output file name (Option). Default is AppInfo.json", CommandOptionType.SingleValue);

                command.OnExecute(() =>
                {
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
                Console.Write(e.Message);
                return 1;
            }
        }
    }
}
