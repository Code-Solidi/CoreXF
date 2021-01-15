/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */

using System.Reflection;

using CoreXF.Abstractions.Base;

using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

#if TEST
using Microsoft.Extensions.FileProviders.Embedded;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
#endif

namespace CoreXF.Framework.Providers
{
    public class ExtensionsFileProvider : IFileProvider
    {
        private readonly
#if TEST
            InnerFileProvider
#else
            EmbeddedFileProvider
#endif
            inner;

        private readonly
#if TEST
            InnerFileProvider
#else
            EmbeddedFileProvider
#endif
            inner2;

        private readonly ILogger logger;

        public ExtensionsFileProvider(ILoggerFactory loggerFactory, IExtension extension)
        {
            this.Extension = extension;
            this.logger = loggerFactory.CreateLogger<ExtensionsFileProvider>();
            var assembly = Assembly.GetAssembly(extension.GetType());

            // NB: consider using root path somehow
#if TEST
            this.inner = new InnerFileProvider(assembly, this.Extension.Name);
            this.inner2 = new InnerFileProvider(assembly, $"{this.Extension.Name}.wwwroot");
#else
            this.inner = new EmbeddedFileProvider(assembly, this.Extension.Name);
            this.inner2 = new EmbeddedFileProvider(assembly, $"{this.Extension.Name}.wwwroot");
#endif
        }

        public IExtension Extension { get; }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            var contents = this.inner.GetDirectoryContents(subpath);
            this.logger.LogTrace($"{this.Extension.Name}: directory contents '{subpath}' => {contents}");
            return contents;
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            var fileInfo = this.inner.GetFileInfo(subpath);
            if (fileInfo is NotFoundFileInfo)
            {
                fileInfo = this.inner2.GetFileInfo(subpath);
            }

            this.logger.LogTrace($"{this.Extension.Name}: fileinfo '{subpath}' => {fileInfo}");
            return fileInfo;
        }

        public IChangeToken Watch(string filter)
        {
            var changeToken = this.inner.Watch(filter);
            return changeToken;
        }

        // consider employing the methods below
        protected virtual IDirectoryContents GetExtensionsDirectoryContents(string subpath)
        {
            return default;
        }

        protected virtual IFileInfo GetExtensionFileInfo(string subpath)
        {
            return default;
        }

        protected virtual IChangeToken WatchExtension(string filter)
        {
            return default;
        }

#if TEST
        /// <summary>
        /// Looks up files using embedded resources in the specified assembly.
        /// This file provider is case sensitive.
        /// </summary>
        class InnerFileProvider : IFileProvider
        {
            private static readonly char[] invalidFileNameChars = Path.GetInvalidFileNameChars()
                .Where(c => c != '/' && c != '\\').ToArray();

            private readonly Assembly assembly;
            private readonly string baseNamespace;
            private readonly DateTimeOffset lastModified;

            /// <summary>
            /// Initializes a new instance of the <see cref="InnerFileProvider" /> class using the specified
            /// assembly with the base namespace defaulting to the assembly name.
            /// </summary>
            /// <param name="assembly">The assembly that contains the embedded resources.</param>
            public InnerFileProvider(Assembly assembly)
                : this(assembly, assembly?.GetName()?.Name)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="InnerFileProvider" /> class using the specified
            /// assembly and base namespace.
            /// </summary>
            /// <param name="assembly">The assembly that contains the embedded resources.</param>
            /// <param name="baseNamespace">The base namespace that contains the embedded resources.</param>
            public InnerFileProvider(Assembly assembly, string baseNamespace)
            {
                this.baseNamespace = string.IsNullOrEmpty(baseNamespace) ? string.Empty : baseNamespace + ".";
                this.assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));

                this.lastModified = DateTimeOffset.UtcNow;

                if (!string.IsNullOrEmpty(this.assembly.Location))
                {
                    try
                    {
                        this.lastModified = File.GetLastWriteTimeUtc(this.assembly.Location);
                    }
                    catch (PathTooLongException)
                    {
                    }
                    catch (UnauthorizedAccessException)
                    {
                    }
                }
            }

            /// <summary>
            /// Locates a file at the given path.
            /// </summary>
            /// <param name="subpath">The path that identifies the file. </param>
            /// <returns>
            /// The file information. Caller must check Exists property. A <see cref="NotFoundFileInfo" /> if the file could
            /// not be found.
            /// </returns>
            public IFileInfo GetFileInfo(string subpath)
            {
                if (string.IsNullOrEmpty(subpath))
                {
                    return new NotFoundFileInfo(subpath);
                }

                var builder = new StringBuilder(this.baseNamespace.Length + subpath.Length);
                builder.Append(this.baseNamespace);

                // Relative paths starting with a leading slash okay
                if (subpath.StartsWith("/", StringComparison.Ordinal))
                {
                    builder.Append(subpath, 1, subpath.Length - 1);
                }
                else
                {
                    builder.Append(subpath);
                }

                for (var i = this.baseNamespace.Length; i < builder.Length; i++)
                {
                    if (builder[i] == '/' || builder[i] == '\\')
                    {
                        builder[i] = '.';
                    }
                }

                var resourcePath = builder.ToString();
                if (InnerFileProvider.HasInvalidPathChars(resourcePath))
                {
                    return new NotFoundFileInfo(resourcePath);
                }

                var name = Path.GetFileName(subpath);
                if (this.assembly.GetManifestResourceInfo(resourcePath) == null)
                {
                    return new NotFoundFileInfo(name);
                }

                return new EmbeddedResourceFileInfo(this.assembly, resourcePath, name, this.lastModified);
            }

            /// <summary>
            /// Enumerate a directory at the given path, if any.
            /// This file provider uses a flat directory structure. Everything under the base namespace is considered to be one
            /// directory.
            /// </summary>
            /// <param name="subpath">The path that identifies the directory</param>
            /// <returns>
            /// Contents of the directory. Caller must check Exists property. A <see cref="NotFoundDirectoryContents" /> if no
            /// resources were found that match <paramref name="subpath" />
            /// </returns>
            public IDirectoryContents GetDirectoryContents(string subpath)
            {
                // The file name is assumed to be the remainder of the resource name.
                if (subpath == null)
                {
                    return NotFoundDirectoryContents.Singleton;
                }

                // EmbeddedFileProvider only supports a flat file structure at the base namespace.
                if (subpath.Length != 0 && !string.Equals(subpath, "/", StringComparison.Ordinal))
                {
                    return NotFoundDirectoryContents.Singleton;
                }

                var entries = new List<IFileInfo>();

                // TODO: The list of resources in an assembly isn't going to change. Consider caching.
                var resources = this.assembly.GetManifestResourceNames();
                for (var i = 0; i < resources.Length; i++)
                {
                    var resourceName = resources[i];
                    if (resourceName.StartsWith(this.baseNamespace, StringComparison.Ordinal))
                    {
                        entries.Add(new EmbeddedResourceFileInfo(
                            this.assembly,
                            resourceName,
                            resourceName.Substring(this.baseNamespace.Length),
                            this.lastModified));
                    }
                }

                return new EnumerableDirectoryContents(entries);
            }

            /// <summary>
            /// Embedded files do not change.
            /// </summary>
            /// <param name="pattern">This parameter is ignored</param>
            /// <returns>A <see cref="NullChangeToken" /></returns>
            public IChangeToken Watch(string pattern)
            {
                return NullChangeToken.Singleton;
            }

            private static bool HasInvalidPathChars(string path)
            {
                return path.IndexOfAny(invalidFileNameChars) != -1;
            }
        }

        internal class EnumerableDirectoryContents : IDirectoryContents
        {
            private readonly IEnumerable<IFileInfo> entries;

            public EnumerableDirectoryContents(IEnumerable<IFileInfo> entries)
            {
                this.entries = entries ?? throw new ArgumentNullException(nameof(entries));
            }

            public bool Exists => true;

            public IEnumerator<IFileInfo> GetEnumerator()
            {
                return this.entries.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }
#endif
    }
}