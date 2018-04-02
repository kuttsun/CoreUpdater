using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;

using Newtonsoft.Json;

using CoreUpdater.Common;

namespace CoreUpdater
{
    public class CoreUpdaterInfo
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public List<FileInfo> Files { get; set; } = new List<FileInfo>();

        public void AddFileInfo(string targetDir, string[] files)
        {
            foreach (var file in files)
            {
                using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var name = Common.Path.GetRelativePath($@"{targetDir}\", file);
                    var hash = Hash.GetHash<SHA256CryptoServiceProvider>(fs);
                    Files.Add(new FileInfo
                    {
                        Name = name,
                        Hash = hash
                    });
                    Console.WriteLine(name);
                    Console.WriteLine(hash);
                }
            }
        }

        public void WriteFile(string targetDir, string fileName)
        {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);

            using (FileStream fs = new FileStream($@"{targetDir}\{fileName}", FileMode.Create, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.Write(json);
            }
        }

        public static CoreUpdaterInfo ReadFile(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            using (StreamReader sr = new StreamReader(fs))
            {
                return JsonConvert.DeserializeObject<CoreUpdaterInfo>(sr.ReadToEnd());
            }
        }

        public static CoreUpdaterInfo ReadString(string str)
        {
            return JsonConvert.DeserializeObject<CoreUpdaterInfo>(str);
        }

        public string GetNewVersionDir()
        {
            return $"{Name}-{Version}";
        }
    }
}
