using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neato
{
    public class CommandLineParser
    {
        private readonly Dictionary<string, Action> registeredCommands = new Dictionary<string, Action>();

        public bool Consume(string[] fullArgs)
        {
            var command = fullArgs[0];
            if (registeredCommands.ContainsKey(fullArgs[0]))
            {
                registeredCommands[fullArgs[0]]();
                return true;
            }
            else
            {
                return false;
            }
        }

        public void RegisterCommand(string commandName, Action behavior)
        {
            this.registeredCommands.Add(commandName, behavior);
        }
    }
}
