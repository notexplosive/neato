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
    public class FileSystemTests : IDisposable
    {
        private readonly string testPathRelative;
        private readonly string testPathFull;

        public FileSystemTests()
        {
            this.testPathRelative = Guid.NewGuid().ToString();
            this.testPathFull = Path.Join(Directory.GetCurrentDirectory(), testPathRelative);
            Directory.CreateDirectory(this.testPathFull);
        }

        public void Dispose()
        {
            if (Directory.Exists(this.testPathFull))
            {
                Directory.Delete(this.testPathFull, true);
            }
        }

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
            var emptyDirectoryFull = Path.Join(this.testPathFull, "empty-directory");
            var lsResultsBefore = Directory.GetDirectories(this.testPathFull);
            fileSystem.MakeDirectory(PathType.Relative, Path.Join(this.testPathRelative, "empty-directory"));
            var lsResultsAfter = Directory.GetDirectories(this.testPathFull);

            lsResultsBefore.Should().NotContain(emptyDirectoryFull);
            lsResultsAfter.Should().Contain(emptyDirectoryFull);
        }

        [Fact]
        public void can_remove_relative_directories()
        {
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
            var emptyDirectoryRelative = "test-empty-directory";
            var emptyDirectoryFull = Path.Join(Directory.GetCurrentDirectory(), emptyDirectoryRelative);
            var lsResultsBefore = Directory.GetDirectories(Directory.GetCurrentDirectory());
            fileSystem.MakeDirectory(PathType.Absolute, emptyDirectoryFull);
            var lsResultsAfter = Directory.GetDirectories(Directory.GetCurrentDirectory());

            lsResultsBefore.Should().NotContain(emptyDirectoryFull);
            lsResultsAfter.Should().Contain(emptyDirectoryFull);

            Directory.Delete(emptyDirectoryFull);
        }

        [Fact]
        public void can_remove_absolute_directories()
        {
            var fileSystem = new FileManager(PathType.Relative);
            Directory.CreateDirectory(testPathFull);

            var existsBefore = Directory.Exists(testPathFull);
            fileSystem.RemoveDirectory(PathType.Absolute, testPathFull);
            var existsAfter = Directory.Exists(testPathFull);

            existsBefore.Should().BeTrue();
            existsAfter.Should().BeFalse();
        }

        [Fact]
        public void can_recursively_remove_directories()
        {
            Directory.CreateDirectory(Path.Join(testPathFull, "temp"));

            var existsBefore = Directory.Exists(testPathFull);
            var fileSystem = new FileManager(PathType.Relative);
            fileSystem.RemoveDirectoryRecursive(PathType.Absolute, testPathFull);
            var existsAfter = Directory.Exists(testPathFull);

            existsBefore.Should().BeTrue();
            existsAfter.Should().BeFalse();
        }

        [Fact]
        public void can_create_file_relative()
        {
            var fileSystem = new FileManager(PathType.Relative, testPathRelative);

            var existsBefore = File.Exists(Path.Join(testPathFull, "a.test"));
            fileSystem.CreateFile(PathType.Relative, "a.test");
            var existsAfter = File.Exists(Path.Join(testPathFull, "a.test"));

            existsBefore.Should().BeFalse();
            existsAfter.Should().BeTrue();
        }

        [Fact]
        public void can_create_file_absolute()
        {
            var fileSystem = new FileManager(PathType.Absolute, testPathFull);

            var existsBefore = File.Exists(Path.Join(testPathFull, "a.test"));
            fileSystem.CreateFile(PathType.Absolute, Path.Join(testPathFull, "a.test"));
            var existsAfter = File.Exists(Path.Join(testPathFull, "a.test"));

            existsBefore.Should().BeFalse();
            existsAfter.Should().BeTrue();
        }

        [Fact]
        public void can_remove_files_matching_wildcard_relative()
        {
            var fileSystem = new FileManager(PathType.Relative, testPathRelative);
            fileSystem.CreateFile(PathType.Relative, "a.del");
            fileSystem.CreateFile(PathType.Relative, "b.del");
            fileSystem.CreateFile(PathType.Relative, "c.del");
            fileSystem.CreateFile(PathType.Relative, "a.keep");
            fileSystem.CreateFile(PathType.Relative, "b.keep");
            Directory.CreateDirectory(Path.Join(testPathFull, "temp"));

            var filesBefore = Directory.GetFiles(testPathFull);
            fileSystem.RemoveFiles(PathType.Relative, ".", "*.del");
            var filesAfter = Directory.GetFiles(testPathFull);

            filesBefore.Should().HaveCount(5);
            filesAfter.Should().HaveCount(2);
            filesAfter.Should().Contain(Path.Join(testPathFull, "a.keep"), Path.Join(testPathFull, "b.keep"));
            Directory.GetDirectories(testPathFull).Should().Contain(Path.Join(testPathFull, "temp"));
            filesAfter.Should().NotContain(Path.Join(testPathFull, "a.del"), Path.Join(testPathFull, "b.del"), Path.Join(testPathFull, "c.del"));

            fileSystem.RemoveDirectoryRecursive(PathType.Absolute, testPathFull);
        }

        [Fact]
        public void can_remove_files_matching_wildcard_absolute()
        {
            var fileSystem = new FileManager(PathType.Absolute, testPathFull);
            fileSystem.CreateFile(PathType.Relative, "a.del");
            fileSystem.CreateFile(PathType.Relative, "b.del");
            fileSystem.CreateFile(PathType.Relative, "c.del");
            fileSystem.CreateFile(PathType.Relative, "a.keep");
            fileSystem.CreateFile(PathType.Relative, "b.keep");
            Directory.CreateDirectory(Path.Join(testPathFull, "temp"));

            var filesBefore = Directory.GetFiles(testPathFull);
            fileSystem.RemoveFiles(PathType.Absolute, testPathFull, "*.del");
            var filesAfter = Directory.GetFiles(testPathFull);

            filesBefore.Should().HaveCount(5);
            filesAfter.Should().HaveCount(2);
            filesAfter.Should().Contain(Path.Join(testPathFull, "a.keep"), Path.Join(testPathFull, "b.keep"));
            Directory.GetDirectories(testPathFull).Should().Contain(Path.Join(testPathFull, "temp"));
            filesAfter.Should().NotContain(Path.Join(testPathFull, "a.del"), Path.Join(testPathFull, "b.del"), Path.Join(testPathFull, "c.del"));

            fileSystem.RemoveDirectoryRecursive(PathType.Absolute, testPathFull);
        }


        /*
        [Fact]
        public void can_write_to_file_relative()
        {
        }
        */

        /*
        [Fact]
        public void can_write_to_file_absolute()
        {
        }
        */

        /*
        [Fact]
        public void can_copy_file_relative_to_relative()
        {
        }
        */

        /*
        [Fact]
        public void can_copy_file_relative_to_absolute()
        {
        }
        */

        /*
        [Fact]
        public void can_copy_file_absolute_to_relative()
        {
        }
        */

        /*
        [Fact]
        public void can_copy_file_absolute_to_absolute()
        {
        }
        */
    }
}
