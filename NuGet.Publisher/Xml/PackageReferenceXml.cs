using System.Xml.Serialization;

namespace NuGet.Publisher.Xml
{
    [XmlRoot(ElementName = "PackageReference")]
    public class PackageReferenceXml
    {
        [XmlAttribute(AttributeName = "Include")]
        public string Include { get; set; }

        [XmlAttribute(AttributeName = "Version")]
        public string Version { get; set; }

        [XmlElement(ElementName = "PrivateAssets")]
        public string PrivateAssets { get; set; }

        [XmlElement(ElementName = "IncludeAssets")]
        public string IncludeAssets { get; set; }
    }
}