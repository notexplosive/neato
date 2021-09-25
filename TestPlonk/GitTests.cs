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
    public class GitTests : IDisposable
    {
        private readonly string repoPath;

        public GitTests()
        {
            var testPathRelative = Guid.NewGuid().ToString();
            this.repoPath = Path.Join(Directory.GetCurrentDirectory(), testPathRelative);
            Directory.CreateDirectory(this.repoPath);
        }

        public void Dispose()
        {
            if (Directory.Exists(this.repoPath))
            {
                Directory.Delete(this.repoPath, true);
            }
        }

        [Fact]
        public void git_init()
        {
            new GitProgram(this.repoPath).Init();
            Directory.Exists(Path.Join(this.repoPath, ".git")).Should().BeTrue();
        }
    }
}
