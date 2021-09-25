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

            parser.RegisterCommand("project")
                .AddParameter(Parameter.String("project name"))
                .OnExecuted((parameters) =>
                {
                    var projectName = parameters[0].AsString();
                    var localFiles = new FileManager(PathType.Relative);

                    var repoPath = Path.Join(localFiles.WorkingDirectory, projectName);
                    var git = new GitProgram(repoPath);
                    var dotnet = new DotnetProgram();

                    if (Directory.Exists(repoPath))
                    {
                        Console.WriteLine("Project already exists");
                        return;
                    }

                    localFiles.MakeDirectory(new PathContext(PathType.Relative, projectName));

                    var oldWorkingDir = Directory.GetCurrentDirectory();
                    Directory.SetCurrentDirectory(repoPath);

                    Console.WriteLine("Creating Repo");
                    git.RunWithArgs("init");

                    Console.WriteLine("Downloading Machina");
                    git.RunWithArgs("submodule", "add", "https://github.com/notexplosive/machina.git");

                    Console.WriteLine("Basic MonoGame Setup");
                    dotnet.RunWithArgs("new", "--install", "MonoGame.Templates.CSharp");
                    dotnet.RunWithArgs("tool", "install", "--global", "dotnet-mgcb-editor");
                    new ExternalProgram("mgcb-editor").RunWithArgs("--register");

                    Console.WriteLine("Creating Template");
                    dotnet.RunWithArgs("new", "mgdesktopgl", "-o", projectName);

                    Console.WriteLine("Creating Solution");
                    dotnet.RunWithArgs("new", "sln");

                    Console.WriteLine("Add projects to Solution");
                    dotnet.RunWithArgs("sln", "add", projectName);
                    var machinaLocalPath = Path.Join(".", "machina", "Machina");
                    dotnet.RunWithArgs("sln", "add", machinaLocalPath);
                    dotnet.RunWithArgs("sln", "add", Path.Join(".", "machina", "TestMachina"));

                    Console.WriteLine("Add Machina to Project");
                    dotnet.RunWithArgs("add", projectName, "reference", machinaLocalPath);

                    Console.WriteLine("Copying Files");
                    localFiles.Copy(
                        new PathContext(PathType.Relative, Path.Join(projectName, "machina", ".gitignore")),
                        new PathContext(PathType.Relative, Path.Join(projectName, ".gitignore")));
                    localFiles.Copy(
                        new PathContext(PathType.Relative, Path.Join(projectName, "machina", "game-readme.md")),
                        new PathContext(PathType.Relative, Path.Join(projectName, "readme.md")));
                    git.RunWithArgs("add", ".");
                    git.RunWithArgs("commit", "-m", "(Machina:Automated) Initial Commit");

                    Directory.SetCurrentDirectory(oldWorkingDir);
                    Console.WriteLine("Done");
                })
                ;

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
