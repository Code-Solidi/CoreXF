using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace PartsWebApp.Models
{
    public class FeaturesViewModel
    {
        public List<System.Reflection.TypeInfo> Controllers { get; internal set; }

        public List<MetadataReference> MetadataReferences { get; internal set; }

        public List<System.Reflection.TypeInfo> TagHelpers { get; internal set; }

        public List<System.Reflection.TypeInfo> ViewComponents { get; internal set; }
    }
}