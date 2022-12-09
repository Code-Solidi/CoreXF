/*
 * Copyright (c) 2016-2022 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

namespace CoreXF.Abstractions.Base
{
    public interface IWebApiExtension : IExtension
    {
        public enum ExtensionStatus { Stopped, Running }

        /// <summary>
        /// The extension status.
        /// </summary>
        ExtensionStatus Status { get; }

        void Start();

        void Stop();
    }
}