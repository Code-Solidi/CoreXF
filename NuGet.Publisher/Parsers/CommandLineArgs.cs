using NuGet.Versioning;

using System;
using System.IO;
using System.Linq;

namespace NuGet.Publisher.Parsers
{
    public class CommandLineArgs
    {
        private FileInfo solution;

        public string NuGetVersion { private get; set; }

        public FileInfo Solution
        {
            get => this.solution;

            set
            {
                this.solution = null;

                var fullName = value.FullName;
                if (Path.EndsInDirectorySeparator(fullName))
                {
                    fullName = fullName.Remove(fullName.Length - 1);
                }

                var path = Path.GetFileName(fullName);
                if (string.IsNullOrWhiteSpace(path))
                {
                    path = Directory.GetFiles(path, ".sln", SearchOption.TopDirectoryOnly).FirstOrDefault();
                }

                if (string.IsNullOrWhiteSpace(path) == false)
                {
                    if (path.EndsWith(".sln", StringComparison.OrdinalIgnoreCase) == false)
                    {
                        path = $"{path}.sln";
                    }

                    var dir = Path.GetDirectoryName(fullName);
                    var index = dir.LastIndexOf(@"bin\Debug");
                    if (index != -1)
                    {
                        dir = dir.Remove(index);
                    }

                    this.solution = new FileInfo(Path.Combine(dir, path));
                    if (this.solution.Exists == false)
                    {
                        // test parent folder for the solution, if does not exist we give up
                        dir = Directory.GetParent(dir)?.Parent.FullName;
                        this.solution = new FileInfo(Path.Combine(dir, path));
                    }
                }
            }
        }

        public bool SolutionExists => this.Solution?.Exists ?? false;

        public NuGetVersion Version => new NuGetVersion(this.NuGetVersion ?? "0");

        public string Help { get; internal set; }
    }
}