/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the GNU GENERAL PUBLIC LICENSE Version 2. See GNU-GPL.txt in the project root for license information.
 */

using System;
using System.IO;
using System.Xml.Serialization;

using NuGet.Publisher.Domain;
using NuGet.Publisher.Xml;

namespace NuGet.Publisher.Parsers
{
    public static class ProjectParser
    {
        public static Project Parse(string path, string solutionName)
        {
            var projectXml = default(ProjectXml);
            var serializer = new XmlSerializer(typeof(ProjectXml));
            try
            {
                using (var reader = new FileStream(path, FileMode.Open))
                {
                    projectXml = (ProjectXml)serializer.Deserialize(reader);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return new Project(path, solutionName, projectXml);
        }
    }
}