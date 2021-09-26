using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Neato
{
    public class ProgramOutput
    {
        public readonly string stdOutput;
        public readonly bool wasSuccessful;

        public ProgramOutput(bool wasSuccessful, string output)
        {
            this.stdOutput = output;
            this.wasSuccessful = wasSuccessful;
        }

        public void PrintToStdOut()
        {
            Console.WriteLine(this.stdOutput);
        }
    }

    public class ExternalProgram
    {
        private readonly string runPath;

        public ExternalProgram(string runPath)
        {
            this.runPath = runPath;
        }

        public ProgramOutput RunWithArgs(params string[] argumentList)
        {
            return RunWithArgsAt(Directory.GetCurrentDirectory(), argumentList);
        }
        public ProgramOutput RunWithArgsAt(string workingDirectory, params string[] argumentList)
        {
            var allOutput = "ran command: " + this.runPath + (argumentList.Length > 0 ? " " : "") + string.Join(" ", argumentList)
                + "\n" + "in working directory: " + workingDirectory;
            var wasSuccessful = true;
            using (Process process = new Process())
            {
                process.StartInfo.WorkingDirectory = workingDirectory;
                process.StartInfo.FileName = runPath;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.StandardOutputEncoding = System.Text.Encoding.UTF8;
                foreach (var argument in argumentList)
                {
                    process.StartInfo.ArgumentList.Add(argument);
                }

                try
                {
                    process.Start();
                    StreamReader standardReader = process.StandardOutput;
                    StreamReader errorReader = process.StandardError;

                    process.OutputDataReceived += Process_OutputDataReceived;
                    process.ErrorDataReceived += Process_OutputDataReceived;

                    allOutput = standardReader.ReadToEnd();
                    allOutput += errorReader.ReadToEnd();
                    process.WaitForExit();
                }
                catch (System.ComponentModel.Win32Exception)
                {
                    wasSuccessful = false;
                }
            }
            return new ProgramOutput(wasSuccessful, allOutput);
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.Write(e.Data);
        }
    }
}

