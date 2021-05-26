using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace DL444.AzureBlobMd5
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            List<string> files = new List<string>(args.Length);
            foreach (string path in args)
            {
                if (Directory.Exists(path))
                {
                    files.AddRange(Directory.GetFiles(path));
                }
                else
                {
                    files.Add(path);
                }
            }
            (string, Task<string>)[] hashTasks = files.Select(x => (x, GetMd5Base64(x))).ToArray();
            foreach ((string path, Task<string> task) in hashTasks)
            {
                try
                {
                    string hash = await task;
                    Console.WriteLine($"{path}\t{hash}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{path}\tFailed: {ex.GetType()}");
                }
            }
        }

        private static async Task<string> GetMd5Base64(string path)
        {
            using FileStream file = File.OpenRead(path);
            MD5 md5 = MD5.Create();
            byte[] hash = await md5.ComputeHashAsync(file);
            return Convert.ToBase64String(hash);
        }
    }
}
