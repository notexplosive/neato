using Neato;
using System;
using System.IO;

namespace NeatoCLI
{
    public static class Logger
    {
        public static void Info(string message)
        {
            Console.WriteLine($"🔵 {message}");
        }

        public static void Error(string message)
        {
            Console.WriteLine($"💢 {message}");
        }

        public static void Warning(string message)
        {
            Console.WriteLine($"⚠ {message}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
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

                    Logger.Info("Packaging as zip");
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

                    Logger.Info("Packaging as executable");
                    var result = dotnet.PublishExe_Special(files.WorkingDirectory, outputDirectory);
                    result.PrintToStdOut();
                });

            parser.RegisterCommand("login")
                .OnExecuted((parameters) =>
                {
                    new ButlerProgram().Login();
                });

            parser.RegisterCommand("status")
                .OnExecuted((parameters) =>
                {
                    void LogInstallStatus(string name, Func<bool> check)
                    {
                        if (check())
                        {
                            Logger.Info($"{name} is installed");
                        }
                        else
                        {
                            Logger.Warning($"{name} is not detected, either not installed or not on PATH");
                        }
                    }

                    LogInstallStatus("butler", () => new ButlerProgram().Version().wasSuccessful);
                    LogInstallStatus("git", () => new GitProgram(".").Version().wasSuccessful);
                    LogInstallStatus("dotnet", () => new DotnetProgram().Version().wasSuccessful);
                    LogInstallStatus("7zip", () => new SevenZipProgram().Run().wasSuccessful);
                });

            parser.RegisterCommand("deploy-itch")
                .AddParameter(Parameter.String("directory"))
                .AddParameter(Parameter.String("itch url"))
                .AddParameter(Parameter.String("game url"))
                .AddParameter(Parameter.String("channel"))
                .OnExecuted((parameters) =>
                {
                    var directoryToUpload = parameters[0].AsString();
                    var itchUrl = parameters[1].AsString();
                    var gameUrl = parameters[2].AsString();
                    var channel = parameters[3].AsString();
                    var butler = new ButlerProgram();
                    var result = butler.Push(directoryToUpload, itchUrl, gameUrl, channel);
                    Logger.Info(result.stdOutput);
                })
                ;

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
                        Logger.Info("Project already exists");
                        return;
                    }

                    localFiles.MakeDirectory(new PathContext(PathType.Relative, projectName));

                    var oldWorkingDir = Directory.GetCurrentDirectory();
                    Directory.SetCurrentDirectory(repoPath);

                    Logger.Info("Creating Repo");
                    git.RunWithArgs("init");

                    Logger.Info("Downloading Machina");
                    git.RunWithArgs("submodule", "add", "https://github.com/notexplosive/machina.git");

                    Logger.Info("Basic MonoGame Setup");
                    dotnet.RunWithArgs("new", "--install", "MonoGame.Templates.CSharp");
                    dotnet.RunWithArgs("tool", "install", "--global", "dotnet-mgcb-editor");
                    new ExternalProgram("mgcb-editor").RunWithArgs("--register");

                    Logger.Info("Creating Template");
                    dotnet.RunWithArgs("new", "mgdesktopgl", "-o", projectName);

                    Logger.Info("Creating Solution");
                    dotnet.RunWithArgs("new", "sln");

                    Logger.Info("Add projects to Solution");
                    dotnet.RunWithArgs("sln", "add", projectName);
                    var machinaLocalPath = Path.Join(".", "machina", "Machina");
                    dotnet.RunWithArgs("sln", "add", machinaLocalPath);
                    dotnet.RunWithArgs("sln", "add", Path.Join(".", "machina", "TestMachina"));

                    Logger.Info("Add Machina to Project");
                    dotnet.RunWithArgs("add", projectName, "reference", machinaLocalPath);

                    Logger.Info("Copying Files");
                    localFiles.Copy(
                        new PathContext(PathType.Relative, Path.Join(projectName, "machina", ".gitignore")),
                        new PathContext(PathType.Relative, Path.Join(projectName, ".gitignore")));
                    localFiles.Copy(
                        new PathContext(PathType.Relative, Path.Join(projectName, "machina", "game-readme.md")),
                        new PathContext(PathType.Relative, Path.Join(projectName, "readme.md")));
                    git.RunWithArgs("add", ".");
                    git.RunWithArgs("commit", "-m", "(Machina:Automated) Initial Commit");

                    Directory.SetCurrentDirectory(oldWorkingDir);
                    Logger.Info("Done");
                })
                ;

            var api = new CommandLineHumanAPI(parser);
            api.UserInput(args);

            var error = api.NextErrorLine();
            while (error != null)
            {
                Console.Error.WriteLine($"💢 {error}");
                error = api.NextErrorLine();
            }
        }
    }
}
