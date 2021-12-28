using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neato
{
    public class GitProgram : ExternalProgram
    {
        private readonly string workingDirectory;

        public GitProgram(string workingDirectory, Logger logger) : base("git", logger)
        {
            this.workingDirectory = workingDirectory;
        }

        public void Init(ProgramOutputLevel outputLevel = ProgramOutputLevel.SuppressProgramFromEmittingToConsole)
        {
            RunWithArgsAt(this.workingDirectory, outputLevel, "init");
        }

        public ProgramOutput Version()
        {
            return RunWithArgs(ProgramOutputLevel.AllowProgramToEmitToConsole, "--version");
        }

        public bool Exists()
        {
            return RunWithArgs(ProgramOutputLevel.SuppressProgramFromEmittingToConsole, "--version").wasSuccessful;
        }

        public void AddAll(ProgramOutputLevel outputLevel)
        {
            RunWithArgs(outputLevel, "add", ".");
        }

        public void CommitWithMessage(ProgramOutputLevel outputLevel, string commitMessage)
        {
            RunWithArgs(outputLevel, "commit", "-m", commitMessage);
        }

        public void AddSubmodule(ProgramOutputLevel outputLevel, string url)
        {
            RunWithArgs(outputLevel, "submodule", "add", url);
        }
    }
}
