using System.Collections.Generic;
using System.Xml.Serialization;

namespace NuGet.Publisher.Xml
{
    [XmlRoot(ElementName = "ItemGroup")]
    public class ItemGroupXml
    {
        [XmlElement(ElementName = "PackageReference")]
        public List<PackageReferenceXml> PackageReference { get; set; }

        [XmlElement(ElementName = "ProjectReference")]
        public List<ProjectReferenceXml> ProjectReference { get; set; }
    }
}