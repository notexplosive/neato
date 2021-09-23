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
            var item = this.list[0];
            this.list.RemoveAt(0);

            return item;
        }

        public int NextInt()
        {
            var item = this.list[0];
            this.list.RemoveAt(0);

            return int.Parse(item);
        }
    }

    public class CommandLineParser
    {
        private readonly Dictionary<string, Command> registeredCommands = new Dictionary<string, Command>();

        public bool Consume(string[] argArray)
        {
            var args = new TokenList(argArray);
            var command = args.NextString();
            if (registeredCommands.ContainsKey(command))
            {
                registeredCommands[command].Execute(args);
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
