using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace CoreUpdater.Common
{
    public static class Hash
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static string GetHash<T>(string text) where T : HashAlgorithm, new()
        {
            // Convert a string to a byte array
            byte[] data = Encoding.UTF8.GetBytes(text);

            return ComputeHash<T>(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static string GetHash<T>(Stream stream) where T : HashAlgorithm, new()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ComputeHash<T>(ms.ToArray());
            }
        }

        static string ComputeHash<T>(byte[] bytes) where T : HashAlgorithm, new()
        {
            var algorithm = new T();

            byte[] bs = algorithm.ComputeHash(bytes);

            algorithm.Clear();

            // Convert byte array to hexadecimal character string
            var result = new StringBuilder();
            foreach (var b in bs)
            {
                result.Append(b.ToString("x2"));
            }
            return result.ToString();
        }
    }
}
