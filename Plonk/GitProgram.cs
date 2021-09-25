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
            RunWithArgsAt(this.workingDirectory, "init");
        }

        public string Log()
        {
            return RunWithArgsAt(this.workingDirectory, "log").stdOutput.Trim();
        }

        public ProgramOutput Version()
        {
            return RunWithArgs("--version");
        }
    }
}
