using System;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

using CoreUpdater;

namespace CoreUpdater.Sample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("--------------------");
            Console.WriteLine("Version." + Assembly.GetExecutingAssembly().GetName().Version);

            IUpdateManager mgr = new GitHub("https://github.com/kuttsun/Test");
            // UpdateManager mgr = new Storage(@"C:\foo");

            if (mgr.CanUpdate(args))
            {
                Console.WriteLine("Start Update.");

                // 6. Update
                mgr.Update(args);

                // 7. Restart application
                mgr.RestartApplication(args, ExecutionType.DotNetCore);

                // 8. Close application
                return;
            }

            // 9. Update is complete
            switch (mgr.Completed(args))
            {
                case UpdateCompletedType.Success:
                    Console.WriteLine("Update succeeded.");
                    break;
                case UpdateCompletedType.Failure:
                    Console.WriteLine("Update failed.");
                    break;
            }

            // 1. Check for updates
            var coreUpdaterInfo = await mgr.CheckForUpdatesAsync();

            // 2. Compare versions
            var version1 = new Version(Assembly.GetExecutingAssembly().GetName().Version.ToString());
            var version2 = new Version(coreUpdaterInfo.Version);

            if (version1 < version2)
            {
                Console.WriteLine("New version found.");

                // 3. Download and extract
                await mgr.PrepareForUpdatesAsync(Directory.GetCurrentDirectory());

                // 4. Start updater (The updater is your new version application)
                mgr.StartUpdater(ExecutionType.DotNetCore);

                // 5. Close application
                return;
            }
            else
            {
                Console.WriteLine("Not found.");
            }

            Console.WriteLine("Hello World!");
            Console.ReadKey();
            return;
        }
    }
}
