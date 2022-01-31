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

        public string GenerateConcatFromPngs(List<string> fileNames)
        {
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

        public List<string> SetupTempScreenshotsFolder(FileManager screenshotsFolder, FileManager tempFolder)
        {
            var result = new List<string>();
            var originalFileNames = screenshotsFolder.GetAllFiles(new PathContext(PathType.Absolute, screenshotsFolder.WorkingDirectory), "*.png");

            var knownFileNames = new HashSet<string>();

            foreach (var fileName in originalFileNames)
            {
                knownFileNames.Add(fileName);
            }

            var sampleFileName = originalFileNames[0];
            // Assumes that all images will have a name like `img (235).png`, the prefix in this case is `img`
            var prefix = sampleFileName.Split()[0];


            // We traverse this way to ensure we hit each file in the correct order.
            // the first image would be `img (1).png` so we start at 1 and end at n+1
            for (int i = 1; i < originalFileNames.Length + 1; i++)
            {
                var assumedFileName = $"{prefix} ({i}).png";

                if (!knownFileNames.Contains(assumedFileName))
                {
                    throw new Exception($"Assumed based on the file names that `{assumedFileName}` would exist, but it didn't.");
                }

                var fileName = i.ToString("D8") + ".png";
                tempFolder.MakeDirectory(new PathContext(PathType.Absolute, tempFolder.WorkingDirectory));
                screenshotsFolder.Copy(new PathContext(PathType.Absolute, assumedFileName), new PathContext(PathType.Absolute, Path.Join(tempFolder.WorkingDirectory, fileName)));
                result.Add(fileName);
            }

            return result;
        }
    }
}
