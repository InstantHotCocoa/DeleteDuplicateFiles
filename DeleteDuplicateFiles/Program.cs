using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Configuration;

namespace DeleteDuplicateFiles
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @".\";
            FileInfo[] files = new DirectoryInfo(path).GetFiles().OrderBy(f => f.LastWriteTime).ToArray();
            List<string> SHA1List = new List<string>();
            if (File.Exists("SHA1List.txt"))
            {
                LoadSHA1List(SHA1List);
            }
            foreach (var file in files)
            {
                string hash = CheckSHA1(file.FullName);
                if (!SHA1List.Contains(hash))
                {
                    SHA1List.Add(hash);
                }
                else
                {
                    string destPath = path + @"Deleted Files\";
                    if (!Directory.Exists(destPath))
                    {
                        Directory.CreateDirectory(destPath);
                    }
                    File.Move(file.FullName, destPath + file);
                    Console.WriteLine($"Deleted {file}");
                }
            }
            if (ConfigurationManager.AppSettings["ExportSHA1List"] == "true")
            {
                WriteSHA1List(SHA1List);
            }
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }
        public static void LoadSHA1List(List<string> list)
        {
            StreamReader sr = new StreamReader("SHA1List.txt");
            String line;
            while ((line = sr.ReadLine()) != null)
            {
                list.Add(line);
            }
        }
        public static void WriteSHA1List(List<string> list)
        {
            using (StreamWriter sw = new StreamWriter("SHA1List.txt"))
            {
                foreach (string hash in list)
                {
                    sw.WriteLine(hash);
                }
            }
        }
        public static string CheckSHA1(string file)
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
