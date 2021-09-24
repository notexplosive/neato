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

        public bool UserInput(params string[] args)
        {
            try
            {
                parser.Consume(args);
            }
            catch (CommandFailedException e)
            {
                this.error.Add(e.Message);
                this.error.Add($"Usage: {e.Command.Usage()}");
                return false;
            }
            catch (CommandNotFoundException e)
            {
                this.error.Add(e.Message);
                return false;
            }

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
