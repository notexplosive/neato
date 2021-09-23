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

        public void MakeDirectory(PathContext context)
        {
            Directory.CreateDirectory(context.CalculatePath(WorkingDirectory));
        }

        public void RemoveDirectory(PathContext context)
        {
            Directory.Delete(context.CalculatePath(WorkingDirectory));
        }

        public void RemoveDirectoryRecursive(PathContext context)
        {
            Directory.Delete(context.CalculatePath(WorkingDirectory), true);
        }

        public void CreateFile(PathContext context)
        {
            var file = File.Create(context.CalculatePath(WorkingDirectory));
            file.Close();
        }

        public void RemoveFiles(PathContext context, string pattern)
        {
            var files = Directory.EnumerateFiles(context.CalculatePath(WorkingDirectory), pattern);
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

        public void WriteToFile(PathContext context, string content)
        {
            var realPath = context.CalculatePath(WorkingDirectory);
            File.WriteAllText(realPath, content);
        }

        public void Copy(PathContext source, PathContext destination)
        {
            File.Copy(source.CalculatePath(WorkingDirectory), destination.CalculatePath(WorkingDirectory));
        }
    }
}
