using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neato
{
    public class CommandNotFoundException : Exception
    {
        public CommandNotFoundException(string message) : base(message)
        {

        }
    }

    public class CommandLineParser
    {
        private readonly Dictionary<string, Command> registeredCommands = new Dictionary<string, Command>();

        public bool Consume(string[] argArray)
        {
            var args = new TokenList(argArray);
            var commandName = args.NextString();
            if (registeredCommands.ContainsKey(commandName))
            {
                return registeredCommands[commandName].Execute(args);
            }
            else
            {
                throw new CommandNotFoundException(commandName);
            }
        }

        public Command RegisterCommand(string commandName)
        {
            var command = new Command(commandName);
            this.registeredCommands.Add(commandName, command);
            return command;
        }
    }

    public class Command
    {
        public Command(string commandName)
        {
            this.commandName = commandName;
        }

        public void OnExecuted(Action<List<Parameter>> action)
        {
            this.executed = action;
        }

        private Action<List<Parameter>> executed;
        public readonly string commandName;
        public readonly List<Parameter> parameters = new List<Parameter>();

        public bool Execute(TokenList args)
        {
            try
            {
                foreach (var parameter in parameters)
                {
                    parameter.ExtractValue(args);
                }
                executed?.Invoke(parameters);
            }
            catch (TokenizerFailedException e)
            {
                Console.Error.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        public Command AddParameter(Parameter parameter)
        {
            this.parameters.Add(parameter);
            return this;
        }

        public string Usage()
        {
            var tokens = new List<string>();

            tokens.Add(this.commandName);
            foreach (var parameter in parameters)
            {
                tokens.Add("<" + parameter.name + ">");
            }

            return string.Join(" ", tokens);
        }
    }

    public class Parameter
    {
        public enum PrimitiveType
        {
            Integer,
            String
        }

        public readonly string name;
        private readonly PrimitiveType primitiveType;
        private object value = null;

        public Parameter(string name, PrimitiveType type)
        {
            this.name = name;
            this.primitiveType = type;
        }

        public void ExtractValue(TokenList args)
        {
            if (this.primitiveType == PrimitiveType.Integer)
                this.value = args.NextInt();
            else if (this.primitiveType == PrimitiveType.String)
                this.value = args.NextString();
        }

        public string AsString()
        {
            return this.value as string;
        }

        public int AsInt()
        {
            return Convert.ToInt32(this.value);
        }
    }
}
