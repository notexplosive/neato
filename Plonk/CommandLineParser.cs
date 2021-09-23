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

        public bool Consume(string[] argArray, out string error)
        {
            var args = new TokenList(argArray);
            var command = args.NextString();
            var succeeded = false;
            if (registeredCommands.ContainsKey(command))
            {
                registeredCommands[command].Execute(args);
            }

            error = string.Empty;

            if (args.HasFailure())
            {
                error = args.Error();
            }

            return succeeded;
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

        public void OnExecuted(Action<TokenList> action)
        {
            this.executed = action;
        }

        private Action<TokenList> executed;

        public void Execute(TokenList args)
        {
            try
            {
                executed?.Invoke(args);
            }
            catch (TokenizerFailedException)
            {
                Console.Error.WriteLine(args.Error());
            }
        }
    }
}
