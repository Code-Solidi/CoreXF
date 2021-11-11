/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the GNU GENERAL PUBLIC LICENSE Version 2. See GNU-GPL.txt in the project root for license information.
 */

using Microsoft.Extensions.Configuration;

using NuGet.Publisher.Domain;
using NuGet.Publisher.Helpers;
using NuGet.Publisher.Parsers;

using Serilog;
using Serilog.Events;

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace NuGet.Publisher
{
    internal static partial class Program
    {
        private static ILogger logger;
        private static AppSettings settings;

        private static void Main(string[] args)
        {
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            Program.logger = new LoggerConfiguration().ReadFrom.Configuration(config).CreateLogger();

            var solutionInfo = default(Solution.SolutionInfo);

            Program.Initialize(args, config);
            try
            {
                solutionInfo = Solution.Load(Program.settings.Solution.FullName, Program.settings.Exclude, Program.settings, logger);
            }
            catch (Exception)
            {
                Program.Exit("Error loading solution. Exiting...", LogEventLevel.Error);
            }

            var count = Solution.Analize(solutionInfo);
            if (count != 0)
            {
                Program.ReportIssues(solutionInfo);
                Program.Exit($"{count} errors encountered analyzing solution. Execution halted.", LogEventLevel.Error);
            }

            var projects = Solution.BuildDependencyGraph(solutionInfo, Program.settings);
            if (Program.settings.Verbosity != Verbosity.Terse)
            {
                Program.PrintDependencyGraph(projects);
            }

            if (Program.settings.PreBuild)
            {
                Program.Build(projects);
            }

            var processed = Program.PackAndPush(projects);
            if (projects.Count() != 0)
            {
                Program.Rollback(processed);
            }
            else
            {
                Program.Commit();
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private static void Initialize(string[] args, IConfigurationRoot config)
        {
            static CommandLineArgs ParseCommandLine(string[] args)
            {
                var result = default(CommandLineArgs);
                var rootCommand = new RootCommand
                {
                    new Option<string>("--nuget-version", "NuGet version of the packages"),
                    new Option<FileInfo>("--solution", "Path to the solution (you can leave blank if provided in appsettings.json).")
                };

                //rootCommand.Description = Environment.NewLine;

                rootCommand.Handler = CommandHandler.Create<CommandLineArgs, IConsole>((settings, console) =>
                {
                    result = settings;
                    result.Help = rootCommand.GetHelp();
                });

                return rootCommand.InvokeAsync(args).Result == 0 ? result : default;
            }

            settings = AppSettings.Load(config);
            var commandLineArgs = ParseCommandLine(args);

            // NB: command line parameters have precedence
            settings.Solution = commandLineArgs.Solution ?? settings.Solution;
            settings.Version = commandLineArgs.Version ?? settings.Version;

            if (settings.Version == default || settings.Solution == default || settings.Solution.Exists == false)
            {
                Program.Exit(commandLineArgs.Help, LogEventLevel.Information);
            }

            Program.logger.Information("--------------------");
            Program.logger.Information("NEW SESSION STARTED.");
            Program.logger.Information("--------------------");
            Program.logger.Information($"Publishing projects in '{settings.Solution.Name}' v{settings.Version} to {settings.NuGetUri}.");
        }

        private static void ReportIssues(Solution.SolutionInfo solutionInfo)
        {
            if (solutionInfo.Issues.Count != 0)
            {
                foreach (var issue in solutionInfo.Issues)
                {
                    Program.logger.Error($"{issue.Project.Name}: [{issue.Kind}] {issue.Message}");
                }

                Program.Exit(new StringBuilder()
                    .AppendLine("------------------")
                    .Append("Total issues: (")
                    .Append(solutionInfo.Issues.Count)
                    .AppendLine(")")
                    .AppendLine("Please, fix the issues and try again.")
                    .ToString(), LogEventLevel.Error);
            }
        }

        private static void PrintDependencyGraph(IEnumerable<Project> resolutionGraph)
        {
            Program.logger.Information(string.Empty);
            Program.logger.Information("Dependency Graph:");
            Program.logger.Information(new string('-', "Dependency Graph:".Length));
            foreach (var project in resolutionGraph)
            {
                if (Program.settings.Verbosity == Verbosity.Verbose)
                {
                    Program.logger.Information($"{project.Name} ({project.Path}), dependencies ({project.PackageDependencies.Count}): ");
                }
                else
                {
                    Program.logger.Information($"{project.Name} ({project.PackageDependencies.Count}): ");
                }

                foreach (var package in project.PackageDependencies)
                {
                    Program.logger.Information($"{new string(' ', 4)}{package.Name} v{settings.Version}");
                }

                Program.logger.Information(string.Empty);
            }
        }

        private static void Build(IEnumerable<Project> resolutionGraph)
        {
            foreach (var project in resolutionGraph)
            {
                Program.logger.Information(string.Empty);
                var text = $"Building '{project.Name}'";
                Program.logger.Information(text);
                Program.logger.Information(new string('-', text.Length));
                var args = $"dotnet msbuild -restore {project.Path} /p:Development='false'";
                if (Program.Exec(args))
                {
                    Program.Exit($"Errors building {project.Name}. Execution halted.", LogEventLevel.Error);
                }
            }
        }

        private static Stack<Project> PackAndPush(ICollection<Project> projects)
        {
            var processed = new Stack<Project>();
            foreach (var project in projects.ToArray())
            {
                //projects.Remove(project);
                //processed.Push(project);

                var failed = Program.Pack(project) == false;
                if (failed == false)
                {
                    failed = Program.Push(project) == false;
                }

                Program.RevertToProjectReferences(project);

                if (failed == false)
                {
                    projects.Remove(project);
                    processed.Push(project);
                }
                else
                {
                    break;
                }
            }

            return processed;
        }

        private static bool Pack(Project project)
        {
            var text = $"Packing '{project.Name}'";
            Program.logger.Information(text);
            Program.logger.Information(new string('-', text.Length));

            var result = true;
            var fileInfo = new FileInfo(settings.TempPath).FullName;
            var args = $"dotnet pack {project.Path} -o {fileInfo} /p:Development='false'";
            if (Program.Exec(args))
            {
                logger.Error($"Error packing '{project.Name}'.");
                result = false;
            }

            Program.logger.Information(string.Empty);
            return result;
        }

        private static bool Push(Project project)
        {
            var text = $"Pushing '{project.Name}'";
            Program.logger.Information(text);
            Program.logger.Information(new string('-', text.Length));

            var result = true;
            var fileInfo = new FileInfo($"{settings.TempPath}\\{project.Name}.{settings.Version}.nupkg");
            var args = $"dotnet nuget push '{fileInfo.FullName}' -k:{settings.ApiKey} -s:{settings.NuGetUri} --skip-duplicate";
            if (Program.Exec(args))
            {
                logger.Error($"Errors pushing '{project.Name}' to NuGet server.");
                result = false;
            }

            Program.logger.Information(string.Empty);
            return result;
        }

        private static void RevertToProjectReferences(Project project)
        {
            var lines = File.ReadAllLines($"{project.Path}.bak");
            var outLines = new List<string>();
            foreach (var line in lines)
            {
                var outLine = line;
                if (line.Contains("<Version>"))
                {
                    var index = line.IndexOf("<Version>");
                    outLine = $"{line.Substring(0, index)}<Version>{settings.Version}</Version>";
                }

                outLines.Add(outLine);
            }

            //var backup = $"{project.Path}.bak";
            //File.Delete(backup);
            //Directory.Move(project.Path, backup);
            File.WriteAllLines(project.Path, outLines);
        }

        private static void Commit()
        {
            Console.Write("Do you want to commit changes (y/n)? ");
            var key = Console.ReadKey();
            Console.WriteLine();
            if (char.ToLower(key.KeyChar) == 'y')
            {
                var message = $"Committed new NuGet package version {settings.Version}.";
                var args = $"git commit -a -m '{message}'";
                if (Program.Exec(args))
                {
                    logger.Error("Error committing changes.");
                }
            }
        }

        private static void Rollback(Stack<Project> processed)
        {
            while (processed.Count != 0)
            {
                var project = processed.Pop();
                Program.logger.Information(string.Empty);
                Program.logger.Information($"Rolling back changes to '{project.Name}'.");

                var args = $"dotnet nuget delete {project.Name} {settings.Version} -k {settings.ApiKey} -s {settings.NuGetUri}";
                Program.Exec(args, "y");

                var backup = $"{project.Path}.bak";
                var delete = $"{project.Path}.delete";
                Program.logger.Information($"Reverting '{backup}' back to '{project.Path}'.");

                Directory.Move(project.Path, delete);
                Directory.Move(backup, project.Path);

                File.Delete(delete);
                File.Delete(backup);
            }
        }

        private static bool Exec(string arguments, string input = null)
        {
            var skippedErrors = new string[]
            {
                "the build result will not be impacted."
            };

            var startInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = arguments,
                RedirectStandardInput = input != null,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = new System.Diagnostics.Process { StartInfo = startInfo })
            {
                process.Start();

                if (input != null)
                {
                    var writer = process.StandardInput;
                    writer.WriteLine(input);
                }

                var errors = false;
                var output = process.StandardOutput.ReadLine();
                while (output != null)
                {
                    if (output.Contains(" error ", StringComparison.OrdinalIgnoreCase))
                    {
                        // NB: this is a workaround. a better solution is required!!
                        if (skippedErrors.Any(x => output.Contains(x, StringComparison.OrdinalIgnoreCase) == false))
                        {
                            Program.logger.Error(output);
                            errors = true;
                        }
                    }
                    else if (output.Contains(" warning ", StringComparison.OrdinalIgnoreCase))
                    {
                        if (settings.Verbosity != Verbosity.Terse)
                        {
                            Program.logger.Warning(output);
                        }
                    }
                    else
                    {
                        Program.logger.Information(output);
                    }

                    output = process.StandardOutput.ReadLine();
                }

                var error = process.StandardError.ReadLine();
                while (error != null)
                {
                    errors = true;
                    Program.logger.Error(error);
                    error = process.StandardError.ReadLine();
                }

                process.WaitForExit(5000);

                return errors;
            }
        }

        private static void Exit(string message, LogEventLevel eventLevel, int result = -1)
        {
            switch (eventLevel)
            {
                case LogEventLevel.Debug:
                    Program.logger.Debug(message);
                    break;

                case LogEventLevel.Verbose:
                    Program.logger.Verbose(message);
                    break;

                case LogEventLevel.Information:
                    Program.logger.Information(message);
                    break;

                case LogEventLevel.Warning:
                    Program.logger.Warning(message);
                    break;

                case LogEventLevel.Error:
                    Program.logger.Error(message);
                    break;

                case LogEventLevel.Fatal:
                    Program.logger.Fatal(message);
                    break;
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            Environment.Exit(result);
        }
    }
}