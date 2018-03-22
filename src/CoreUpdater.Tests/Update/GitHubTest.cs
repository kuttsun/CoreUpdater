using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

using Xunit;

namespace CoreUpdater.Updates.Tests
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
            var appInfo = mgr.CheckForUpdateAsync().Result;
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
            mgr.Update(null, fixture.UpdateSrcDir, fixture.UpdateDstDir);
        }

        [Fact(DisplayName ="Update TimeoutException")]
        public void UpdateTest_InvalidPid()
        {
            // Get own process ID 
            var pid = Process.GetCurrentProcess().Id;
            Assert.Throws<TimeoutException>(() => mgr.Update(pid.ToString(), fixture.UpdateSrcDir, fixture.UpdateDstDir));
        }
    }
}
