/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the GNU GENERAL PUBLIC LICENSE Version 2. See GNU-GPL.txt in the project root for license information.
 */

using System.Xml.Serialization;

namespace NuGet.Publisher.Xml
{
    [XmlRoot(ElementName = "PropertyGroup")]
    public class PropertyGroupXml
    {
        [XmlElement(ElementName = "TargetFramework")]
        public string TargetFramework { get; set; }

        [XmlElement(ElementName = "Version")]
        public string Version { get; set; }

        [XmlElement(ElementName = "PublishRepositoryUrl")]
        public string PublishRepositoryUrl { get; set; }

        [XmlElement(ElementName = "EmbedUntrackedSources")]
        public string EmbedUntrackedSources { get; set; }

        [XmlElement(ElementName = "IncludeSymbols")]
        public string IncludeSymbols { get; set; }

        [XmlElement(ElementName = "SymbolPackageFormat")]
        public string SymbolPackageFormat { get; set; }

        [XmlElement(ElementName = "RepositoryType")]
        public string RepositoryType { get; set; }

        [XmlElement(ElementName = "RepositoryUrl")]
        public string RepositoryUrl { get; set; }

        [XmlElement(ElementName = "Company")]
        public string Company { get; set; }

        [XmlElement(ElementName = "Authors")]
        public string Authors { get; set; }

        [XmlElement(ElementName = "PackageProjectUrl")]
        public string PackageProjectUrl { get; set; }

        [XmlElement(ElementName = "GeneratePackageOnBuild")]
        public string GeneratePackageOnBuild { get; set; }

        [XmlElement(ElementName = "PackageIcon")]
        public string PackageIcon { get; set; }

        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }
    }
}