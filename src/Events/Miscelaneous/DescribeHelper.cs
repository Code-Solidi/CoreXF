using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Miscelaneous
{
    public static class DescribeHelper
    {
        public const string QueryAsJson = "application/vnd+gtp.query+json";
        public const string ContentType = "Content-Type";

        public static string Describe(ControllerBase controller)
        {
            var lines = new List<string>();
            var methods = controller.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public);
            foreach (var method in methods)
            {
                var describe = method.GetCustomAttribute<DescribeAttribute>();
                if (describe != null)
                {
                    var request = controller.HttpContext.Request;
                    var description = string.Empty;
                    description = DescribeHelper.DescribeAsJson(describe, request, method);
                    lines.Add(description);
                }
            }

            var result = string.Empty;
            var body = string.Join(",", lines.ToArray());
            result = $"[{body}]";

            return result;
        }

        private static string DescribeAsJson(DescribeAttribute describe, HttpRequest request, MethodInfo method)
        {
            DescribeHelper.PrepareDescription(request, method, out var uri, out var verbs, out var parameters);
            var item = new ApiCall
            {
                Uri = uri,
                Verbs = verbs,
                Description = describe.Description,
                Parameters = parameters
            };

            return JsonConvert.SerializeObject(item);
        }

        private static void PrepareDescription(HttpRequest request
            , MethodInfo method
            , out string uri
            , out string verbs
            , out IEnumerable<ApiCallParameter> parameters)
        {
            var scheme = request.Scheme;
            var host = request.Host.Value;
            var query = request.QueryString.Value;
            var verb = method.GetCustomAttribute<HttpMethodAttribute>(true);
            var path = //request.Path.Value.Replace("/help", string.Empty) +
                string.IsNullOrWhiteSpace(verb.Template) == false ? $"/{verb.Template}" : "/";
            uri = $"{scheme}://{host}{path}{query}";
            verbs = verb.HttpMethods.Any() ? string.Join(", ", verb.HttpMethods) : string.Empty;
            parameters = DescribeHelper.PrepareParameters(method);
        }

        private static List<ApiCallParameter> PrepareParameters(MethodInfo method)
        {
            var paramList = new List<ApiCallParameter>();
            foreach (var pi in method.GetParameters())
            {
                var param = DescribeHelper.PrepareParameter(pi.ParameterType, pi.Name);
                paramList.Add(param);
            }

            return paramList;
        }

        private static ApiCallParameter PrepareParameter(Type type, string name)
        {
            if (type.IsPrimitive || type == typeof(string) || type == typeof(DateTime))
            {
                return new ApiCallParameter { Type = DescribeHelper.ConvertToCSharpName(type.FullName), Name = name };
            }
            else
            {
                var paramList = new List<ApiCallParameter>();
                foreach (var propInfo in type.GetProperties())
                {
                    var param = DescribeHelper.PrepareParameter(propInfo.PropertyType, propInfo.Name);
                    paramList.Add(param);
                }

                return new ApiCallParameter
                {
                    Type = DescribeHelper.ConvertToCSharpName(type.FullName),
                    Name = name,
                    Properties = paramList
                };
            }
        }

        private static string ConvertToCSharpName(string fullName)
        {
            // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/built-in-types-table
            var conversion = new Dictionary<string, string>
            {
                { "Boolean", "bool" },
                { "Byte", "byte" },
                { "SByte", "sbyte" },
                { "Char", "char" },
                { "Decimal", "decimal" },
                { "Double", "double" },
                { "Single", "float" },
                { "Int32", "int" },
                { "UInt32", "uint" },
                { "Int64", "long" },
                { "UInt64", "ulong" },
                { "Object", "object" },
                { "Int16", "short" },
                { "UInt16", "ushort" },
                { "String", "string" }
            };

            if (fullName.StartsWith("System"))
            {
                fullName = fullName.Remove(0, "System".Length + 1);
                if (conversion.ContainsKey(fullName))
                {
                    return conversion[fullName];
                }
            }

            return fullName;
        }

        private class ApiCallParameter
        {
            public string Type { get; set; }

            public string Name { get; set; }

            public IEnumerable<ApiCallParameter> Properties { get; set; }
        }

        private class ApiCall
        {
            public string Uri { get; set; }

            public string Verbs { get; set; }

            public string Description { get; set; }

            public IEnumerable<ApiCallParameter> Parameters { get; set; }
        }

        public static bool Inspect(IHeaderDictionary headers)
        {
            return headers.ContainsKey(DescribeHelper.ContentType)
                && headers[DescribeHelper.ContentType] == DescribeHelper.QueryAsJson;
        }
    }
}