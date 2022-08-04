using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SvnSolution
{
    class Program
    {
        static string projectName = "";
        static string rootPath = @"C:\Users\BRWANG\projects\PTM\PTDW\src\PTDW";
        static string packageFolderPath = rootPath + @"\packages";
        static Dictionary<string, string> dicPackageVersion = new Dictionary<string, string>();
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");

            rootPath = args[0];

            //get packages from packages folder
            var packages = GetPackages(packageFolderPath);
            
            //get packages from config file
            var projects = GetProjects(rootPath);
            int count = 0;

            foreach (var package in dicPackageVersion.Keys)
            {
                if (!packages.Contains(package + "." + dicPackageVersion[package]))
                {
                    Console.WriteLine("Package: " + package + " Version:" + dicPackageVersion[package] + " is missing...");
                    Console.WriteLine("Installing " + package + " of version " + dicPackageVersion[package]);
                    InstallPackage(package, dicPackageVersion[package]);
                    count++;
                }
            }
            Console.WriteLine(count + " package(s) missing.");
            Console.ReadLine();
        }

        static List<string> GetPackages(string path)
        {
            List<string> result = new List<string>();




            DirectoryInfo directory = new DirectoryInfo(path);
            DirectoryInfo[] directories = directory.GetDirectories();

            foreach (DirectoryInfo folder in directories)
              result.Add(folder.Name);

            return result;
            
        }

        static List<string> GetProjects(string path)
        {
            List<string> result = new List<string>();




            DirectoryInfo directory = new DirectoryInfo(path);
            DirectoryInfo[] directories = directory.GetDirectories();
            projectName = rootPath.Substring(rootPath.LastIndexOf("\\") + 1);
            foreach (DirectoryInfo folder in directories)
            {
                if (folder.Name.Contains(projectName))
                {
                    result.Add(folder.Name);

                    AnalyzeConfig(rootPath + "\\" +  folder.Name);
                }
                
            }
                

            return result;

        }

        static Dictionary<string, string> AnalyzeConfig(string path)
        {
            
            List<string> result = new List<string>();

            DirectoryInfo d = new DirectoryInfo(path); //Assuming Test is your Folder

            FileInfo[] Files = d.GetFiles("packages.config"); //Getting Text files
            string str = "";

            foreach (FileInfo file in Files)
            {
                str = str + ", " + file.Name;
                StringBuilder result_test = new StringBuilder();

                

                //Load xml
                XDocument xdoc = XDocument.Load(file.FullName);

                //Run query
                var pcs = from pc in xdoc.Descendants("package")
                           select new
                           {
                               id = pc.Attribute("id").Value,
                               version = pc.Attribute("version").Value
                           };

                //Loop through results
                foreach (var pc1 in pcs)
                {
                    result_test.AppendLine(pc1.id);
                    if (!dicPackageVersion.Keys.Contains(pc1.id))
                    {
                        dicPackageVersion.Add(pc1.id, pc1.version);
                    }
                    
                   
                }
                
                
            }


            return dicPackageVersion;

        }

        static void InstallPackage(string package, string version)
        {
            //c:\software\nuget install Autofac -Version 4.6.2 -OutputDirectory C:\Users\BRWANG\projects\PTM\PTDW\src\PTDW\packages

            //string command = @"c:\software\nuget.exe ";
            string args = null;
            args += "install ";
            args += package;
            args += " -Version ";
            args += version;
            args += " -OutputDirectory ";
            args += rootPath;
            args += @"\packages";

            string nuget = @"c:\software\nuget.exe";
            

            

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = nuget;
            startInfo.Arguments = args;
            Process.Start(startInfo);

        }
    }
}
