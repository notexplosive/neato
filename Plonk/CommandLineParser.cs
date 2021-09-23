using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neato
{
    public class TokenList
    {
        private readonly List<string> list;
        private int currentPosition = 0;
        private string error = string.Empty;
        private bool hasError;

        public string Error()
        {
            return this.error;
        }

        public void SaveError(string text)
        {
            if (this.error.Length > 0)
            {
                this.error += "\n";
            }
            this.error += text;
            this.hasError = true;
        }

        public TokenList(string[] args)
        {
            this.list = new List<string>();
            foreach (var arg in args)
            {
                this.list.Add(arg);
            }
        }

        public string NextString()
        {
            if (this.list.Count > 0)
            {
                var item = this.list[0];
                this.list.RemoveAt(0);
                this.currentPosition++;

                return item;
            }
            else
            {
                SaveError(string.Format("Missing value at position {0}", this.currentPosition));
                return "";
            }
        }

        public int NextInt()
        {
            var token = NextString();
            var canParse = int.TryParse(token, out int result);

            if (canParse)
            {
                return result;
            }
            else
            {
                SaveError(string.Format("Expected integer at position {0}, got {1}", this.currentPosition, token.Length > 0 ? token : "nothing"));
                return -1;
            }
        }

        public bool HasFailure()
        {
            return this.hasError;
        }
    }

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
            executed?.Invoke(args);
        }
    }
}
