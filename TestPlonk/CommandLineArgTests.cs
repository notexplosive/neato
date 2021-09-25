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
            var pass = false;

            try
            {
                parser.Consume(new string[] { "--spujb" });
            }
            catch (UnknownCommandException)
            {
                pass = true;
            }

            pass.Should().BeTrue();
        }

        [Fact]
        public void empty_args_raises_error()
        {
            var parser = new CommandLineParser();
            var pass = false;

            try
            {
                parser.Consume(new string[] { });
            }
            catch (CommandAbsentException)
            {
                pass = true;
            }

            pass.Should().BeTrue();
        }

        [Fact]
        public void registered_command_runs_command_no_args()
        {
            var parser = new CommandLineParser();
            bool wasFluffed = false;
            bool wasGargled = false;
            parser.RegisterCommand("fluff").OnExecuted((args) => { wasFluffed = true; });
            parser.RegisterCommand("gargle").OnExecuted((args) => { wasGargled = true; });

            parser.Consume(new string[] { "fluff" });

            wasGargled.Should().BeFalse();
            wasFluffed.Should().BeTrue();
        }

        [Fact]
        public void registered_command_runs_command_args()
        {
            var parser = new CommandLineParser();
            var stringVal = string.Empty;
            var numberVal = -1;

            parser.RegisterCommand("fluff")
                .AddParameter(Parameter.Int("crab"))
                .AddParameter(Parameter.String("claw"))
                .OnExecuted((parameters) =>
            {
                numberVal = parameters[0].AsInt();
                stringVal = parameters[1].AsString();
            });
            parser.Consume(new string[] { "fluff", "5", "garfield" });

            stringVal.Should().Be("garfield");
            numberVal.Should().Be(5);
        }

        [Fact]
        public void registered_command_runs_fails_with_not_enough_args()
        {
            var parser = new CommandLineParser();
            var stringVal = "original";
            var numberVal = -23;
            var caughtFailure = false;

            parser.RegisterCommand("fluff")
                .AddParameter(Parameter.String("claw"))
                .AddParameter(Parameter.Int("crab"))
                .OnExecuted((parameters) =>
            {
                stringVal = parameters[0].AsString();
                numberVal = parameters[1].AsInt();
            });

            try
            {
                parser.Consume(new string[] { "fluff", "raggle" });
            }
            catch (CommandFailedException)
            {
                caughtFailure = true;
            }

            stringVal.Should().Be("original");
            numberVal.Should().Be(-23);
            caughtFailure.Should().BeTrue();
        }

        [Fact]
        public void command_tells_you_how_to_use_it()
        {
            var command = new Command("fluff")
                .AddParameter(
                    Parameter.Int("fluffing amount"))
                .AddParameter(
                    Parameter.String("name"));

            command.Usage().Should().Be("fluff <fluffing amount> <name>");
        }
    }
}
