/*
 * Copyright (c) 2017-2020 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */

using System;
using System.Collections.Generic;
using System.Linq;

using CoreXF.Abstractions.Builder;
using CoreXF.Framework.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace CoreXF.Framework.Builder
{
    public partial class ExtensionsApplicationBuilder : IExtensionsApplicationBuilder
    {
        private IApplicationBuilder appBuilder;
        private readonly IList<Func<RequestDelegate, RequestDelegate>> middlewares;
        private readonly List<Shim> shims = new List<Shim>();
        private readonly IIdentityGenerator generator;

        public ExtensionsApplicationBuilder(IApplicationBuilder appBuilder, IIdentityGenerator generator = null)
        {
            this.appBuilder = appBuilder;
            this.generator = generator ?? new NaiveShimIdentityGenerator(0);
            this.middlewares = new List<Func<RequestDelegate, RequestDelegate>>();
            var id = this.generator.GetId();
            this.shims.Add(new Shim(id));
        }

        public IServiceProvider ApplicationServices
        {
            get => this.appBuilder.ApplicationServices;
            set => this.appBuilder.ApplicationServices = value;
        }

        public IEnumerable<string> GetShims()
        {
            return this.shims.Select(x => x.Id);
        }

        public IApplicationBuilder ExpansionPoint(string shimId)
        {
            var found = this.shims.SingleOrDefault(x => x.Id == shimId);
            return found != default ? found.ApplicationBuilder(this) : null;
        }

        public IDictionary<string, object> Properties => this.appBuilder.Properties;

        public IFeatureCollection ServerFeatures => this.appBuilder.ServerFeatures;

        public RequestDelegate Build() => this.appBuilder.Build();

        public IApplicationBuilder New() => this.appBuilder.New();

        public IApplicationBuilder Use(Func<RequestDelegate, RequestDelegate> middleware)
        {
            this.middlewares.Add(middleware);
            var last = this.shims.LastOrDefault()?.Id;
            var id = this.generator.GetId(last);
            this.shims.Add(new Shim(id));
            return this;
        }

        public void Populate(IApplicationBuilder app)
        {
            var thisComponents = this.middlewares.ToList();
            var n = thisComponents.GetEnumerator();

            var components = new List<Func<RequestDelegate, RequestDelegate>>();
            foreach (var shim in this.shims)
            {
                if (shim.IsUsed())
                {
                    components.AddRange(shim.GetComponents());
                }

                if (n.MoveNext())
                {
                    components.Add(n.Current);
                }
            }

            var appComponents = app.Components();
            foreach (var component in components)
            {
                appComponents.Add(component);
            }
        }

        private class Shim
        {
            private IApplicationBuilder builder;

            public Shim(string id)
            {
                this.Id = id;
            }

            internal string Id { get; }

            internal IApplicationBuilder ApplicationBuilder(IApplicationBuilder app)
            {
                this.builder ??= app.New();
                return this.builder;
            }

            internal bool IsUsed() => this.builder != null;

            internal IEnumerable<Func<RequestDelegate, RequestDelegate>> GetComponents() => this.builder?.Components();
        }
    }
}