using FluentAssertions;
using Neato;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestNeato
{
    public class CommandLineParserTests
    {
        [Fact]
        public void output_usage_if_command_args_invalid()
        {
            var parser = new CommandLineParser();

            parser.RegisterCommand("tick")
                .AddParameter(new Parameter("number of times", Parameter.PrimitiveType.Integer));

            using (StringWriter error = new StringWriter())
            {
                var oldError = Console.Error;

                Console.SetError(error);
                parser.Consume(new string[] { "tick" });

                error.ToString().Trim().Should().Be("usage: tick <number of times>");

                Console.SetError(oldError);
            }
        }
    }
}
