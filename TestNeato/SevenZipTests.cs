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
    public class SevenZipTests : IDisposable
    {
        private readonly string directoryToZip;
        private readonly string outputDirectory;

        public SevenZipTests()
        {
            this.directoryToZip = Path.Join(Directory.GetCurrentDirectory(), Guid.NewGuid().ToString());
            this.outputDirectory = Path.Join(Directory.GetCurrentDirectory(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(directoryToZip);

            File.Create(Path.Join(directoryToZip, "a.txt")).Close();
            File.Create(Path.Join(directoryToZip, "b.txt")).Close();
            File.Create(Path.Join(directoryToZip, "c.txt")).Close();
        }

        [Fact]
        public void can_zip_directory_without_dot_zip()
        {
            var sevenZip = new SevenZipProgram(new BufferedLogger());
            sevenZip.SendToZip(directoryToZip, outputDirectory, "output");

            var outputFile = Path.Join(outputDirectory, "output.zip");
            File.Exists(outputFile).Should().BeTrue();
        }

        [Fact]
        public void can_zip_directory_with_dot_zip()
        {
            var sevenZip = new SevenZipProgram(new BufferedLogger());
            sevenZip.SendToZip(directoryToZip, outputDirectory, "output.zip");

            var outputFile = Path.Join(outputDirectory, "output.zip");
            File.Exists(outputFile).Should().BeTrue();
        }

        public void Dispose()
        {
            if (Directory.Exists(directoryToZip))
            {
                Directory.Delete(directoryToZip, true);
            }

            if (Directory.Exists(outputDirectory))
            {
                Directory.Delete(outputDirectory, true);
            }
        }
    }
}
