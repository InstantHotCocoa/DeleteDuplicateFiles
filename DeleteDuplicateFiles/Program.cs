using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Configuration;

namespace DeleteDuplicateFiles
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string path = @".\";
            FileInfo[] files = new DirectoryInfo(path).GetFiles().OrderBy(f => f.LastWriteTime).ToArray();
            int current = 1;
            int total = files.Length;
            bool ExportSHA1List = Convert.ToBoolean(ConfigurationManager.AppSettings["ExportSHA1List"]);
            bool UseSHA1List = Convert.ToBoolean(ConfigurationManager.AppSettings["UseSHA1List"]);
            List<string> SHA1List = new List<string>();

            if (UseSHA1List)
            {
                if (File.Exists("SHA1List.txt"))
                {
                    LoadSHA1List(SHA1List);
                }
                else
                {
                    Console.WriteLine("Cannot find SHA1List.txt. Program terminated.\nPress any key to continue");
                    Console.ReadKey();
                    return;
                }
            }
            else
            {
                if (File.Exists("SHA1List.txt"))
                {
                    File.Delete("SHA1List.txt");
                }
            }

            foreach (FileInfo file in files)
            {
                string hash = CheckSHA1(file.FullName);
                if (ExportSHA1List)
                {
                    WriteSHA1List(hash);
                }

                if (SHA1List.Contains(hash))
                {
                    string destPath = path + @"Deleted Files\";
                    if (!Directory.Exists(destPath))
                    {
                        Directory.CreateDirectory(destPath);
                    }
                    try
                    {
                        File.Move(file.FullName, destPath + file);
                        Console.WriteLine($"Deleted {file}");
                    }
                    catch (IOException)
                    {
                        Console.WriteLine($"Can't delete {file} due to a same filename exists");
                    }
                }
                else
                {
                    SHA1List.Add(hash);
                }

                Console.Write($"{current}/{total}\r");
                ++current;
            }

            Console.WriteLine("Finished. Press any key to continue");
            Console.ReadKey();
        }

        public static void LoadSHA1List(List<string> list)
        {
            using (StreamReader sr = new StreamReader("SHA1List.txt"))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    list.Add(line);
                }
            }
        }

        public static void WriteSHA1List(string hash)
        {
            using (StreamWriter sw = new StreamWriter("SHA1List.txt", true))
            {
                sw.WriteLine(hash);
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
