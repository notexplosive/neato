﻿using FluentAssertions;
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

            parser.RegisterCommand("fluff").OnExecuted((args) =>
            {
                numberVal = args.NextInt();
                stringVal = args.NextString();
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

            parser.RegisterCommand("fluff").Executed += (args) =>
            {
                numberVal = args.NextInt();
                stringVal = args.NextString();
            };
            parser.Consume(new string[] { "fluff", "5" });

            stringVal.Should().Be("original");
            numberVal.Should().Be(-23);
        }
    }
}
