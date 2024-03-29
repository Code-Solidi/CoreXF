﻿/*
 * Copyright (c) 2016-2022 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using System.Threading;

using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Primitives;

namespace CoreXF.Framework.Providers
{
    // https://stackoverflow.com/questions/46156649/asp-net-core-register-controller-at-runtime
    internal class ExtensionsActionDescriptorChangeProvider : IActionDescriptorChangeProvider
    {
        internal ExtensionsActionDescriptorChangeProvider()
        {
        }

        public CancellationTokenSource TokenSource { get; private set; }

        public bool HasChanged { get; set; }

        public IChangeToken GetChangeToken()
        {
            this.TokenSource = new CancellationTokenSource();
            return new CancellationChangeToken(this.TokenSource.Token);
        }
    }
}