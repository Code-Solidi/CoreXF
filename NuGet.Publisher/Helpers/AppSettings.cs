using Microsoft.Extensions.Configuration;

using NuGet.Versioning;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NuGet.Publisher.Helpers
{
    public enum Verbosity { Terse, Normal, Verbose }

    public enum Source { Solution, Json }

    public class AppSettings
    {
        public FileInfo Solution { get; set; }

        private NuGetVersion version;

        public NuGetVersion Version
        {
            get => this.version;
            set
            {
                this.version = value;
                this.TempPath = Path.Combine(this.TempPath, $"v{this.version.ToNormalizedString()}");
            }
        }

        public string NuGetUri { get; set; }

        public string ApiKey { get; set; }

        public bool PreBuild { get; set; }

        public Verbosity Verbosity { get; set; }

        public IEnumerable<string> Exclude { get; set; }

        public string TempPath { get; set; }

        public static AppSettings Load(IConfigurationRoot config)
        {
            var settings = new AppSettings();
            config.GetSection("Settings").Bind(settings);
            return settings;
        }
    }
}