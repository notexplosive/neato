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
        public SteamCmdProgram() : base(Path.Join(AppDomain.CurrentDomain.BaseDirectory, "steam", "steamcmd"))
        {
        }

        public ProgramOutput RunQuit()
        {
            return RunWithArgs("+quit");
        }

        public ProgramOutput Login(string steamUsername)
        {
            return RunWithArgs($"+login {steamUsername}", "+quit");
        }

        public ProgramOutput Deploy(string fullPathToVDF)
        {
            return RunWithArgs("+run_app_build", fullPathToVDF, "+quit");
        }
    }
}
