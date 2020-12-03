/*
 * Copyright (c) 2016-2020 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */

namespace HostApp5WebApi.Models
{
    public class MicroserviceModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public MsStatus Status { get; set; }

        public string Version { get; set; }

        public string Url { get; set; }

        public string Authors { get; set; }

        public string Location { get; set; }

        public enum MsStatus
        {
            Ready, Starting, Running, Stopped
        }
    }
}