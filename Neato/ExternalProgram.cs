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

    public enum ProgramOutputLevel
    {
        AllowProgramToEmitToConsole,
        SuppressProgramFromEmittingToConsole
    }

    public class ExternalProgram
    {
        private readonly string runPath;
        private readonly Logger logger;

        public ExternalProgram(string runPath, Logger logger)
        {
            this.runPath = runPath;
            this.logger = logger;
        }

        public ProgramOutput RunWithArgs(ProgramOutputLevel outputLevel, params string[] argumentList)
        {
            return RunWithArgsAt(Directory.GetCurrentDirectory(), outputLevel, argumentList);
        }

        public ProgramOutput RunWithArgsAt(string workingDirectory, ProgramOutputLevel outputLevel, params string[] argumentList)
        {
            if (outputLevel == ProgramOutputLevel.AllowProgramToEmitToConsole)
            {
                this.logger.Info("ran command: " + this.runPath + (argumentList.Length > 0 ? " " : "") + string.Join(" ", argumentList)
                    + "\n" + "in working directory: " + workingDirectory);
            }
            var wasSuccessful = true;
            using (Process process = new Process())
            {
                process.StartInfo.WorkingDirectory = workingDirectory;
                process.StartInfo.FileName = runPath;
                process.StartInfo.UseShellExecute = false;

                if (outputLevel == ProgramOutputLevel.SuppressProgramFromEmittingToConsole)
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

                    if (outputLevel == ProgramOutputLevel.SuppressProgramFromEmittingToConsole)
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

