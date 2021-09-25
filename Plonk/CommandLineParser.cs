using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neato
{
    public class UnknownCommandException : Exception
    {
        public string CommandName { get; }

        public UnknownCommandException(string commandName) : base()
        {
            CommandName = commandName;
        }
    }

    public class WrongNumberOfArgsException : Exception
    {
        public Command Command { get; }

        public WrongNumberOfArgsException(Command command) : base()
        {
            Command = command;
        }
    }

    public class CommandAbsentException : Exception
    {

    }

    public class CommandLineParser
    {
        private readonly Dictionary<string, Command> registeredCommands = new Dictionary<string, Command>();

        public void Consume(string[] argArray)
        {
            var args = new TokenList(argArray);
            string commandName;
            try
            {
                commandName = args.NextString();
            }
            catch (TokenizerFailedException)
            {
                throw new CommandAbsentException();
            }

            if (registeredCommands.ContainsKey(commandName))
            {
                var command = registeredCommands[commandName];
                if (args.Remaining().Count() == command.parameters.Count())
                {
                    command.Execute(args);
                }
                else
                {
                    throw new WrongNumberOfArgsException(command);
                }
            }
            else
            {
                throw new UnknownCommandException(commandName);
            }
        }

        public string SupportedCommands()
        {
            return string.Join(", ", this.registeredCommands.Keys);
        }

        public Command RegisterCommand(string commandName)
        {
            var command = new Command(commandName);
            this.registeredCommands.Add(commandName, command);
            return command;
        }
    }

    public class CommandFailedException : Exception
    {
        public Command Command { get; }

        public CommandFailedException(Command command) : base()
        {
            Command = command;
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

        public void Execute(TokenList args)
        {
            try
            {
                foreach (var parameter in parameters)
                {
                    parameter.ExtractValue(args);
                }
                executed?.Invoke(parameters);
            }
            catch (TokenizerFailedException)
            {
                throw new CommandFailedException(this);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new CommandFailedException(this);
            }
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
        private enum PrimitiveType
        {
            Integer,
            String
        }

        public readonly string name;
        private readonly PrimitiveType primitiveType;
        private object value = null;

        private Parameter(string name, PrimitiveType type)
        {
            this.name = name;
            this.primitiveType = type;
        }

        public static Parameter Int(string name)
        {
            return new Parameter(name, PrimitiveType.Integer);
        }

        public static Parameter String(string name)
        {
            return new Parameter(name, PrimitiveType.String);
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
