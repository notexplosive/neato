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
    public class CommandLineHumanAPITests
    {
        [Fact]
        public void output_usage_if_command_args_invalid()
        {
            var parser = new CommandLineParser();
            var api = new CommandLineHumanAPI(parser);

            parser.RegisterCommand("tick")
                .AddParameter(new Parameter("number of times", Parameter.PrimitiveType.Integer));

            api.UserInput("tick");
            api.NextErrorLine().Should().Be("Missing value at position 1");
            api.NextErrorLine().Should().Be("Usage: tick <number of times>");
        }
    }
}
