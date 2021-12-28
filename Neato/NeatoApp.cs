using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neato
{
    public class NeatoApp
    {
        private CommandLineParser Parser { get; } = new();
        private CommandLineHumanAPI API { get; }
        private Logger Logger { get; }

        public NeatoApp(Logger logger)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Logger = logger;
            API = new CommandLineHumanAPI(Parser);

            Parser.RegisterCommand("monogame-release-build-zip")
                .AddParameter(Parameter.String("path to csproj"))
                .AddParameter(Parameter.String("destination"))
                .AddParameter(Parameter.String("zip name"))
                .OnExecuted((parameters) =>
                {
                    var dotnet = new DotnetProgram(Logger);
                    var sevenZip = new SevenZipProgram(Logger);
                    var localFiles = new FileManager(PathType.Relative, parameters[0].AsString());
                    var outputDirectory = parameters[1].AsString();
                    var zipName = parameters[2].AsString();

                    Logger.Info("Packaging as zip");
                    var buildOutputFiles = new FileManager(PathType.Absolute, Path.Join(outputDirectory, "build-" + Guid.NewGuid()));
                    var dotnetResult = dotnet.NormalPublish(localFiles.WorkingDirectory, buildOutputFiles);
                    var sevenZipResult = sevenZip.SendToZip(buildOutputFiles.WorkingDirectory, outputDirectory, zipName);

                    buildOutputFiles.RemoveDirectoryRecursive(new PathContext(PathType.Relative, "."));
                });

            Parser.RegisterCommand("monogame-release-build")
                .AddParameter(Parameter.String("path to csproj"))
                .AddParameter(Parameter.String("destination"))
                .OnExecuted((parameters) =>
                {
                    var dotnet = new DotnetProgram(Logger);
                    var sevenZip = new SevenZipProgram(Logger);
                    var localFiles = new FileManager(PathType.Relative, parameters[0].AsString());
                    var outputDirectory = parameters[1].AsString();

                    Logger.Info($"Publishing to {outputDirectory}");
                    var dotnetResult = dotnet.NormalPublish(localFiles.WorkingDirectory, new FileManager(PathType.Absolute, outputDirectory));
                });

            Parser.RegisterCommand("combine-screenshots")
                .AddParameter(Parameter.String("screenshot folder"))
                .AddParameter(Parameter.String("destination video"))
                .OnExecuted((parameters) =>
                {
                    var screenshotsFolder = new FileManager(PathType.Absolute, Path.GetFullPath(parameters[0].AsString()));
                    var destinationVideoLocation = Path.GetFullPath(parameters[1].AsString());

                    var ffmpeg = new FfmpegProgram(Logger);

                    screenshotsFolder.WriteToFile(new PathContext(PathType.Relative, "concat.txt"), ffmpeg.GenerateConcatFromPngs(screenshotsFolder));

                    ffmpeg.RunWithArgsAt(screenshotsFolder.WorkingDirectory, OutputLevel.Allow,
                        "-f", "concat",
                        "-i", "concat.txt",
                        "-preset", "slow",
                        "-vf", "scale=1280:720",
                        "-vsync", "vfr",
                        "-pix_fmt", "yuv420p",
                        "-crf", "18",
                        destinationVideoLocation);
                });

            Parser.RegisterCommand("login-itch")
                .OnExecuted((parameters) =>
                {
                    new ButlerProgram(Logger).Login();
                });

            Parser.RegisterCommand("login-steam")
                .AddParameter(Parameter.String("username"))
                .OnExecuted((parameters) =>
                {
                    Logger.Info($"Logging into steam as {parameters[0].AsString()}");
                    new SteamCmdProgram(Logger).Login(parameters[0].AsString());
                });

            Parser.RegisterCommand("status")
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

                    LogInstallStatus("butler", () => new ButlerProgram(Logger).Exists());
                    LogInstallStatus("git", () => new GitProgram(".", Logger).Exists());
                    LogInstallStatus("dotnet", () => new DotnetProgram(Logger).Exists());
                    LogInstallStatus("7zip", () => new SevenZipProgram(Logger).Exists());
                    LogInstallStatus("steamcmd", () => new SteamCmdProgram(Logger).Exists());
                });

            Parser.RegisterCommand("publish-all")
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
                            var dotnet = new DotnetProgram(Logger);
                            var localFiles = new FileManager(PathType.Relative, parameters[0].AsString());
                            var outputDirectoryLocal = $"build-temp-{Guid.NewGuid()}";
                            localFiles.MakeDirectory(new PathContext(PathType.Relative, outputDirectoryLocal));

                            Logger.Info($"Building to {outputDirectoryLocal}");
                            dotnet.NormalPublish(pathToCsproj, new FileManager(PathType.Absolute, outputDirectoryLocal));

                            Logger.Info($"Removing .pdb files");
                            localFiles.RemoveFiles(new PathContext(PathType.Relative, outputDirectoryLocal), "*.pdb");

                            Logger.Info("Logging into itch.io");
                            new ButlerProgram(Logger).Login();

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

            Parser.RegisterCommand("find-vdf")
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

            Parser.RegisterCommand("publish-steam")
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

            Parser.RegisterCommand("publish-itch")
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

            Parser.RegisterCommand("new-project")
                .AddParameter(Parameter.String("project name"))
                .OnExecuted((parameters) =>
                {
                    var projectName = parameters[0].AsString();
                    var localFiles = new FileManager(PathType.Relative);

                    var repoPath = Path.Join(localFiles.WorkingDirectory, projectName);
                    var git = new GitProgram(repoPath, Logger);
                    var dotnet = new DotnetProgram(Logger);

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
                    git.Init(outputLevel);

                    Logger.Info("Downloading Machina");
                    git.AddSubmodule(outputLevel, "https://github.com/notexplosive/machina.git");

                    Logger.Info("Installing Monogame Template");
                    dotnet.RunWithArgs(outputLevel, "new", "--install", "MonoGame.Templates.CSharp");
                    Logger.Info("Installing Content Editor tool");
                    dotnet.RunWithArgs(outputLevel, "tool", "install", "--global", "dotnet-mgcb-editor");
                    Logger.Info("Registering Content Editor tool");
                    new ExternalProgram("mgcb-editor", Logger).RunWithArgs(outputLevel, "--register");

                    Logger.Info("Creating Template");
                    dotnet.RunWithArgs(outputLevel, "new", "mgdesktopgl", "-o", projectName);

                    Logger.Info("Creating Solution");
                    dotnet.RunWithArgs(outputLevel, "new", "sln");

                    Logger.Info("Add projects to Solution");
                    dotnet.AddToSln(outputLevel, projectName);
                    var machinaLocalPath = Path.Join(".", "machina", "Machina");
                    dotnet.AddToSln(outputLevel, machinaLocalPath);
                    dotnet.AddToSln(outputLevel, Path.Join(".", "machina", "TestMachina"));
                    dotnet.AddToSln(outputLevel, Path.Join(".", "machina", "MachinaDesktop"));
                    dotnet.AddToSln(outputLevel, Path.Join(".", "machina", "MachinaAndroid"));

                    Logger.Info("Add Machina to Project");
                    dotnet.RunWithArgs(outputLevel, "add", projectName, "reference", machinaLocalPath);

                    Logger.Info("Copying Files");
                    localFiles.Copy(
                        new PathContext(PathType.Relative, Path.Join(projectName, "machina", ".gitignore")),
                        new PathContext(PathType.Relative, Path.Join(projectName, ".gitignore")));
                    localFiles.Copy(
                        new PathContext(PathType.Relative, Path.Join(projectName, "machina", "game-readme.md")),
                        new PathContext(PathType.Relative, Path.Join(projectName, "readme.md")));
                    git.AddAll(outputLevel);
                    git.CommitWithMessage(outputLevel, "(Machina:Automated) Initial Commit");

                    Directory.SetCurrentDirectory(oldWorkingDir);
                    Logger.Info("Done.");
                    Logger.Warning("You still have some manual steps!");
                    Logger.Warning($"\t- Replace the default Game1.cs and Program.cs with the Machina boilerplate");
                    Logger.Warning($"\t- Add MachinaAssets.shproj {Path.Join(".", "machina", "MachinaAssets")} to the sln");
                    Logger.Warning($"\t- Add a shared project reference of MachinaAssets to your game.");
                })
                ;
        }

        public void ConsumeUserInput(params string[] args)
        {
            API.ConsumeUserInput(args);
        }

        public void EmitErrorsIfApplicable()
        {
            var error = API.NextErrorLine();

            while (error != null)
            {
                Logger.Error(error);
                error = API.NextErrorLine();
            }
        }

        private void DeployToItch(string directory, string itchUsername, string gameUrl, string channel)
        {
            var butler = new ButlerProgram(Logger);
            butler.Push(directory, itchUsername, gameUrl, channel);
        }

        private void DeployToSteam(string username, string vdfFile)
        {
            var steamCmd = new SteamCmdProgram(Logger);
            steamCmd.Deploy(username, vdfFile);
            Logger.Warning("You still need to set this as the default build in Steamworks");
        }

        public string GetOnlyVdfFileInDirectory(string directory)
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
