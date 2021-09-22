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

        [Fact]
        public void can_start_at_working_directory()
        {
            var fileSystem = new FileManager(PathType.Relative);
            fileSystem.WorkingDirectory.Should().Be(Directory.GetCurrentDirectory());
        }

        [Fact]
        public void can_create_relative_directories()
        {
            var fileSystem = new FileManager(PathType.Relative);
            var testPathRelative = "test-empty-directory";
            var testPathFull = Path.Join(Directory.GetCurrentDirectory(), testPathRelative);
            try { Directory.Delete(testPathFull); }
            catch { }
            var lsResultsBefore = Directory.GetDirectories(Directory.GetCurrentDirectory());
            fileSystem.MakeDirectory(PathType.Relative, testPathRelative);
            var lsResultsAfter = Directory.GetDirectories(Directory.GetCurrentDirectory());

            lsResultsBefore.Should().NotContain(testPathFull);
            lsResultsAfter.Should().Contain(testPathFull);

            Directory.Delete(testPathFull);
        }

        [Fact]
        public void can_remove_relative_directories()
        {
            var testPathRelative = Guid.NewGuid().ToString();
            var testPathFull = Path.Join(Directory.GetCurrentDirectory(), testPathRelative);
            var fileSystem = new FileManager(PathType.Relative);
            Directory.CreateDirectory(testPathFull);

            var existsBefore = Directory.Exists(testPathFull);
            fileSystem.RemoveDirectory(PathType.Relative, testPathRelative);
            var existsAfter = Directory.Exists(testPathFull);

            existsBefore.Should().BeTrue();
            existsAfter.Should().BeFalse();
        }

        [Fact]
        public void can_create_absolute_directories()
        {
            var fileSystem = new FileManager(PathType.Relative);
            var testPathRelative = "test-empty-directory";
            var testPathFull = Path.Join(Directory.GetCurrentDirectory(), testPathRelative);
            try { Directory.Delete(testPathFull); }
            catch { }
            var lsResultsBefore = Directory.GetDirectories(Directory.GetCurrentDirectory());
            fileSystem.MakeDirectory(PathType.Absolute, testPathFull);
            var lsResultsAfter = Directory.GetDirectories(Directory.GetCurrentDirectory());

            lsResultsBefore.Should().NotContain(testPathFull);
            lsResultsAfter.Should().Contain(testPathFull);

            Directory.Delete(testPathFull);
        }

        [Fact]
        public void can_remove_absolute_directories()
        {
            var testPathRelative = Guid.NewGuid().ToString();
            var testPathFull = Path.Join(Directory.GetCurrentDirectory(), testPathRelative);
            var fileSystem = new FileManager(PathType.Relative);
            Directory.CreateDirectory(testPathFull);

            var existsBefore = Directory.Exists(testPathFull);
            fileSystem.RemoveDirectory(PathType.Absolute, testPathFull);
            var existsAfter = Directory.Exists(testPathFull);

            existsBefore.Should().BeTrue();
            existsAfter.Should().BeFalse();
        }
    }
}
