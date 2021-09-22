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
    }
}
