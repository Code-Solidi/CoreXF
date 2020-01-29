// Copyright (c) Code Solidi Ltd. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
namespace CoreXF.Abstractions
{
    /// <summary>A default implementation of <see cref="CoreXF.Abstractions.IExtension">IExtension</see>.</summary>
    public class ExtensionBase : IExtension
    {
        /// <summary>The name of the extension. As a convention use the name of the assembly.</summary>
        public virtual string Name => nameof(ExtensionBase);

        /// <summary>The description of the extension.</summary>
        public virtual string Description => "Base extension class, inherit to extend functionality.";

        /// <summary>The URL of the site related to the extension.</summary>
        public virtual string Url => "www.codesolidi.com";

        /// <summary>The extension's version.</summary>
        public virtual string Version => "1.0.0";

        /// <summary>The authors of the extension, comma separated.</summary>
        public virtual string Authors => "Code Solidi Ltd.";
    }
}