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

        private string CalculatePath(PathContext context)
        {
            if (context.pathType == PathType.Relative)
            {
                return Path.Join(WorkingDirectory, context.path);
            }
            else
            {
                return context.path;
            }
        }

        public void MakeDirectory(PathType pathType, string path)
        {
            var context = new PathContext(pathType, path);
            Directory.CreateDirectory(CalculatePath(context));
        }

        public void RemoveDirectory(PathType pathType, string path)
        {
            var context = new PathContext(pathType, path);
            Directory.Delete(CalculatePath(context));
        }

        public void RemoveDirectoryRecursive(PathType pathType, string path)
        {
            var context = new PathContext(pathType, path);
            Directory.Delete(CalculatePath(context), true);
        }

        public void CreateFile(PathType pathType, string path)
        {
            var context = new PathContext(pathType, path);
            var file = File.Create(CalculatePath(context));
            file.Close();
        }

        public void RemoveFiles(PathType pathType, string path, string pattern)
        {
            var context = new PathContext(pathType, path);
            var files = Directory.EnumerateFiles(CalculatePath(context), pattern);
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

        public void WriteToFile(PathType pathType, string path, string content)
        {
            var context = new PathContext(pathType, path);
            var realPath = CalculatePath(context);
            File.WriteAllText(realPath, content);
        }

        public void Copy(PathType sourcePathType, string sourcePath, PathType destinationPathType, string destinationPath)
        {
            var sourceContext = new PathContext(sourcePathType, sourcePath);
            var destinationContext = new PathContext(destinationPathType, destinationPath);
            File.Copy(CalculatePath(sourceContext), CalculatePath(destinationContext));
        }
    }
}
