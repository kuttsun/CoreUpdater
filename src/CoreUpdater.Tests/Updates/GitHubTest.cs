using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

using Xunit;

namespace CoreUpdater.Tests.Updates
{
    public class GitHubFixture : IDisposable
    {
        public string UpdateSrcDir { get; } = @"TestData\UpdateSrc";
        public string UpdateDstDir { get; } = @"TestData\UpdateDst";

        // Setup
        public GitHubFixture()
        {
        }

        // Teardown
        public void Dispose()
        {
            Directory.Delete(UpdateSrcDir, true);
            Directory.Delete(UpdateDstDir, true);
        }
    }

    public class GitHubTest : IClassFixture<GitHubFixture>
    {
        GitHubFixture fixture;
        GitHub mgr;

        public GitHubTest(GitHubFixture fixture)
        {
            this.fixture = fixture;
            mgr = new GitHub("https://github.com/kuttsun/Test");
        }

        [Fact]
        public void CheckForUpdateTest()
        {
            var appInfo = mgr.CheckForUpdatesAsync().Result;
            Assert.NotNull(appInfo);
        }

        //[Fact]
        //public void PrepareForUpdateTest()
        //{
        //    var appInfo = mgr.PrepareForUpdate("test.zip").Result;
        //    Assert.NotNull(appInfo);
        //}

        [Fact]
        public void UpdateTest()
        {
            mgr.Update(new string[] { "CoreUpdaterStarting", "--pid", null, "-n", "name", "-s", fixture.UpdateSrcDir, "-d", fixture.UpdateDstDir });
        }

        [Fact]
        public void UpdateTest_TimeoutException()
        {
            // Get own process ID 
            var pid = Process.GetCurrentProcess().Id;
            Assert.Throws<TimeoutException>(() => mgr.Update(new string[] { "CoreUpdaterStarting", "--pid", pid.ToString(), "-n", "name", "-s", fixture.UpdateSrcDir, "-d", fixture.UpdateDstDir }));
        }
    }
}
