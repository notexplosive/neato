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

            parser.RegisterCommand("monogame-release-build-zip")
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
                    var buildOutputFiles = new FileManager(PathType.Absolute, Path.Join(outputDirectory, "build-" + Guid.NewGuid()));
                    var dotnetResult = dotnet.NormalPublish(localFiles.WorkingDirectory, buildOutputFiles);
                    var sevenZipResult = sevenZip.SendToZip(buildOutputFiles.WorkingDirectory, outputDirectory, zipName);

                    buildOutputFiles.RemoveDirectoryRecursive(new PathContext(PathType.Relative, "."));
                });

            parser.RegisterCommand("monogame-release-build")
                .AddParameter(Parameter.String("path to csproj"))
                .AddParameter(Parameter.String("destination"))
                .OnExecuted((parameters) =>
                {
                    var dotnet = new DotnetProgram();
                    var sevenZip = new SevenZipProgram();
                    var localFiles = new FileManager(PathType.Relative, parameters[0].AsString());
                    var outputDirectory = parameters[1].AsString();

                    Logger.Info($"Publishing to {outputDirectory}");
                    var dotnetResult = dotnet.NormalPublish(localFiles.WorkingDirectory, new FileManager(PathType.Absolute, outputDirectory));
                });

            parser.RegisterCommand("login-itch")
                .OnExecuted((parameters) =>
                {
                    new ButlerProgram().Login();
                });

            parser.RegisterCommand("login-steam")
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

            parser.RegisterCommand("publish-all")
                .AddParameter(Parameter.String("path that contains csproj"))
                .AddParameter(Parameter.String("steam username"))
                .AddParameter(Parameter.String("itch username"))
                .AddParameter(Parameter.String("itch game url"))
                .AddParameter(Parameter.String("itch channel"))
                .OnExecuted((parameters) =>
                {
                    var pathToCsproj = parameters[0].AsString();
                    var steamUsername = parameters[1].AsString();
                    var itchUsername = parameters[2].AsString();
                    var gameUrl = parameters[3].AsString();
                    var channel = parameters[4].AsString();

                    var vdfFile = GetOnlyVdfFileInDirectory(Directory.GetCurrentDirectory());

                    if (!string.IsNullOrEmpty(vdfFile))
                    {
                        Logger.Info($"About to build and then upload {Path.GetFullPath(pathToCsproj)}");
                        Logger.Info($"\tto itch.io at {itchUsername.ToUpper()}/{gameUrl}:{channel}");
                        Logger.Info($"\tto steam as {steamUsername.ToUpper()} using {Path.GetFileName(vdfFile)}");
                        Logger.Warning($"Does the above look correct? [y/N]");
                        var answer = Console.ReadKey();

                        if (answer.Key == ConsoleKey.Y)
                        {
                            var dotnet = new DotnetProgram();
                            var localFiles = new FileManager(PathType.Relative, parameters[0].AsString());
                            var outputDirectoryLocal = $"build-temp-{Guid.NewGuid()}";
                            localFiles.MakeDirectory(new PathContext(PathType.Relative, outputDirectoryLocal));

                            Logger.Info($"Building to {outputDirectoryLocal}");
                            dotnet.NormalPublish(pathToCsproj, new FileManager(PathType.Absolute, outputDirectoryLocal));

                            Logger.Info($"Removing .pdb files");
                            localFiles.RemoveFiles(new PathContext(PathType.Relative, outputDirectoryLocal), "*.pdb");

                            Logger.Info("Logging into itch.io");
                            new ButlerProgram().Login();

                            Logger.Info("Deploying to itch");
                            DeployToItch(outputDirectoryLocal, itchUsername, gameUrl, channel);
                            Logger.Info("Deploying to steam");
                            DeployToSteam(steamUsername, vdfFile);
                            Logger.Info("Cleaning up");
                            localFiles.RemoveDirectoryRecursive(new PathContext(PathType.Relative, outputDirectoryLocal));
                        }
                    }
                    else
                    {
                        Logger.Warning("Cannot deploy to steam without a .vdf file in the current directory");
                    }
                });

            parser.RegisterCommand("find-vdf")
                .OnExecuted((parameters) =>
                {
                    var files = GetAllVdfsInDirectory(Directory.GetCurrentDirectory());

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

            parser.RegisterCommand("publish-steam")
                .AddParameter(Parameter.String("username"))
                .AddParameter(Parameter.String("directory"))
                .OnExecuted((parameters) =>
                {
                    var directory = parameters[0].AsString();
                    var username = parameters[1].AsString();
                    var vdfFile = GetOnlyVdfFileInDirectory(Directory.GetCurrentDirectory());

                    if (!string.IsNullOrEmpty(vdfFile))
                    {
                        DeployToSteam(username, vdfFile);
                    }
                });

            parser.RegisterCommand("publish-itch")
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
                    DeployToItch(directoryToUpload, itchUrl, gameUrl, channel);
                })
                ;

            parser.RegisterCommand("new-project")
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
                Logger.Error(error);
                error = api.NextErrorLine();
            }
        }

        private static void DeployToItch(string directory, string itchUsername, string gameUrl, string channel)
        {
            var butler = new ButlerProgram();
            butler.Push(directory, itchUsername, gameUrl, channel);
        }

        private static void DeployToSteam(string username, string vdfFile)
        {
            var steamCmd = new SteamCmdProgram();
            steamCmd.Deploy(username, vdfFile);
            Logger.Warning("You still need to set this as the default build in Steamworks");
        }

        public static string GetOnlyVdfFileInDirectory(string directory)
        {
            var files = GetAllVdfsInDirectory(directory);

            if (files.Count == 1)
            {
                var vdfFile = files[0];
                Logger.Info($"Found vdf: {vdfFile}");
                return vdfFile;
            }
            else if (files.Count == 0)
            {
                Logger.Error("No .vdf files found");
                return null;
            }
            else
            {
                Logger.Error("Found multiple .vdf files, unsure which one to use");
                return null;
            }
        }

        public static List<string> GetAllVdfsInDirectory(string directory)
        {
            var enumerated = Directory.EnumerateFiles(directory, "*.vdf");

            var files = new List<string>();

            foreach (var en in enumerated)
            {
                files.Add(en);
            }

            return files;
        }
    }
}
