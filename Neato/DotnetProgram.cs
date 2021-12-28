using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neato
{
    public class DotnetProgram : ExternalProgram
    {
        public DotnetProgram(Logger logger) : base("dotnet", logger)
        {
        }

        public ProgramOutput PublishExe_Special(string csprojPath, string outputDirectory)
        {
            return RunWithArgs(OutputLevel.Allow,
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
            return RunWithArgs(OutputLevel.Allow,
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
            return RunWithArgs(OutputLevel.Allow, "--version");
        }

        public bool Exists()
        {
            return RunWithArgs(OutputLevel.Suppress, "--version").wasSuccessful;
        }

        public void AddToSln(OutputLevel outputLevel, string path)
        {
            RunWithArgs(outputLevel, "sln", "add", path);
        }

        public void NewSln(OutputLevel outputLevel)
        {
            RunWithArgs(outputLevel, "new", "sln");
        }
    }
}
