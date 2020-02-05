using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using CoreXF.Abstractions;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace CoreXF.Framework
{
    public class ExtensionsApplicationBuilder : IExtensionsApplicationBuilder
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

        public interface IIdentityGenerator
        {
            string GetId(string current = null);
        }

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

    public class ExtensionsApplicationBuilderFactory : IExtensionsApplicationBuilderFactory
    {
        private static IExtensionsApplicationBuilder builder;   // NB: consider Lazy<T>

        public IExtensionsApplicationBuilder CreateBuilder(IApplicationBuilder builder)
        {
            if (ExtensionsApplicationBuilderFactory.builder == null)
            {
                ExtensionsApplicationBuilderFactory.builder = new ExtensionsApplicationBuilder(builder);
            }

            return ExtensionsApplicationBuilderFactory.builder;
        }
    }
}