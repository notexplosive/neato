using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neato
{
    public class ButlerProgram : ExternalProgram
    {
        public ButlerProgram() : base("butler")
        {
        }

        public void Logout()
        {
            RunWithArgs("logout", "--assume-yes");
        }

        public void Login()
        {
            RunWithArgs("login");
        }

        public ProgramOutput Version()
        {
            return RunWithArgs("--version");
        }

        public ProgramOutput Push(string directoryToUpload, string itchUrl, string gameUrl, string channel)
        {
            return RunWithArgs("push", directoryToUpload, $"{itchUrl}/{gameUrl}:{channel}");
        }
    }
}
