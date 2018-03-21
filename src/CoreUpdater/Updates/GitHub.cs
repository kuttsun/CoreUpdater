using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using CoreUpdater.Common;

namespace CoreUpdater.Updates
{
    public class GitHub : UpdateManager
    {
        HttpClient client = new HttpClient();

        // GitHub Repository (e.g. https://github.com/MyName/MyRepository)
        string gitHubRepository;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gitHubRepository">GitHub Repository (e.g. https://github.com/MyName/MyRepository)</param>
        /// <param name="logger"></param>
        public GitHub(string gitHubRepository, ILogger logger = null) : base(logger)
        {
            this.logger = logger;
            this.gitHubRepository = gitHubRepository;
        }

        override public async Task<AppInfo> CheckForUpdateAsync()
        {
            var tag = await GetLatestReleaseTagAsync();

            var jsonUrl = GetAssetUrl(tag, AppInfoFileName);

            return await CheckForUpdateAsync(jsonUrl);
        }

        async Task<AppInfo> CheckForUpdateAsync(string jsonUrl)
        {
            var appInfo = await DownloadJsonAsync(jsonUrl);

            // Deserialize
            return AppInfo.ReadString(appInfo);
        }

        override public async Task<AppInfo> PrepareForUpdate(string outputDir)
        {
            return await PrepareForUpdate(outputDir, AppName + ".zip");
        }

        /// <summary>
        /// Download zip from GitHub, and extract it
        /// </summary>
        /// <param name="zipFileName"></param>
        /// <param name="outputDir"></param>
        /// <returns></returns>
        override public async Task<AppInfo> PrepareForUpdate(string outputDir, string zipFileName)
        {
            var tag = await GetLatestReleaseTagAsync();
            var jsonUrl = GetAssetUrl(tag, AppInfoFileName);
            var zipUrl = GetAssetUrl(tag, zipFileName);

            var appInfo = await CheckForUpdateAsync(jsonUrl);

            var outputPath = $@"{outputDir}\{zipFileName}";

            await DownloadZipAsync(zipUrl, outputPath);

            Zip.ExtractEntries(outputPath, appInfo.GetNewVersionDir());

            // Delete downloaded zip file.
            File.Delete(outputPath);

            return AppInfo.ReadFile($@"{appInfo.GetNewVersionDir()}\{AppInfoFileName}");
        }

        async Task<string> DownloadJsonAsync(string url)
        {
            var response = await client.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }

        async Task<bool> DownloadZipAsync(string url, string outputPath)
        {
            var response = await client.GetAsync(url);

            using (FileStream fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                stream.CopyTo(fs);
                fs.Flush();
            }

            return true;
        }

        /// <summary>
        /// e.g. https://github.com/MyName/MyApp/releases/tag/1.0.0
        /// </summary>
        /// <returns></returns>
        async Task<Uri> GetLatestReleaseUrlAsync()
        {
            // This link simply redirects to the repositories latest release page,
            // and cannot be used to download an asset directly
            var response = await client.GetAsync(gitHubRepository + "/releases/latest");

            return response.RequestMessage.RequestUri;
        }

        /// <summary>
        /// e.g. https://github.com/MyName/MyApp/releases/tag/1.0.0 --> 1.0.0
        /// </summary>
        /// <returns></returns>
        async Task<string> GetLatestReleaseTagAsync()
        {
            var latestReleaseUrl = await GetLatestReleaseUrlAsync();

            return latestReleaseUrl.Segments.Last();
        }

        /// <summary>
        /// e.g. https://github.com/MyName/MyApp/releases/download/1.0.0/AppInfo.json
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="asset"></param>
        /// <returns></returns>
        string GetAssetUrl(string tag, string asset)
        {
            return $"{gitHubRepository}/releases/download/{tag}/{asset}";
        }
    }
}
