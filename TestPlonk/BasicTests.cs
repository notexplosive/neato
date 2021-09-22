using Plonk;
using System;
using Xunit;
using FluentAssertions;

namespace TestPlonk
{
    public class BasicTests
    {
        [Fact]
        public void can_call_external_programs()
        {
            // Intentionally call `ping` with bad args so we get an easy to expect result

            var program = new ExternalProgram("ping");
            var result = program.RunWithArgs("10.0.0.1", "-i", "0");

            result.stdOutput.Should().Be("Bad value for option -i, valid range is from 1 to 255.\r\n");
        }

        [Fact]
        public void fails_to_call_programs_that_do_not_exist()
        {
            var program = new ExternalProgram("fjbnlazb");
            var result = program.RunWithArgs();

            result.stdOutput.Should().Be("");
            result.wasSuccessful.Should().Be(false);
        }
    }
}
