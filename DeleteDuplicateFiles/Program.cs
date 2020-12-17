using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace DeleteDuplicateFiles
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @".\";
            string[] files = Directory.GetFiles(path);
            List<string> sha1List = new List<string>();
            foreach (var file in files)
            {
                string hash = checkSHA1(file);
                if (!sha1List.Contains(hash))
                {
                    sha1List.Add(hash);
                }
                else
                {
                    File.Delete(file);
                    Console.WriteLine($"Deleted {file}");
                }
            }
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }
        public static string checkSHA1(string file)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                using (FileStream stream = File.OpenRead(file))
                {
                    Byte[] hash = sha1.ComputeHash(stream);
                    StringBuilder sBuilder = new StringBuilder();
                    for (int i = 0; i < hash.Length; i++)
                    {
                        sBuilder.Append(hash[i].ToString("x2"));
                    }
                    return sBuilder.ToString();
                }
            }
        }
    }
}
