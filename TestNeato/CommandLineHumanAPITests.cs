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
        public void output_usage_if_command_missing_args()
        {
            var parser = new CommandLineParser();
            var api = new CommandLineHumanAPI(parser);

            parser.RegisterCommand("tick")
                .AddParameter(Parameter.Int("number of times"));

            api.UserInput("tick");
            api.NextErrorLine().Should().Be("Wrong number of arguments.");
            api.NextErrorLine().Should().Be("Usage: tick <number of times>");
        }

        [Fact]
        public void output_usage_if_command_not_found()
        {
            var parser = new CommandLineParser();
            var api = new CommandLineHumanAPI(parser);

            parser.RegisterCommand("tick")
                .AddParameter(Parameter.Int("number of times"));
            parser.RegisterCommand("tack")
                .AddParameter(Parameter.Int("number of times"));
            parser.RegisterCommand("foo")
                .AddParameter(Parameter.Int("number of times"));

            api.UserInput("tock");
            api.NextErrorLine().Should().Be("Unknown command 'tock'");
            api.NextErrorLine().Should().Be("Commands: tick, tack, foo");
        }

        [Fact]
        public void output_usage_if_no_command()
        {
            var parser = new CommandLineParser();
            var api = new CommandLineHumanAPI(parser);

            parser.RegisterCommand("tick")
                .AddParameter(Parameter.Int("number of times"));
            parser.RegisterCommand("tack")
                .AddParameter(Parameter.Int("number of times"));
            parser.RegisterCommand("foo")
                .AddParameter(Parameter.Int("number of times"));

            api.UserInput();
            api.NextErrorLine().Should().Be("Missing command.");
            api.NextErrorLine().Should().Be("Commands: tick, tack, foo");
        }

        [Fact]
        public void output_usage_if_too_many_args()
        {
            var parser = new CommandLineParser();
            var api = new CommandLineHumanAPI(parser);

            parser.RegisterCommand("tick")
                .AddParameter(Parameter.Int("number of times"));

            api.UserInput("tick", "3", "10", "8");
            api.NextErrorLine().Should().Be("Wrong number of arguments.");
            api.NextErrorLine().Should().Be("Usage: tick <number of times>");
        }

        [Fact]
        public void malformed_command_raises_error()
        {
            var parser = new CommandLineParser();
            var api = new CommandLineHumanAPI(parser);

            parser.RegisterCommand("tick")
                .OnExecuted((parameters) =>
                {
                    parameters[5].AsString();
                })
                ;

            api.UserInput("tick");
            api.NextErrorLine().Should().Be("Failed. Malformed command");
            api.NextErrorLine().Should().Be("Usage: tick");
        }
    }
}
