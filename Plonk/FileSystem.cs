using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plonk
{
    public class FileSystem
    {
        public FileSystem(string workingDirectory = "")
        {
            WorkingDirectory = workingDirectory;
        }

        public string WorkingDirectory { get; set; }
    }
}
