/*
 * Copyright (c) 2016-2020 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */

using static CoreXF.Abstractions.Base.IExtension;

namespace CoreXF.WebApiHost.Models
{
    public class MicroserviceModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public ExtensionStatus Status { get; set; }

        public string Version { get; set; }

        public string Url { get; set; }

        public string Authors { get; set; }

        public string Location { get; set; }
    }
}