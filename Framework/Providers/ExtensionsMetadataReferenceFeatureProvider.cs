/*
 * Copyright (c) Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;

using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.CodeAnalysis;

namespace CoreXF.Framework.Providers
{
    public class ExtensionsMetadataReferenceFeatureProvider : IApplicationFeatureProvider<MetadataReferenceFeature>
    {
        /// <inheritdoc />
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, MetadataReferenceFeature feature)
        {
            if (parts == null)
            {
                throw new ArgumentNullException(nameof(parts));
            }

            if (feature == null)
            {
                throw new ArgumentNullException(nameof(feature));
            }

            var libraryPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var providerPart in parts.OfType<ICompilationReferencesProvider>())
            {
                var referencePaths = providerPart.GetReferencePaths();

                /* When AssemblyLoadContext loads assembly from stream Location property is "", while assembly is not marked as Dynamic.
                 * The later is checked in MetadataReferenceFeatureProvider so as to avoid adding part to metadata references.
                 * This, however, causes an exception.
                 */
                foreach (var path in referencePaths)
                {
                    if (libraryPaths.Add(path)) // check for duplicates ;)
                    {
                        try
                        {
                            var metadataReference = CreateMetadataReference(path);
                            feature.MetadataReferences.Add(metadataReference);
                        }
                        catch (Exception x)
                        {
                            throw new Exception($"A path for '{((AssemblyPart)providerPart).Name}' is invalid: '{x.Message}'.");
                        }
                    }
                }
            }
        }

        private static MetadataReference CreateMetadataReference(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                var moduleMetadata = ModuleMetadata.CreateFromStream(stream, PEStreamOptions.PrefetchMetadata);
                var assemblyMetadata = AssemblyMetadata.Create(moduleMetadata);
                return assemblyMetadata.GetReference(filePath: path);
            }
        }
    }
}