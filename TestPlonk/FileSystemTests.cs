using FluentAssertions;
using Plonk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestPlonk
{
    public class FileSystemTests
    {

        [Fact]
        public void can_change_working_directory()
        {
            var fileSystem = new FileManager(PathType.Absolute, @"C:\temp");
            fileSystem.WorkingDirectory.Should().Be(@"C:\temp");
        }

        [Fact]
        public void can_use_relative_working_directory()
        {
            var fileSystem = new FileManager(PathType.Relative, @"temp");
            fileSystem.WorkingDirectory.Should().Be(Path.Join(Directory.GetCurrentDirectory(), "temp"));
        }
    }
}
