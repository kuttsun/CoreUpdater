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

namespace CoreUpdater
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
        public GitHub(string gitHubRepository, string coreUpdaterInfoFileName = null, ILogger logger = null) : base(coreUpdaterInfoFileName, logger)
        {
            this.gitHubRepository = gitHubRepository;
        }

        public override async Task<CoreUpdaterInfo> CheckForUpdatesAsync()
        {
            var tag = await GetLatestReleaseTagAsync();

            var jsonUrl = GetAssetUrl(tag, CoreUpdaterInfoFileName);

            return await CheckForUpdatesAsync(jsonUrl);
        }

        async Task<CoreUpdaterInfo> CheckForUpdatesAsync(string jsonUrl)
        {
            var appInfo = await DownloadJsonAsync(jsonUrl);

            // Deserialize
            return CoreUpdaterInfo.ReadString(appInfo);
        }

        public override async Task<CoreUpdaterInfo> PrepareForUpdatesAsync(string outputDir)
        {
            return await PrepareForUpdatesAsync(outputDir, AppName + ".zip");
        }

        /// <summary>
        /// Download zip from GitHub, and extract it
        /// </summary>
        /// <param name="zipFileName"></param>
        /// <param name="outputDir"></param>
        /// <returns></returns>
        public override async Task<CoreUpdaterInfo> PrepareForUpdatesAsync(string outputDir, string zipFileName)
        {
            var tag = await GetLatestReleaseTagAsync();
            var jsonUrl = GetAssetUrl(tag, CoreUpdaterInfoFileName);
            var zipUrl = GetAssetUrl(tag, zipFileName);

            var appInfo = await CheckForUpdatesAsync(jsonUrl);

            var outputPath = $@"{outputDir}\{zipFileName}";

            await DownloadZipAsync(zipUrl, outputPath);

            Zip.ExtractEntries(outputPath, appInfo.GetNewVersionDir());

            // Delete downloaded zip file.
            File.Delete(outputPath);

            return CoreUpdaterInfo.ReadFile($@"{appInfo.GetNewVersionDir()}\{CoreUpdaterInfoFileName}");
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
        /// 
        /// </summary>
        /// <returns>
        /// e.g. https://github.com/MyName/MyApp/releases/tag/1.0.0
        /// </returns>
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
        /// <returns>
        /// e.g. 1.0.0
        /// </returns>
        async Task<string> GetLatestReleaseTagAsync()
        {
            var latestReleaseUrl = await GetLatestReleaseUrlAsync();

            return latestReleaseUrl.Segments.Last();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="asset"></param>
        /// <returns>
        /// e.g. https://github.com/MyName/MyApp/releases/download/1.0.0/CoreUpdaterInfo.json
        /// </returns>
        string GetAssetUrl(string tag, string asset)
        {
            return $"{gitHubRepository}/releases/download/{tag}/{asset}";
        }
    }
}
