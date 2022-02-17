using System;
using System.Collections.Generic;
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

            // Get source path
            string sourcePath = GetSourcePath();
            if (string.IsNullOrEmpty(sourcePath))
            {
                Console.WriteLine("You must choose the source folder");
                Console.ReadKey();
                return;
            }

            var sourceFiles = Directory.GetFiles(sourcePath);
            var sourceDirectories = Directory.GetDirectories(sourcePath);
            Console.WriteLine(ShowDirectoryInfo("source", sourcePath, sourceDirectories, sourceFiles));

            // Get destination path
            string destinationPath = GetDestinationPath();
            if (string.IsNullOrEmpty(destinationPath))
            {
                Console.WriteLine("You must choose the destination folder");
                Console.ReadKey();
                return;
            }
            var destFiles = Directory.GetFiles(destinationPath);
            var destDirs = Directory.GetDirectories(destinationPath);
            var destDirsByNames = destDirs.ToDictionary(d => Path.GetFileName(d));
            Console.WriteLine(ShowDirectoryInfo("destination", destinationPath, destDirs, destFiles));

            // Check
            Console.WriteLine("If everything is ok, tap \"y\" here, otherwise tap any key.");

            var choice = Console.ReadKey();
            if (choice.Key != ConsoleKey.Y)
            {
                return;
            }
            Console.WriteLine();

            // Do action
            MoveFiles(sourceFiles, destDirsByNames, destinationPath, (mes) => Console.WriteLine(mes));

            // End
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

        private static void MoveFiles(string[] sourceFiles, Dictionary<string, string> destDirsByNames, string destinationPath, Action<string> showMessage)
        {
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
                        if (destDirsByNames.TryGetValue(purposeDir, out string dDir))
                        {
                            dFile = Path.Combine(dDir, fName);
                        }
                        else
                        {
                            Directory.CreateDirectory(Path.Combine(destinationPath, "img"));

                            dFile = Path.Combine(destinationPath, "img", fName);
                        }

                        showMessage?.Invoke($"Moving {sFile} to {dFile}");
                        File.Move(sFile, dFile);
                    }
                    else
                    {
                        showMessage?.Invoke($"{sFile} couldn't parse file name");
                    }
                }
                catch (Exception ex)
                {
                    showMessage?.Invoke(ex.Message);
                }
            }
        }
    }
}