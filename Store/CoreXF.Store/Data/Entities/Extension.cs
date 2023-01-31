using NuGet.Versioning;

namespace CoreXF.Store.Data.Entities
{
    public class Extension : Product
    {
        public AppUser Owner { get; set; }

        public string Overview { get => this.Description; set => this.Description = value; }

        public decimal Rating { get; set; }

        public string ProjectUrl { get; set; }

        public string Version { get; set; } // Semantic Version

        public uint Downloads { get; set; }

        public SemanticVersion SemanticVersion
        {
            get
            {
                if (SemanticVersion.TryParse(this.Version, out var version))
                {
                    return version;
                }

                return new SemanticVersion(0, 0, 0);
            }
        }
    }
}