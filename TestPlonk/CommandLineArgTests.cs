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

            result.Should().Be(string.Format("Usage {0}", Path.Join(Directory.GetCurrentDirectory(), System.AppDomain.CurrentDomain.FriendlyName)));
        }
    }
}
