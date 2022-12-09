/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the GNU GENERAL PUBLIC LICENSE Version 2. See GNU-GPL.txt in the project root for license information.
 */

using CoreXF.Abstractions;

namespace WebAplExt2
{
    public class CalculatorExtension : WebApiExtension
    {
        public CalculatorExtension()
        {
            this.Name = nameof(CalculatorExtension).Replace("Extension", string.Empty);
            this.Copyright = "© Code Solidi Ltd. 2019-2022";
        }
    }
}