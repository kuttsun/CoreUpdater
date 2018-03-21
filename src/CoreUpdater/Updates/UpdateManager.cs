using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;
using System.IO;

using Microsoft.Extensions.Logging;

namespace CoreUpdater.Updates
{
    abstract public class UpdateManager
    {
        public string AppName { get; set; }
        public string AppInfoFileName { get; set; } = "AppInfo.json";

        protected ILogger logger;

        protected bool preRelease = false;

        public UpdateManager(ILogger logger)
        {
            this.logger = logger;

            AppName = Assembly.GetExecutingAssembly().GetName().Name;
        }

        abstract public Task<AppInfo> CheckForUpdateAsync();
        abstract public Task<AppInfo> PrepareForUpdate(string outputDir, string zipFileName);

        /// <summary>
        /// Prepare for update.
        /// Please call ReserveForUpdate method and close the application after this method.
        /// </summary>
        /// <param name="inputPath"></param>
        /// <param name="outputPath"></param>
        /// <returns></returns>
        abstract public Task<AppInfo> PrepareForUpdate(string outputPath);

        /// <summary>
        /// Update the application.
        /// This method implements the update after the application closes.
        /// The application will start up after the update is completed.
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="srcDir"></param>
        /// <param name="dstDir"></param>
        /// <exception cref="TimeoutException"></exception>
        public void Update(string pid, string srcDir, string dstDir)
        {
            if (pid != null)
            {
                logger?.LogInformation($"Wait for the target application ({pid}) to finish...");
                WaitForExit(Convert.ToInt32(pid));
            }

            logger?.LogInformation($"Start updates.{Process.GetCurrentProcess().Id}");

            // Start updates.
            var currentAppInfo = AppInfo.ReadFile($@"{dstDir}\{AppInfoFileName}");
            var newAppInfo = AppInfo.ReadFile($@"{srcDir}\{AppInfoFileName}");

            // Delete file in current dir.
            DeleteFiles(dstDir, currentAppInfo);

            // Copy file to current dir fron new dir.
            CopyFiles(srcDir, dstDir, newAppInfo);
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
            }
        }

        void DeleteFiles(string dir, AppInfo appinfo)
        {
            // Delete file in current dir.
            foreach (var file in appinfo.Files)
            {
                File.Delete($@"{dir}\{file.Name}");
                logger?.LogDebug($@"[Delete] {dir}\{file.Name}");
            }
            File.Delete($@"{dir}\{AppInfoFileName}");
            logger?.LogDebug($@"[Delete] {dir}\{AppInfoFileName}");
        }

        void CopyFiles(string srcDir, string dstDir, AppInfo srcAppinfo)
        {
            foreach (var file in srcAppinfo.Files)
            {
                File.Copy($@"{srcDir}\{file.Name}", $@"{dstDir}\{file.Name}", true);
                logger?.LogDebug($@"[Copy] {srcDir}\{file.Name}");
            }
            File.Copy($@"{srcDir}\{AppInfoFileName}", $@"{dstDir}\{AppInfoFileName}", true);
            logger?.LogDebug($@"[Copy] {srcDir}\{AppInfoFileName}");
        }
    }
}
