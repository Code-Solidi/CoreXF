/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

namespace CoreXF.Abstractions.Base
{
    /// <summary>A default implementation of <see cref="IExtensionMvc">IExtensionMvc</see>.</summary>
    public class ExtensionBaseMvc : ExtensionBase, IExtensionMvc
    {
        /// <summary>
        /// The name of the compiled views assembly
        /// </summary>
        public string Views { get; set; }

        protected ExtensionBaseMvc()
        {
            this.Views = $"{this.GetType().Assembly.GetName().Name}.Views.dll";
        }
    }
}