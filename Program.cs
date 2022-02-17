using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace FileMover
{
    internal static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Console.WriteLine("Hi Bokas!! " + Environment.NewLine +
                              "First you need to choose the source directory, then chose the destination one" + Environment.NewLine);

            string sourcePath = GetSourcePath();
            if (string.IsNullOrEmpty(sourcePath))
            {
                Console.WriteLine("You must choose the source folder");
            }

            var sourceFiles = Directory.GetFiles(sourcePath);
            var sourceDirectories = Directory.GetDirectories(sourcePath);
            Console.WriteLine(ShowDirectoryInfo("source", sourcePath, sourceDirectories, sourceFiles));

            string destinationPath = GetDestinationPath();
            if (string.IsNullOrEmpty(destinationPath))
            {
                Console.WriteLine("You must choose the destination folder");
            }
            var destFiles = Directory.GetFiles(destinationPath);
            var destDirs = Directory.GetDirectories(destinationPath);
            var destDirsNames = destDirs.ToDictionary(d => Path.GetFileName(d));
            Console.WriteLine(ShowDirectoryInfo("destination", destinationPath, destDirs, destFiles));

            Console.WriteLine("If everything is ok, tap \"y\" here, otherwise tap any key.");

            var choice = Console.ReadKey();
            if (choice.Key != ConsoleKey.Y)
            {
                return;
            }
            Console.WriteLine();

            Regex reg = new Regex(@"^(.*?)(?=_)");

            foreach (var sFile in sourceFiles)
            {
                try
                {
                    string fName = Path.GetFileName(sFile);
                    var match = reg.Match(fName);

                    if (match.Success)
                    {
                        string dFile;

                        var purposeDir = match.Value;
                        if (destDirsNames.TryGetValue(purposeDir, out string dDir))
                        {
                            dFile = Path.Combine(dDir, fName);
                        }
                        else
                        {
                            Directory.CreateDirectory(Path.Combine(destinationPath, "img"));

                            dFile = Path.Combine(destinationPath, "img", fName);
                        }

                        Console.WriteLine($"Moving {sFile} to {dFile}", dFile);
                        File.Move(sFile, dFile);
                    }
                    else
                    {
                        Console.WriteLine($"{sFile} couldn't parse file name");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            Console.WriteLine();
            Console.WriteLine("All done! Enjoy :)");

            Console.ReadKey();
        }

        private static string GetSourcePath()
        {
            using (var fbd = new FolderBrowserDialog { Description = "Choose the source folder" })
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    return fbd.SelectedPath;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        private static string GetDestinationPath()
        {
            using (var fbd = new FolderBrowserDialog { Description = "Choose the destination folder" })
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    return fbd.SelectedPath;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        private static string ShowDirectoryInfo(string direction, string path, string[] directories, string[] files) =>
            $"The {direction} path is: {path}" + Environment.NewLine +
            $"There are {files.Length} files in the {direction} directory" + Environment.NewLine +
            $"There are {directories.Length} subdirectories in the {direction} directory" + Environment.NewLine;
    }
}