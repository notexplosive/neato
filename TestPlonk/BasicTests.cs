using Neato;
using System;
using Xunit;
using FluentAssertions;
using System.IO;

namespace TestNeato
{
    public class BasicTests
    {
        [Fact]
        public void can_call_external_programs()
        {
            // Intentionally call `ping` with bad args so we get an easy to expect result

            var program = new ExternalProgram("ping");
            var result = program.RunWithArgs("10.0.0.1", "-i", "0");

            result.wasSuccessful.Should().BeTrue();
        }

        [Fact]
        public void fails_to_call_programs_that_do_not_exist()
        {
            var program = new ExternalProgram("fjbnlazb");
            var result = program.RunWithArgs();

            result.stdOutput.Should().Be($"ran command: fjbnlazb\nin working directory: {Directory.GetCurrentDirectory()}");
            result.wasSuccessful.Should().Be(false);
        }

        [Fact]
        public void dotnet_is_installed()
        {
            var program = new DotnetProgram();
            var result = program.Version();

            result.wasSuccessful.Should().Be(true);
        }

        [Fact]
        public void git_is_installed()
        {
            var program = new GitProgram(".");
            var result = program.Version();

            result.wasSuccessful.Should().Be(true);
        }

        [Fact]
        public void sevenzip_is_installed()
        {
            var program = new SevenZipProgram();
            var result = program.Run();

            result.wasSuccessful.Should().Be(true);
        }

        [Fact]
        public void butler_is_installed()
        {
            var program = new ButlerProgram();
            var result = program.Version();

            result.wasSuccessful.Should().Be(true);
        }

    }
}
