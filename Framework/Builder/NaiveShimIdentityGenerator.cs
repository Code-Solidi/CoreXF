/*
 * Copyright (c) 2017-2020 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */

using System;
using System.Text.RegularExpressions;

namespace CoreXF.Framework.Builder
{
    public partial class ExtensionsApplicationBuilder
    {
        public class NaiveShimIdentityGenerator : IIdentityGenerator
        {
            private readonly int number;
            private const string template = "Shim-{0}";
            private const string pattern = @"\d+";

            public NaiveShimIdentityGenerator(int number)
            {
                this.number = number;
            }

            public string GetId(string current = null)
            {
                var next = this.number;
                if (string.IsNullOrEmpty(current) == false)
                {
                    var regex = new Regex(pattern);
                    var match = regex.Match(current);
                    if (match.Success)
                    {
                        next = Convert.ToInt32(match.Value) + 1;
                    }
                }

                return string.Format(template, next);
            }
        }
    }
}