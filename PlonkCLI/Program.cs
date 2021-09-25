using Neato;
using System;
using System.IO;

namespace NeatoCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new CommandLineParser();

            parser.RegisterCommand("package-zip")
                .AddParameter(Parameter.String("path to csproj"))
                .AddParameter(Parameter.String("destination"))
                .AddParameter(Parameter.String("zip name"))
                .OnExecuted((parameters) =>
                {
                    var dotnet = new DotnetProgram();
                    var sevenZip = new SevenZipProgram();
                    var localFiles = new FileManager(PathType.Relative, parameters[0].AsString());
                    var outputDirectory = parameters[1].AsString();
                    var zipName = parameters[2].AsString();

                    Console.WriteLine("Packaging as zip");
                    var buildOutputFiles = new FileManager(PathType.Absolute, Path.Join(outputDirectory, "neato-temp"));
                    var dotnetResult = dotnet.NormalPublish(localFiles.WorkingDirectory, buildOutputFiles);
                    var sevenZipResult = sevenZip.SendToZip(buildOutputFiles.WorkingDirectory, outputDirectory, zipName);

                    buildOutputFiles.RemoveDirectoryRecursive(new PathContext(PathType.Relative, "."));

                    sevenZipResult.PrintToStdOut();
                    dotnetResult.PrintToStdOut();
                });

            parser.RegisterCommand("make-special-exe")
                .AddParameter(Parameter.String("path to csproj"))
                .AddParameter(Parameter.String("destination"))
                .OnExecuted((parameters) =>
                {
                    var dotnet = new DotnetProgram();
                    var files = new FileManager(PathType.Relative, parameters[0].AsString());
                    var outputDirectory = parameters[2].AsString();

                    Console.WriteLine("Packaging as executable");
                    var result = dotnet.PublishExe_Special(files.WorkingDirectory, outputDirectory);
                    result.PrintToStdOut();
                });

            var api = new CommandLineHumanAPI(parser);
            api.UserInput(args);

            var error = api.NextErrorLine();
            while (error != null)
            {
                Console.Error.WriteLine($"[error] {error}");
                error = api.NextErrorLine();
            }
        }
    }
}
