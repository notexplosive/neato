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
            RunWithArgs(OutputLevel.Allow, "logout", "--assume-yes");
        }

        public void Login()
        {
            RunWithArgs(OutputLevel.Allow, "login");
        }

        public ProgramOutput Version()
        {
            return RunWithArgs(OutputLevel.Allow, "--version");
        }

        public ProgramOutput Push(string directoryToUpload, string itchUrl, string gameUrl, string channel)
        {
            return RunWithArgs(OutputLevel.Allow, "push", directoryToUpload, $"{itchUrl}/{gameUrl}:{channel}");
        }

        public bool Exists()
        {
            return RunWithArgs(OutputLevel.Suppress, "--version").wasSuccessful;
        }
    }
}
