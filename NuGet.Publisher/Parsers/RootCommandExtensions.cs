/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the GNU GENERAL PUBLIC LICENSE Version 2. See GNU-GPL.txt in the project root for license information.
 */

using System.CommandLine;
using System.CommandLine.IO;
using System.Text;

namespace NuGet.Publisher.Parsers
{
    internal static class RootCommandExtensions
    {
        private class InnerConsole : IConsole
        {
            public IStandardStreamWriter Out { get; }

            public bool IsOutputRedirected { get; }

            public IStandardStreamWriter Error { get; }

            public bool IsErrorRedirected { get; }

            public bool IsInputRedirected { get; }

            public InnerConsole()
            {
                this.Out = new InnerStringWriter(new StringBuilder());
                //this.Error = new InnerStringWriter(new StringBuilder());
            }

            public override string ToString()
            {
                return this.Out.ToString();
            }

            private class InnerStringWriter : IStandardStreamWriter
            {
                private readonly StringBuilder stringBuilder;

                public InnerStringWriter(StringBuilder stringBuilder)
                {
                    this.stringBuilder = stringBuilder;
                }

                public void Write(string value)
                {
                    this.stringBuilder.Append(value);
                }

                public override string ToString()
                {
                    return this.stringBuilder.ToString();
                }
            }
        }

        public static string GetHelp(this RootCommand rootCommand)
        {
            var help = new InnerConsole();
            var result = rootCommand.Invoke("-h", help);
            return help.ToString();
        }
    }
}