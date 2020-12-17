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