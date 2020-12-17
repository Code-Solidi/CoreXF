using System.Xml.Serialization;

namespace NuGet.Publisher.Xml
{
    [XmlRoot(ElementName = "Target")]
    public class TargetXml
    {
        [XmlElement(ElementName = "Exec")]
        public Exec Exec { get; set; }

        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "AfterTargets")]
        public string AfterTargets { get; set; }

        [XmlAttribute(AttributeName = "Condition")]
        public string Condition { get; set; }
    }

    [XmlRoot(ElementName = "Exec")]
    public class Exec
    {
        [XmlAttribute(AttributeName = "Command")]
        public string Command { get; set; }
    }
}