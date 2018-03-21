using System;
using System.Collections.Generic;
using System.Text;

using Xunit;

using CoreUpdater.Common;

namespace CoreUpdater.Tests.Common
{
    public class PathTest
    {
        [Theory,
            InlineData(@"..\file.txt", @"C:\Windows\System\", @"C:\Windows\file.txt"),
            InlineData(@"file.txt", @"C:\Windows\System", @"C:\Windows\file.txt"),
            InlineData(@"System\file.txt", @"C:\Windows\", @"C:\Windows\System\file.txt")]
        public void GetRelativePathTest(string expected, string path1, string path2)
        {
            Assert.Equal(expected, Path.GetRelativePath(path1, path2));
        }
    }
}
