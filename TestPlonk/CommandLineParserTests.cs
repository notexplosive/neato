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
        public void output_usage_if_command_invalid()
        {
            var parser = new CommandLineParser();

            parser.RegisterCommand("tick")
                .AddParameter(new Parameter("number of times", Parameter.PrimitiveType.Integer));

            using (StringWriter sw = new StringWriter())
            {
                Console.SetError(sw);
                parser.Consume(new string[] { "tick" });

                sw.ToString().Trim().Should().Be("usage: tick <number of times>");
            }
        }
    }
}
