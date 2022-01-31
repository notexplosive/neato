using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neato
{
    public class CommandLineHumanAPI
    {
        private readonly CommandLineParser parser;
        private readonly List<string> error;

        public CommandLineHumanAPI(CommandLineParser parser)
        {
            this.parser = parser;
            this.error = new List<string>();
        }

        public bool ConsumeUserInput(params string[] args)
        {
#if DEBUG
#else
            try
            {
#endif
            this.parser.Consume(args);
#if DEBUG
#else
            }
            catch (CommandFailedException e)
            {
                this.error.Add($"Failed. {e.Message}");
                this.error.Add($"Usage: {e.Command.Usage()}");
                return false;
            }
            catch (UnknownCommandException e)
            {
                this.error.Add($"Unknown command '{e.CommandName}'");
                this.error.Add($"Commands: {parser.SupportedCommands()}");
                return false;
            }
            catch (CommandAbsentException)
            {
                this.error.Add($"Missing command.");
                this.error.Add($"Commands: {parser.SupportedCommands()}");
                return false;
            }
            catch (WrongNumberOfArgsException e)
            {
                this.error.Add($"Wrong number of arguments.");
                this.error.Add($"Usage: {e.Command.Usage()}");
            }
            catch (Exception e)
            {
                this.error.Add($"Generic failure: {e.Message}");
            }
#endif

            return true;
        }

        public string NextErrorLine()
        {
            if (this.error.Count > 0)
            {
                var result = this.error[0];
                this.error.RemoveAt(0);
                return result;
            }
            else
            {
                return null;
            }
        }
    }
}
