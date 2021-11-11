/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the GNU GENERAL PUBLIC LICENSE Version 2. See GNU-GPL.txt in the project root for license information.
 */

using NuGet.Publisher.Domain;

using System;
using System.Collections.Generic;
using System.Linq;

namespace NuGet.Publisher.Helpers
{
    internal static class Extensions
    {
        public static void ReplaceWithPackage(this IList<Project> list, Project project, string version)
        {
            foreach (var item in list)
            {
                var projectDep = item.ProjectDependencies.Single(x => x.Name == project.Name);
                item.ProjectDependencies.Remove(projectDep);
                item.PackageDependencies.Add(new Project.PackageReference { Name = project.Name, Version = version });
            }
        }

        public static List<T> Clone<T>(this IList<T> list)
        {
            var temp = new T[list.Count];
            list.CopyTo(temp, 0);
            return new List<T>(temp);
        }

        public static T Value<T>(this string name)
        {
            name = Enum.GetNames(typeof(T)).ToArray().Single(x => x.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (Enum.TryParse(typeof(T), name, out var value))
            {
                return (T)value;
            }

            return default;
        }
    }
}