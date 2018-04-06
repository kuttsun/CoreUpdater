using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;
using System.IO;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.CommandLineUtils;

namespace CoreUpdater
{
    abstract public class UpdateManager : IUpdateManager
    {
        public string AppName { get; set; }
        public string CoreUpdaterInfoFileName { get; set; } = "CoreUpdaterInfo.json";

        protected ILogger logger;

        protected bool preRelease = false;

        readonly string coreUpdaterStartingArgument = "CoreUpdaterStarting";
        readonly string coreUpdaterCompletedArgument = "CoreUpdaterCompleted";

        public UpdateManager(string coreUpdaterInfoFileName, ILogger logger)
        {
            this.logger = logger;
            if (coreUpdaterInfoFileName != null)
            {
                CoreUpdaterInfoFileName = coreUpdaterInfoFileName;
            }

            AppName = Assembly.GetExecutingAssembly().GetName().Name;
        }

        public abstract Task<CoreUpdaterInfo> CheckForUpdatesAsync();
        public abstract Task<CoreUpdaterInfo> PrepareForUpdatesAsync(string outputDir, string zipFileName);

        /// <summary>
        /// Prepare for updates.
        /// Please call StartUpdater method and close the application after this method.
        /// </summary>
        /// <param name="inputPath"></param>
        /// <param name="outputPath"></param>
        /// <returns></returns>
        public abstract Task<CoreUpdaterInfo> PrepareForUpdatesAsync(string outputPath);

        public void Update(string[] args)
        {
            // Analyze program arguments
            var cla = new CommandLineApplication(throwOnUnexpectedArg: false);

            var arg = cla.Argument(coreUpdaterStartingArgument, "CoreUpdater is starting");
            var pid = cla.Option("--pid", "Process ID of target application", CommandOptionType.SingleValue);
            var name = cla.Option("-n|--name", "Application name. Default is assembly name", CommandOptionType.SingleValue);
            var srcDir = cla.Option("-s|--src", "Source directory (new version directory)", CommandOptionType.SingleValue);
            var dstDir = cla.Option("-d|--dst", "Destination directory (current version direcroty)", CommandOptionType.SingleValue);

            // Default behavior
            cla.OnExecute(() =>
            {
                if (CanUpdate(args))
                {
                    Update(pid.Value(), srcDir.Value(), dstDir.Value());
                }
                return 0;
            });

            // Execution
            cla.Execute(args);
        }
        /// <summary>
        /// Update the application.
        /// This method implements the update after the application closes.
        /// The application will start up after the update is completed.
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="srcDir"></param>
        /// <param name="dstDir"></param>
        /// <exception cref="TimeoutException"></exception>
        void Update(string pid, string srcDir, string dstDir)
        {
            if (pid != null)
            {
                logger?.LogInformation($"Wait for the target application ({pid}) to finish...");
                WaitForExit(Convert.ToInt32(pid));
            }

            logger?.LogInformation($"Start updates.{Process.GetCurrentProcess().Id}");

            // Start updates.
            var currentAppInfo = CoreUpdaterInfo.ReadFile($@"{dstDir}\{CoreUpdaterInfoFileName}");
            var newAppInfo = CoreUpdaterInfo.ReadFile($@"{srcDir}\{CoreUpdaterInfoFileName}");

            // Delete file in current dir.
            DeleteFiles(dstDir, currentAppInfo);

            // Copy file to current dir fron new dir.
            CopyFiles(srcDir, dstDir, newAppInfo);
        }

        public void RestartApplication(string[] args, bool result = true)
        {
            // Analyze program arguments
            var cla = new CommandLineApplication(throwOnUnexpectedArg: false);

            var arg = cla.Argument(coreUpdaterStartingArgument, "CoreUpdater is starting");
            var pid = cla.Option("--pid", "Process ID of target application", CommandOptionType.SingleValue);
            var name = cla.Option("-n|--name", "Application name. Default is assembly name", CommandOptionType.SingleValue);
            var srcDir = cla.Option("-s|--src", "Source directory (new version directory)", CommandOptionType.SingleValue);
            var dstDir = cla.Option("-d|--dst", "Destination directory (current version direcroty)", CommandOptionType.SingleValue);

            // Default behavior
            cla.OnExecute(() =>
            {
                // Restart application
                Process.Start($@"{dstDir.Value()}\{name.Value()}", $"{coreUpdaterCompletedArgument} -r {(result ? UpdateCompletedType.Success : UpdateCompletedType.Failure)}");
                return 0;
            });

            // Execution
            cla.Execute(args);
        }

        public void StartUpdater(CoreUpdaterInfo coreUpdaterInfo)
        {
            var assemblyName = Path.GetFileName(Assembly.GetExecutingAssembly().Location);
            var srcDir = Path.GetFullPath(coreUpdaterInfo.GetNewVersionDir());
            var dstDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var processId = Process.GetCurrentProcess().Id.ToString();

            StartUpdater(assemblyName, srcDir, dstDir, processId);
        }

        void StartUpdater(string assemblyName, string srcDir, string dstDir, string processId)
        {
            Process.Start($@"{srcDir}\{assemblyName}", $"{coreUpdaterStartingArgument} --pid={processId} -n={assemblyName} -s={srcDir} -d={dstDir}");
        }

        public bool CanUpdate(string[] args)
        {
            // Analyze program arguments
            var cla = new CommandLineApplication(throwOnUnexpectedArg: false);

            var arg = cla.Argument(coreUpdaterStartingArgument, "CoreUpdater is starting");
            var pid = cla.Option("--pid", "Process ID of target application", CommandOptionType.SingleValue);
            var name = cla.Option("-n|--name", "Application name. Default is assembly name", CommandOptionType.SingleValue);
            var srcDir = cla.Option("-s|--src", "Source directory (new version directory)", CommandOptionType.SingleValue);
            var dstDir = cla.Option("-d|--dst", "Destination directory (current version direcroty)", CommandOptionType.SingleValue);

            cla.OnExecute(() =>
             {
                 if (arg.Value != coreUpdaterStartingArgument || pid.HasValue() == false || name.HasValue() == false || srcDir.HasValue() == false || dstDir.HasValue() == false)
                 {
                     return 1;
                 }
                 else
                 {
                     return 0;
                 }
             });

            // Execution
            return cla.Execute(args) == 0 ? true : false;
        }

        public UpdateCompletedType Completed(string[] args)
        {
            // Analyze program arguments
            var cla = new CommandLineApplication(throwOnUnexpectedArg: false);

            var arg = cla.Argument(coreUpdaterCompletedArgument, "Update is completed by CoreUpdater");
            var result = cla.Option("-r|--result", "Update results", CommandOptionType.SingleValue);

            // Default behavior
            cla.OnExecute(() =>
            {
                if (arg.Value != coreUpdaterCompletedArgument || result.HasValue() == false)
                {
                    return (int)UpdateCompletedType.None;
                }

                if (result.Value() == UpdateCompletedType.Success.ToString())
                {
                    return (int)UpdateCompletedType.Success;
                }
                else
                {
                    return (int)UpdateCompletedType.Failure;
                }
            });

            // Execution
            return (UpdateCompletedType)Enum.ToObject(typeof(UpdateCompletedType), cla.Execute(args));
        }

        void WaitForExit(int pid)
        {
            try
            {
                var process = Process.GetProcessById(pid);
                process.WaitForExit(10000);
                if (process.HasExited == false)
                {
                    throw new TimeoutException();
                }
            }
            catch (SystemException e)
            {
                logger?.LogInformation(e.Message);
                throw;
            }
        }

        void DeleteFiles(string dir, CoreUpdaterInfo appinfo)
        {
            // Delete file in current dir.
            foreach (var file in appinfo.Files)
            {
                File.Delete($@"{dir}\{file.Name}");
                logger?.LogDebug($@"[Delete] {dir}\{file.Name}");
            }
            File.Delete($@"{dir}\{CoreUpdaterInfoFileName}");
            logger?.LogDebug($@"[Delete] {dir}\{CoreUpdaterInfoFileName}");
        }

        void CopyFiles(string srcDir, string dstDir, CoreUpdaterInfo srcAppinfo)
        {
            foreach (var file in srcAppinfo.Files)
            {
                File.Copy($@"{srcDir}\{file.Name}", $@"{dstDir}\{file.Name}", true);
                logger?.LogDebug($@"[Copy] {srcDir}\{file.Name}");
            }
            File.Copy($@"{srcDir}\{CoreUpdaterInfoFileName}", $@"{dstDir}\{CoreUpdaterInfoFileName}", true);
            logger?.LogDebug($@"[Copy] {srcDir}\{CoreUpdaterInfoFileName}");
        }
    }
}
