using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Miscelaneous
{
    public static class Utility
    {
        public enum Method { GET, POST, PUT, DELETE }
/*
        public enum ChangeReason
        {
            Add,
            Modify,
            Delete,
            Attach,
            Detach,
            Undo
        }

        public static string GetServiceUri(IConfiguration config)
        {
            var address = config.GetValue<string>("HttpServer:address") ?? "127.0.0.1";
            var ipAddress = IPAddress.Parse(address);
            var port = config.GetValue<int>("HttpServer:port");

            var schema = Uri.UriSchemeHttp;
            var httpsPort = config.GetValue<int>("HttpServer:httpsPort");
            if (httpsPort != 0)
            {
                port = httpsPort;
                schema = Uri.UriSchemeHttps;
            }

            return new Uri($"{schema}://{ipAddress}:{port}").AbsoluteUri;
        }*/

        public static async Task<HttpResponseMessage> SendAsync(string requestUri, string content, Method method = Method.POST)
        {
            using (var client = new HttpClient())
            {
                var result = new HttpResponseMessage(HttpStatusCode.NoContent);
                try
                {
                    switch (method)
                    {
                        case Method.GET:
                            result = await client.GetAsync(requestUri);
                            break;

                        case Method.POST:
                            result = await client.PostAsync(requestUri, new StringContent(content, Encoding.UTF8, "application/json"));
                            break;

                        case Method.PUT:
                            result = await client.PutAsync(requestUri, new StringContent(content, Encoding.UTF8, "application/json"));
                            break;

                        case Method.DELETE:
                            result = await client.DeleteAsync(requestUri);
                            break;
                    }
                }
                catch (HttpRequestException x)
                {
                    result = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        ReasonPhrase = x.InnerException?.Message ?? x.Message
                    };
                }

                return result;
            }
        }
        /*
                public static string CreateUri(HttpRequest request, Guid id, string path = null)
                {
                    var thescheme = request.Scheme;
                    var thehost = request.Host.ToString().Trim(new[] { '/' });
                    var thepath = (string.IsNullOrWhiteSpace(path) == false
                        ? path
                        : request.Path.ToString()).Trim(new[] { '/' });

                    var uri = $"{thescheme}://{thehost}/{thepath}/{id.ToString()}";
                    return uri;
                }

                public static bool HasMatch(object obj, Dictionary<string, object> pairs)
                {
                    bool evaluate(Type type
                        , BinaryExpression body
                        , ParameterExpression left
                        , ParameterExpression right
                        , object valueLeft
                        , object valueRight)
                    {
                        switch (type.Name.ToLowerInvariant())
                        {
                            case "int16":
                            case "int32":
                            case "int64":
                                {
                                    var lambda = Expression.Lambda<Func<int, int, bool>>(body, left, right);
                                    return lambda.Compile()((int)valueLeft, (int)valueRight);
                                }

                            case "string":
                                {
                                    var lambda = Expression.Lambda<Func<string, string, bool>>(body, left, right);
                                    return lambda.Compile()((string)valueLeft, (string)valueRight);
                                }

                            case "datetime":
                                {
                                    var lambda = Expression.Lambda<Func<DateTime, DateTime, bool>>(body, left, right);
                                    return lambda.Compile()((DateTime)valueLeft, (DateTime)valueRight);
                                }
                        }

                        return false;
                    }

                    var props = new Dictionary<Tuple<string, Type>, object>();
                    foreach (var pi in obj.GetType().GetProperties())
                    {
                        props.Add(new Tuple<string, Type>(pi.Name, pi.PropertyType), pi.GetValue(obj));
                    }

                    var result = true;
                    var defaultPair = default(KeyValuePair<string, object>);
                    foreach (var prop in props)
                    {
                        var pair = pairs.SingleOrDefault(x => x.Key.Equals(prop.Key.Item1, StringComparison.OrdinalIgnoreCase));
                        if (pair.Key != defaultPair.Key || pair.Value != defaultPair.Value)
                        {
                            var name = pair.Key;
                            var type = prop.Key.Item2;
                            var left = Expression.Parameter(type, name);
                            var right = Expression.Parameter(type, name);
                            var body = Expression.Equal(left, right);

                            result &= evaluate(type, body, left, right, pair.Value, prop.Value);
                        }
                    }

                    return result;
                }*/
    }
}