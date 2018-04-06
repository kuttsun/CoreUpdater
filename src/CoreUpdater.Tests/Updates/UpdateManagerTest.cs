using System;
using System.Collections.Generic;
using System.Text;

using Xunit;

namespace CoreUpdater.Tests.Updates
{
    public class UpdateManagerTest
    {
        [Fact]
        public void CanUpdateTest()
        {
            // Instance can be anything
            UpdateManager mgr = new GitHub("");
            Assert.True(mgr.CanUpdate(new string[] { "CoreUpdaterStarting", "--pid=1000", "-n=name", "-s=src", "-d=dst" }));
            Assert.True(mgr.CanUpdate(new string[] { "CoreUpdaterStarting", "--pid", "1000", "-n", "name", "-s", "src", "-d", "dst" }));
            Assert.False(mgr.CanUpdate(new string[] { "CoreUpdaterStarting" }));
            Assert.False(mgr.CanUpdate(new string[] { "foo", "--pid", "1000", "-n", "name", "-s", "src", "-d", "dst" }));
        }

        [Fact]
        public void CompletedTest()
        {
            // Instance can be anything
            UpdateManager mgr = new GitHub("");
            Assert.Equal(UpdateCompletedType.None, mgr.Completed(new string[] { "" }));
            Assert.Equal(UpdateCompletedType.Success, mgr.Completed(new string[] { "CoreUpdaterCompleted", "-r", "Success" }));
            Assert.Equal(UpdateCompletedType.Failure, mgr.Completed(new string[] { "CoreUpdaterCompleted", "-r", "Failure" }));
        }
    }
}
