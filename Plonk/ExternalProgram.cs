using System;
using System.Diagnostics;
using System.IO;

namespace Plonk
{
    public class ExternalProgram
    {
        private readonly string runPath;

        public ExternalProgram(string runPath)
        {
            this.runPath = runPath;
        }

        public string RunWithArgs(params string[] argumentList)
        {
            string output = string.Empty;
            using (Process process = new Process())
            {
                process.StartInfo.FileName = runPath;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                foreach (var argument in argumentList)
                {
                    process.StartInfo.ArgumentList.Add(argument);
                }
                process.Start();

                StreamReader reader = process.StandardOutput;
                output = reader.ReadToEnd();

                process.WaitForExit();
            }
            return output;
        }
    }
}
