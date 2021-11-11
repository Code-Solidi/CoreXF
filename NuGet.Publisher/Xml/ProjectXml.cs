/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the GNU GENERAL PUBLIC LICENSE Version 2. See GNU-GPL.txt in the project root for license information.
 */

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
