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
        private readonly Dictionary<string, Command> registeredCommands = new Dictionary<string, Command>();

        public bool Consume(string[] fullArgs)
        {
            var command = fullArgs[0];
            if (registeredCommands.ContainsKey(fullArgs[0]))
            {
                registeredCommands[fullArgs[0]].Execute();
                return true;
            }
            else
            {
                return false;
            }
        }

        public Command RegisterCommand(string commandName)
        {
            var command = new Command();
            this.registeredCommands.Add(commandName, command);
            return command;
        }
    }

    public class Command
    {
        public Command()
        {
        }

        public event Action Executed;

        public void Execute()
        {
            Executed?.Invoke();
        }
    }
}
