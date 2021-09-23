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
    public class CommandLineArgTests
    {
        [Fact]
        public void invalid_flag_raises_error()
        {
            var parser = new CommandLineParser();
            var result = parser.Consume(new string[] { "--spujb" });

            result.Should().BeFalse();
        }

        [Fact]
        public void registered_command_runs_command()
        {
            var parser = new CommandLineParser();
            bool wasFluffed = false;
            bool wasGargled = false;
            parser.RegisterCommand("fluff", () => { wasFluffed = true; });
            parser.RegisterCommand("gargle", () => { wasGargled = true; });

            parser.Consume(new string[] { "fluff" });

            wasGargled.Should().BeFalse();
            wasFluffed.Should().BeTrue();
        }
    }
}
