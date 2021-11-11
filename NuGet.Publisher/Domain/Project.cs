/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the GNU GENERAL PUBLIC LICENSE Version 2. See GNU-GPL.txt in the project root for license information.
 */

using NuGet.Publisher.Xml;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace NuGet.Publisher.Domain
{
    public class Project
    {
        public Project(string path, string solutionName, ProjectXml projectXml)
        {
            this.Name = System.IO.Path.GetFileNameWithoutExtension(path);
            this.Path = System.IO.Path.GetFullPath(path);
            this.SetPackageRefererences(projectXml, solutionName);
            this.SetProjectRefererences(projectXml, solutionName);
            //this.DisablePostBuildTargets(projectXml, solutionName);
        }

        public string Name { get; }

        public List<PackageReference> PackageDependencies { get; set; }

        [JsonIgnore]
        public string Path { get; }

        [JsonIgnore]
        public List<ProjectReference> ProjectDependencies { get; set; }

        //[JsonIgnore]
        //public List<PostBuildTarget> BuildTargets { get; set; }

        public override string ToString() => this.Name;

        private void SetPackageRefererences(ProjectXml projectXml, string solutionName)
        {
            this.PackageDependencies = new List<PackageReference>();
            foreach (var package in projectXml.ItemGroups.Where(x => x.PackageReference.Count != 0).Select(x => x.PackageReference))
            {
                foreach (var candidate in package.Where(x => x.Include.StartsWith(/*"CoreXF"*/solutionName)).Select(x => new PackageReference(x)))
                {
                    var found = this.PackageDependencies.SingleOrDefault(x => x.Name == candidate.Name);
                    if (found == default)
                    {
                        this.PackageDependencies.Add(candidate);
                    }
                }
            }
        }

        private void SetProjectRefererences(ProjectXml projectXml, string solutionName)
        {
            this.ProjectDependencies = new List<ProjectReference>();
            foreach (var project in projectXml.ItemGroups.Where(x => x.ProjectReference.Count != 0).Select(x => x.ProjectReference))
            {
                foreach (var candidate in project
                    .Where(x => System.IO.Path.GetFileName(x.Include).StartsWith(/*"CoreXF"*/solutionName))
                    .Select(x => new ProjectReference(x)))
                {
                    var found = this.ProjectDependencies.SingleOrDefault(x => x.Name == candidate.Name);
                    if (found == default)
                    {
                        this.ProjectDependencies.Add(candidate);
                    }
                }
            }
        }

        //private void DisablePostBuildTargets(ProjectXml projectXml, string solutionName)
        //{
        //    this.BuildTargets = new List<PostBuildTarget>();
        //    foreach (var target in projectXml.Targets)
        //    {
        //        this.BuildTargets.Add(new PostBuildTarget
        //        {
        //            Command = target.Exec.Command,
        //            Name = target.Name,
        //            AfterTargets = target.AfterTargets,
        //            Condiion = target.Condition
        //        });
        //    }
        //}

        public class PackageReference
        {
            public PackageReference()
            {
            }

            public PackageReference(PackageReferenceXml packageReference)
            {
                this.Name = packageReference.Include;
                this.Version = packageReference.Version;
            }

            public string Name { get; set; }

            public string Version { get; set; }

            public override string ToString() => $"{this.Name}, v{this.Version}";
        }

        public class ProjectReference
        {
            public ProjectReference(ProjectReferenceXml projectReference)
            {
                this.Name = System.IO.Path.GetFileNameWithoutExtension(projectReference.Include);
                this.Path = System.IO.Path.GetFullPath(projectReference.Include);
            }

            public string Name { get; set; }

            public string Path { get; set; }

            public override string ToString() => this.Name;
        }

        //public class PostBuildTarget
        //{
        //    public string Command { get; internal set; }

        //    public string Name { get; internal set; }

        //    public string AfterTargets { get; internal set; }

        //    public string Condiion { get; internal set; }

        //    public override string ToString() => this.Name;
        //}
    }
}