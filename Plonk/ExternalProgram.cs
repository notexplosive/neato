using System;
using System.Diagnostics;
using System.IO;

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
            var stdOutput = "ran command: " + this.runPath + (argumentList.Length > 0 ? " " : "") + string.Join(" ", argumentList);
            var wasSuccessful = true;
            using (Process process = new Process())
            {
                process.StartInfo.FileName = runPath;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                foreach (var argument in argumentList)
                {
                    process.StartInfo.ArgumentList.Add(argument);
                }

                try
                {
                    process.Start();
                    StreamReader reader = process.StandardOutput;
                    stdOutput = reader.ReadToEnd();
                    process.WaitForExit();
                }
                catch (System.ComponentModel.Win32Exception)
                {
                    wasSuccessful = false;
                }
            }
            return new ProgramOutput(wasSuccessful, stdOutput);
        }
    }
}
