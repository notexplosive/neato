using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neato
{
    public class SevenZipProgram : ExternalProgram
    {
        public SevenZipProgram() : base("7z")
        {
        }

        public ProgramOutput SendToZip(string buildOutputDirectory, string outputDirectory, string zipName)
        {
            return RunWithArgs("a", "-r", Path.Join(outputDirectory, $"{zipName}.zip"), Path.Join(buildOutputDirectory, "*"));
        }
    }
}
