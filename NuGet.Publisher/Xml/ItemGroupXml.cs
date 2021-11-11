/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the GNU GENERAL PUBLIC LICENSE Version 2. See GNU-GPL.txt in the project root for license information.
 */

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