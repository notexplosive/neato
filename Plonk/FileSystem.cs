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

    public class FileSystem
    {
        public FileSystem(PathType pathType, string workingDirectory)
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
