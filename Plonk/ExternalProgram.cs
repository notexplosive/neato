using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Neato
{
    public class ProgramOutput
    {
        public readonly bool wasSuccessful;

        public ProgramOutput(bool wasSuccessful)
        {
            this.wasSuccessful = wasSuccessful;
        }
    }

    public enum OutputLevel
    {
        Allow,
        Suppress
    }

    public class ExternalProgram
    {
        private readonly string runPath;

        public ExternalProgram(string runPath)
        {
            this.runPath = runPath;
        }

        public ProgramOutput RunWithArgs(OutputLevel outputLevel, params string[] argumentList)
        {
            return RunWithArgsAt(Directory.GetCurrentDirectory(), outputLevel, argumentList);
        }

        public ProgramOutput RunWithArgsAt(string workingDirectory, OutputLevel outputLevel, params string[] argumentList)
        {
            if (outputLevel == OutputLevel.Allow)
            {
                Console.WriteLine("ran command: " + this.runPath + (argumentList.Length > 0 ? " " : "") + string.Join(" ", argumentList)
                    + "\n" + "in working directory: " + workingDirectory);
            }
            var wasSuccessful = true;
            using (Process process = new Process())
            {
                process.StartInfo.WorkingDirectory = workingDirectory;
                process.StartInfo.FileName = runPath;
                process.StartInfo.UseShellExecute = false;

                if (outputLevel == OutputLevel.Suppress)
                {
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                }

                foreach (var argument in argumentList)
                {
                    process.StartInfo.ArgumentList.Add(argument);
                }

                try
                {
                    process.Start();

                    if (outputLevel == OutputLevel.Suppress)
                    {
                        // Flush the buffers
                        process.StandardOutput.ReadToEnd();
                        process.StandardError.ReadToEnd();
                    }

                    process.WaitForExit();
                }
                catch (System.ComponentModel.Win32Exception)
                {
                    wasSuccessful = false;
                }
            }
            return new ProgramOutput(wasSuccessful);
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.Write(e.Data);
        }
    }
}

