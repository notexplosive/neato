using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neato
{
    public class FfmpegProgram : ExternalProgram
    {
        public FfmpegProgram(Logger logger) : base("ffmpeg", logger)
        {
        }

        public string GenerateConcatFromPngs(FileManager folder)
        {
            var fileNames = folder.GetAllFiles(new PathContext(PathType.Absolute, folder.WorkingDirectory), "*.png");
            var result = new StringBuilder();

            void AppendFileName(string fileName)
            {
                result.AppendLine($"file {Path.GetFileName(fileName)}");
            }

            foreach (var fileName in fileNames)
            {
                AppendFileName(fileName);
                result.AppendLine($"duration 0.05");
            }

            // ffmpeg quirk, last file must be specified twice with no duration
            AppendFileName(fileNames.Last());

            return result.ToString().Replace('\\', '/');
        }
    }
}
