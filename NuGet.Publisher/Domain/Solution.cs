using NuGet.Publisher.Helpers;
using NuGet.Publisher.Parsers;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using static NuGet.Publisher.Domain.Solution.SolutionInfo;

namespace NuGet.Publisher.Domain
{
    internal static class Solution
    {
        internal static SolutionInfo LoadFromFile(string pathToSolutionFile)
        {
            if (new FileInfo(pathToSolutionFile).Exists == false)
            {
                Console.WriteLine($"Cannot find solution file {pathToSolutionFile}");
                return new SolutionInfo(new string[0]);
            }

            var solutionFileLines = File.ReadAllLines(pathToSolutionFile).ToArray();
            var projectReferenceRegEx = new Regex("Project\\(\"([\\{\\}0-9A-Z\\-]+)\"\\) = \"(.*)\", \"(.*.csproj)\", \"([\\{\\}0-9A-Z\\-]+)\"\\s*");
            var projectRelativePaths = solutionFileLines
                .Select(line => projectReferenceRegEx.Match(line))
                .Where(x => x.Success)
                .Select(x => x.Groups[3].ToString());
            var projectPaths = projectRelativePaths.Select(x => Path.Combine(Path.GetDirectoryName(pathToSolutionFile), x));

            return new SolutionInfo(projectPaths.ToArray());
        }

        internal static SolutionInfo Load(string solution, IEnumerable<string> exclude, AppSettings settings, Serilog.ILogger logger)
        {
            static bool Include(string path, IEnumerable<string> exclude)
            {
                var projectName = Path.GetFileNameWithoutExtension(path);
                return exclude?.Any(x => x.Equals(projectName, StringComparison.OrdinalIgnoreCase)) != false;
            }

            static bool ConvertProjectToPackageReferences(Project project, AppSettings settings)
            {
                var lines = File.ReadAllLines(project.Path);
                var outLines = new List<string>();
                var hasVersion = false;
                foreach (var line in lines)
                {
                    var outLine = line;
                    if (line.Contains("<Version>"))
                    {
                        var index = line.IndexOf("<Version>");
                        outLine = $"{line.Substring(0, index)}<Version>{settings.Version}</Version>";
                        hasVersion = true;
                    }

                    if (hasVersion && line.Contains("<ProjectReference"))
                    {
                        var index = line.IndexOf("<ProjectReference");
                        var include = line.IndexOf("Include=\"") + "Include=\"".Length;
                        var projectPath = line.Substring(include, line.LastIndexOf('"') - include);
                        var package = Path.GetFileNameWithoutExtension(projectPath);
                        outLine = $"{line.Substring(0, index)}<PackageReference Include=\"{package}\" Version=\"{settings.Version}\" />";
                    }

                    outLines.Add(outLine);
                }

                var backup = $"{project.Path}.bak";
                if (File.Exists(backup))
                {
                    File.Delete(backup);
                }

                Directory.Move(project.Path, backup);
                File.WriteAllLines(project.Path, outLines);

                return hasVersion;
            }

            var solutionInfo = Solution.LoadFromFile(solution);
            foreach (var path in solutionInfo.ProjectFilePaths)
            {
                if (Include(path, exclude))
                {
                    try
                    {
                        var project = ProjectParser.Parse(path, Path.GetFileNameWithoutExtension(solution));
                        if (ConvertProjectToPackageReferences(project, settings))
                        {
                            solutionInfo.Add(project);
                        }
                    }
                    catch (Exception x)
                    {
                        logger.Error($"Error parsing: '{path}'. {x.Message}{Environment.NewLine}{x.StackTrace}");
                        throw;
                    }
                }
            }

            return solutionInfo;
        }

        internal static ICollection<Project> BuildDependencyGraph(SolutionInfo solution, AppSettings settings)
        {
            var projects = solution.Projects.Clone();
            var resolved = default(IEnumerable<Project>);
            var result = new List<Project>();

            do
            {
                resolved = projects.Where(x => x.ProjectDependencies.Count == 0).ToList();
                result.AddRange(resolved);
                projects.RemoveAll(x => resolved.Contains(x));

                foreach (var project in resolved)
                {
                    var dependents = projects.Where(x => x.ProjectDependencies.Any(p => p.Name == project.Name)).ToList();
                    if (dependents.Any())
                    {
                        dependents.ReplaceWithPackage(project, "1.2.3");
                    }
                }
            }
            while (resolved.Any());

            return result;
        }

        internal static int Analize(SolutionInfo solution)
        {
            foreach (var project in solution.Projects)
            {
                foreach (var projectDep in project.ProjectDependencies)
                {
                    var duplicate = project.PackageDependencies.SingleOrDefault(x => x.Name == projectDep.Name);
                    if (duplicate != default)
                    {
                        var issue = new Issue
                        {
                            Kind = Issue.IssueKind.Warning,
                            Project = project,
                            Message = $"Found dependency duplicate between project {projectDep.Name} and package {duplicate.Name} v{duplicate.Version}."
                        };

                        solution.Issues.Add(issue);
                    }
                }
            }

            return solution.Issues.Count;
        }

        public class SolutionInfo
        {
            private readonly string[] paths;

            public SolutionInfo(string[] paths)
            {
                this.IsParsed = true;
                this.paths = paths;
                this.Projects = new List<Project>();
                this.Issues = new List<Issue>();
            }

            public IEnumerable<string> ProjectFilePaths => this.paths.AsEnumerable();

            internal IList<Project> Projects { get; }

            public bool IsParsed { get; }

            public IList<Issue> Issues { get; private set; }

            internal void Add(Project project)
            {
                this.Projects.Add(project);
            }

            public class Issue
            {
                public enum IssueKind { Warning, Error }

                public IssueKind Kind { get; set; }

                public Project Project { get; set; }

                public string Message { get; set; }
            }
        }
    }
}