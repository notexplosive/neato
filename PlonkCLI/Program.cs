using Neato;
using System;
using System.Collections.Generic;
using System.IO;

namespace NeatoCLI
{
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
                });

            parser.RegisterCommand("normal-publish")
                .AddParameter(Parameter.String("path to csproj"))
                .AddParameter(Parameter.String("destination"))
                .OnExecuted((parameters) =>
                {
                    var dotnet = new DotnetProgram();
                    var sevenZip = new SevenZipProgram();
                    var localFiles = new FileManager(PathType.Relative, parameters[0].AsString());
                    var outputDirectory = parameters[1].AsString();

                    Logger.Info($"Publishing to {outputDirectory}, this might take a while");
                    var dotnetResult = dotnet.NormalPublish(localFiles.WorkingDirectory, new FileManager(PathType.Absolute, outputDirectory));
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
                });

            parser.RegisterCommand("itch-login")
                .OnExecuted((parameters) =>
                {
                    new ButlerProgram().Login();
                });

            parser.RegisterCommand("steam-login")
                .AddParameter(Parameter.String("username"))
                .OnExecuted((parameters) =>
                {
                    Logger.Info($"Logging into steam as {parameters[0].AsString()}");
                    new SteamCmdProgram().Login(parameters[0].AsString());
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

                    LogInstallStatus("butler", () => new ButlerProgram().Exists());
                    LogInstallStatus("git", () => new GitProgram(".").Exists());
                    LogInstallStatus("dotnet", () => new DotnetProgram().Exists());
                    LogInstallStatus("7zip", () => new SevenZipProgram().Exists());
                    LogInstallStatus("steamcmd", () => new SteamCmdProgram().Exists());
                });

            parser.RegisterCommand("find-vdf")
                .OnExecuted((parameters) =>
                {
                    var files = GetAllVdfsInDirectory();

                    if (files.Count == 0)
                    {
                        Logger.Warning("No .vdf files found");
                    }
                    else
                    {
                        foreach (var file in files)
                        {
                            Logger.Info(file);
                        }
                    }
                });

            parser.RegisterCommand("deploy-steam")
                .AddParameter(Parameter.String("username"))
                .AddParameter(Parameter.String("directory"))
                .OnExecuted((parameters) =>
                {
                    var directory = parameters[0].AsString();
                    var username = parameters[1].AsString();
                    var files = GetAllVdfsInDirectory();

                    if (files.Count == 1)
                    {
                        var vdfFile = files[0];
                        Logger.Info($"Found vdf: {vdfFile}");
                        var steamCmd = new SteamCmdProgram();

                        var deployResult = steamCmd.Deploy(username, vdfFile);
                    }
                    else if (files.Count == 0)
                    {
                        Logger.Error("No .vdf files found");
                    }
                    else
                    {
                        Logger.Error("Found multiple .vdf files, unsure which one to use");
                    }
                });

            parser.RegisterCommand("deploy-itch")
                .AddParameter(Parameter.String("directory"))
                .AddParameter(Parameter.String("itch account url"))
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
                    var outputLevel = OutputLevel.Suppress;

                    Logger.Info("Creating Repo");
                    git.RunWithArgs(outputLevel, "init");

                    Logger.Info("Downloading Machina");
                    git.RunWithArgs(outputLevel, "submodule", "add", "https://github.com/notexplosive/machina.git");

                    Logger.Info("Installing Monogame Template");
                    dotnet.RunWithArgs(outputLevel, "new", "--install", "MonoGame.Templates.CSharp");
                    Logger.Info("Installing Content Editor tool");
                    dotnet.RunWithArgs(outputLevel, "tool", "install", "--global", "dotnet-mgcb-editor");
                    Logger.Info("Registering Content Editor tool");
                    new ExternalProgram("mgcb-editor").RunWithArgs(outputLevel, "--register");

                    Logger.Info("Creating Template");
                    dotnet.RunWithArgs(outputLevel, "new", "mgdesktopgl", "-o", projectName);

                    Logger.Info("Creating Solution");
                    dotnet.RunWithArgs(outputLevel, "new", "sln");

                    Logger.Info("Add projects to Solution");
                    dotnet.RunWithArgs(outputLevel, "sln", "add", projectName);
                    var machinaLocalPath = Path.Join(".", "machina", "Machina");
                    dotnet.RunWithArgs(outputLevel, "sln", "add", machinaLocalPath);
                    dotnet.RunWithArgs(outputLevel, "sln", "add", Path.Join(".", "machina", "TestMachina"));

                    Logger.Info("Add Machina to Project");
                    dotnet.RunWithArgs(outputLevel, "add", projectName, "reference", machinaLocalPath);

                    Logger.Info("Copying Files");
                    localFiles.Copy(
                        new PathContext(PathType.Relative, Path.Join(projectName, "machina", ".gitignore")),
                        new PathContext(PathType.Relative, Path.Join(projectName, ".gitignore")));
                    localFiles.Copy(
                        new PathContext(PathType.Relative, Path.Join(projectName, "machina", "game-readme.md")),
                        new PathContext(PathType.Relative, Path.Join(projectName, "readme.md")));
                    git.RunWithArgs(outputLevel, "add", ".");
                    git.RunWithArgs(outputLevel, "commit", "-m", "(Machina:Automated) Initial Commit");

                    Directory.SetCurrentDirectory(oldWorkingDir);
                    Logger.Info("Done.");
                    Logger.Warning("You still have some manual steps!");
                    Logger.Warning($"\t- Replace the default Game1.cs and Program.cs with the Machina boilerplate");
                    Logger.Warning($"\t- Add MachinaAssets.shproj {Path.Join(".", "machina", "MachinaAssets")} to the sln");
                    Logger.Warning($"\t- Add a shared project reference of MachinaAssets to your game.");
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

        public static List<string> GetAllVdfsInDirectory()
        {
            var enumerated = Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*.vdf");

            var files = new List<string>();

            foreach (var en in enumerated)
            {
                files.Add(en);
            }

            return files;
        }
    }
}
