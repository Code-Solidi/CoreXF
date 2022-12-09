/*
 * Copyright (c) 2016-2022 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using CoreXF.Abstractions.Base;

using static CoreXF.Abstractions.Base.IWebApiExtension;

namespace CoreXF.Abstractions
{
    public class WebApiExtension : AbstractExtension, IWebApiExtension
    {
        public ExtensionStatus Status { get; private set; } = ExtensionStatus.Running;

        public virtual void Start()
        {
            this.Status = ExtensionStatus.Running;
        }

        public virtual void Stop()
        {
            this.Status = ExtensionStatus.Stopped;
        }
    }
}