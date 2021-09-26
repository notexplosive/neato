﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neato
{
    public class GitProgram : ExternalProgram
    {
        private readonly string workingDirectory;

        public GitProgram(string workingDirectory) : base("git")
        {
            this.workingDirectory = workingDirectory;
        }

        public void Init()
        {
            RunWithArgsAt(this.workingDirectory, OutputLevel.Suppress, "init");
        }

        public ProgramOutput Version()
        {
            return RunWithArgs(OutputLevel.Allow, "--version");
        }

        public bool Exists()
        {
            return RunWithArgs(OutputLevel.Suppress, "--version").wasSuccessful;
        }
    }
}
