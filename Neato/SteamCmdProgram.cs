using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neato
{
    public class SteamCmdProgram : ExternalProgram
    {
        public SteamCmdProgram(Logger logger) : base("steamcmd", logger)
        {
        }

        public ProgramOutput RunQuit()
        {
            return RunWithArgs(OutputLevel.Allow, "+quit");
        }

        public ProgramOutput Login(string steamUsername)
        {
            return RunWithArgs(OutputLevel.Allow, "+login", $"{steamUsername}", "+quit");
        }

        public ProgramOutput Deploy(string steamUsername, string fullPathToVDF)
        {
            return RunWithArgs(OutputLevel.Allow, "+login", $"{steamUsername}", "+run_app_build", fullPathToVDF, "+quit");
        }

        public bool Exists()
        {
            return RunWithArgs(OutputLevel.Suppress, "+quit").wasSuccessful;
        }
    }
}
