using System.Xml.Serialization;

namespace NuGet.Publisher.Xml
{
    [XmlRoot(ElementName = "ProjectReference")]
    public class ProjectReferenceXml
    {
        [XmlAttribute(AttributeName = "Include")]
        public string Include { get; set; }
    }
}