using FluentAssertions;
using Neato;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestNeato
{
    public class TokenListTests
    {
        [Fact]
        public void takes_params()
        {
            var tokenList = new TokenList("1", "2", "three");

            tokenList.NextString().Should().Be("1");
            tokenList.NextString().Should().Be("2");
            tokenList.NextString().Should().Be("three");
        }

        [Fact]
        public void integer_types_can_be_strings()
        {
            var tokenList = new TokenList("1", "2", "three");

            tokenList.NextInt().Should().Be(1);
            tokenList.NextString().Should().Be("2");
        }
    }
}
