/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the GNU GENERAL PUBLIC LICENSE Version 2. See GNU-GPL.txt in the project root for license information.
 */

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