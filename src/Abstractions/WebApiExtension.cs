/*
 * Copyright (c) 2016-2022 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using CoreXF.Abstractions.Base;

namespace CoreXF.Abstractions
{
    /// <summary>
    /// The web api extension.
    /// </summary>
    public class WebApiExtension : AbstractExtension, IWebApiExtension
    {
        /// <inheritdoc/>>
        public ExtensionStatus Status { get; private set; } = ExtensionStatus.Running;

        /// <inheritdoc/>>
        public virtual void Start()
        {
            this.Status = ExtensionStatus.Running;
        }

        /// <inheritdoc/>>
        public virtual void Stop()
        {
            this.Status = ExtensionStatus.Stopped;
        }
    }
}