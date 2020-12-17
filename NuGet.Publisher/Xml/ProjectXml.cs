using System.Xml.Serialization;
using System.Collections.Generic;

namespace NuGet.Publisher.Xml
{
    [XmlRoot(ElementName = "Project")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CC0021:Use nameof", Justification = "<Pending>")]
    public class ProjectXml
    {
        [XmlElement(ElementName = "PropertyGroup")]
        public List<PropertyGroupXml> PropertyGroups { get; set; }

        [XmlElement(ElementName = "ItemGroup")]
        public List<ItemGroupXml> ItemGroups { get; set; }

        //[XmlElement(ElementName = "Target")]
        //public List<TargetXml> Targets { get; set; }

        [XmlAttribute(AttributeName = "Sdk")]
        public string Sdk { get; set; }
    }
}
