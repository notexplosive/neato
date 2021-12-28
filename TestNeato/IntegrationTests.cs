using FluentAssertions;
using Neato;
using System;
using Xunit;

namespace TestNeato
{
    public class IntegrationTests
    {
        [Fact]
        public void TestNewProject()
        {
            var logger = new BufferedLogger();
            var neato = new NeatoApp(logger);
            neato.ConsumeUserInput("new-project", Guid.NewGuid().ToString());
            logger.GetMessagesOfLevel(LogLevel.Error).Should().HaveCount(0);
        }
    }
}
