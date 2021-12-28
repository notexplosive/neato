using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neato
{
    public class ButlerProgram : ExternalProgram
    {
        public ButlerProgram(Logger logger) : base("butler", logger)
        {
        }

        public void Logout()
        {
            RunWithArgs(ProgramOutputLevel.AllowProgramToEmitToConsole, "logout", "--assume-yes");
        }

        public void Login()
        {
            RunWithArgs(ProgramOutputLevel.AllowProgramToEmitToConsole, "login");
        }

        public ProgramOutput Version()
        {
            return RunWithArgs(ProgramOutputLevel.AllowProgramToEmitToConsole, "--version");
        }

        public ProgramOutput Push(string directoryToUpload, string itchUrl, string gameUrl, string channel)
        {
            return RunWithArgs(ProgramOutputLevel.AllowProgramToEmitToConsole, "push", directoryToUpload, $"{itchUrl}/{gameUrl}:{channel}");
        }

        public bool Exists()
        {
            return RunWithArgs(ProgramOutputLevel.SuppressProgramFromEmittingToConsole, "--version").wasSuccessful;
        }
    }
}
