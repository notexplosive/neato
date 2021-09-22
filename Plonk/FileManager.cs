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

        private string RelativePath(string relativePath)
        {
            return Path.Join(WorkingDirectory, relativePath);
        }

        public void MakeDirectory(PathType pathType, string path)
        {
            if (pathType == PathType.Relative)
            {
                Directory.CreateDirectory(RelativePath(path));
            }
            else
            {
                Directory.CreateDirectory(path);
            }
        }

        public void RemoveDirectory(PathType pathType, string path)
        {
            if (pathType == PathType.Relative)
            {
                Directory.Delete(RelativePath(path));
            }
            else
            {
                Directory.Delete(path);
            }
        }
    }
}
