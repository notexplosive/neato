using System;
using System.Collections.Generic;
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
    }
}
