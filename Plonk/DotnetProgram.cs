using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neato
{
    public class DotnetProgram : ExternalProgram
    {
        public DotnetProgram() : base("dotnet")
        {
        }

        public ProgramOutput PublishExe_Special(string csprojPath, string outputDirectory)
        {
            return RunWithArgs(
                        "publish",
                        csprojPath,
                        "-c", "Release",
                        "-r", "win-x64",
                        "/p:PublishReadyToRun=false",
                        "/p:TieredCompilation=false",
                        "/p:IncludeNativeLibrariesForSelfExtract=true",
                        "--self-contained",
                        "--output", outputDirectory);
        }

        /// <summary>
        /// This is the normal publish we used to run from release_build.bat
        /// </summary>
        /// <param name="csprojPath"></param>
        /// <param name="outputDirectory"></param>
        /// <returns></returns>
        public ProgramOutput NormalPublish(string csprojPath, FileManager outputFile)
        {
            return RunWithArgs(
                        "publish",
                        csprojPath,
                        "-c", "Release",
                        "-r", "win-x64",
                        "/p:PublishReadyToRun=false",
                        "/p:TieredCompilation=false",
                        "--self-contained",
                        "--output", outputFile.WorkingDirectory);
        }

        public ProgramOutput Version()
        {
            return RunWithArgs("--version");
        }
    }
}
