using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Xunit;

namespace CoreUpdater.Tests
{
    public class CoreUpdaterInfoTest
    {
        [Fact]
        public void ReadStringTest()
        {
            var str = "{  \"Name\": \"MakePdf\",  \"Version\": \"1.0.0\",  \"Files\": [    {      \"Name\": \"CoreUpdaterInfo.json\",      \"Hash\": \"79dc6187bd3081ecec2282469b21fbacbf9522a146cd26d81411eb3e5588d987\"    },    {      \"Name\": \"MakePdf.Core.dll\",      \"Hash\": \"c023099221415a38f62a5ed1ecd1a42e5dc88d6b240fb60e97da97ac4b5db3a4\"    }  ]}";
            var appInto = CoreUpdaterInfo.ReadString(str);
            Assert.Equal("MakePdf", appInto.Name);
            Assert.Equal("1.0.0", appInto.Version);
        }

        [Fact]
        public void ReadFileTest()
        {
            var appInto = CoreUpdaterInfo.ReadFile(@"TestData\CoreUpdaterInfoTest.json");
            Assert.Equal("MakePdf", appInto.Name);
            Assert.Equal("1.0.0", appInto.Version);
        }
    }
}
