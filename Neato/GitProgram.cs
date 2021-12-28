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

        public void Init(OutputLevel outputLevel = OutputLevel.Suppress)
        {
            RunWithArgsAt(this.workingDirectory, outputLevel, "init");
        }

        public ProgramOutput Version()
        {
            return RunWithArgs(OutputLevel.Allow, "--version");
        }

        public bool Exists()
        {
            return RunWithArgs(OutputLevel.Suppress, "--version").wasSuccessful;
        }

        public void AddAll(OutputLevel outputLevel)
        {
            RunWithArgs(outputLevel, "add", ".");
        }

        public void CommitWithMessage(OutputLevel outputLevel, string commitMessage)
        {
            RunWithArgs(outputLevel, "commit", "-m", commitMessage);
        }

        public void AddSubmodule(OutputLevel outputLevel, string url)
        {
            RunWithArgs(outputLevel, "submodule", "add", url);
        }
    }
}
