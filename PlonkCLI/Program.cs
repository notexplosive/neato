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

            parser.RegisterCommand("package")
                .AddParameter(Parameter.String("path to csproj"))
                .AddParameter(Parameter.String("exe|zip"))
                .AddParameter(Parameter.String("destination"))
                .OnExecuted((parameters) =>
                {
                    var dotnet = new DotnetProgram();
                    var files = new FileManager(PathType.Relative, parameters[0].AsString());

                    if (parameters[1].AsString() == "exe")
                    {
                        Console.WriteLine("Packaging as executable");
                    }


                    var outputDirectory = parameters[2].AsString();
                    var result = dotnet.RunWithArgs(
                        "publish",
                        files.WorkingDirectory,
                        "-c", "Release",
                        "-r", "win-x64",
                        "/p:PublishReadyToRun=false",
                        "/p:TieredCompilation=false",
                        "/p:IncludeNativeLibrariesForSelfExtract=true",
                        "--self-contained",
                        "--output", outputDirectory);

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
