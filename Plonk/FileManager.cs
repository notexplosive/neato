using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plonk
{
    public enum PathType
    {
        Absolute,
        Relative
    }

    public class FileManager
    {
        public FileManager(PathType pathType)
        {
            if (pathType == PathType.Relative)
            {
                WorkingDirectory = Directory.GetCurrentDirectory();
            }
            else
            {
                WorkingDirectory = Directory.GetDirectoryRoot(Directory.GetCurrentDirectory());
            }
        }

        public FileManager(PathType pathType, string workingDirectory)
        {
            if (pathType == PathType.Relative)
            {
                WorkingDirectory = Path.Join(Directory.GetCurrentDirectory(), workingDirectory);
            }
            else
            {
                WorkingDirectory = workingDirectory;
            }
        }

        public string WorkingDirectory { get; set; }

        private string CalculatePath(PathType pathType, string path)
        {
            if (pathType == PathType.Relative)
            {
                return Path.Join(WorkingDirectory, path);
            }
            else
            {
                return path;
            }
        }

        private string CalculatePath(PathType pathType)
        {
            if (pathType == PathType.Relative)
            {
                return WorkingDirectory;
            }
            else
            {
                return Directory.GetDirectoryRoot(WorkingDirectory);
            }
        }

        public void MakeDirectory(PathType pathType, string path)
        {
            Directory.CreateDirectory(CalculatePath(pathType, path));
        }

        public void RemoveDirectory(PathType pathType, string path)
        {
            Directory.Delete(CalculatePath(pathType, path));
        }

        public void RemoveDirectoryRecursive(PathType pathType, string path)
        {
            Directory.Delete(CalculatePath(pathType, path), true);
        }

        public void CreateFile(PathType pathType, string path)
        {
            var file = File.Create(CalculatePath(pathType, path));
            file.Close();
        }

        public void RemoveFiles(PathType pathType, string pattern)
        {
            var files = Directory.EnumerateFiles(CalculatePath(pathType), pattern);
            var failed = true;

            while (failed)
            {
                failed = false;
                foreach (var file in files)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch
                    {
                        failed = true;
                    }
                }
            }
        }
    }
}
