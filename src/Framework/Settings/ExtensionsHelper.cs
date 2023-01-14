/*
 * Copyright (c) 2016-2022 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using CoreXF.Abstractions.Attributes;
using CoreXF.Abstractions.Base;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CoreXF.Framework.Settings
{
    /// <summary>
    /// The extensions helper.
    /// </summary>
    public static class ExtensionsHelper
    {
        /// <summary>
        /// Determines if a given <paramref name="typeInfo"/> is a controller.
        /// </summary>
        /// <param name="typeInfo">The <see cref="TypeInfo"/> candidate.</param>
        /// <returns><code>true</code> if the type is a controller; otherwise <code>false</code>.</returns>
        public static bool IsController(TypeInfo typeInfo)
        {
            const string ControllerTypeNameSuffix = nameof(Controller);

            var result = typeInfo.IsClass && typeInfo.IsAbstract == false;

            // public top-level non-nested (regardless of visibility) classes
            result &= typeInfo.IsPublic && typeInfo.ContainsGenericParameters == false;

            result &= typeInfo.IsDefined(typeof(NonControllerAttribute)) == false;
            result &= typeInfo.Name.EndsWith(ControllerTypeNameSuffix, StringComparison.OrdinalIgnoreCase) ||
                typeInfo.IsDefined(typeof(ControllerAttribute));

            return result;
        }

        /// <summary>
        /// Is compiled view.
        /// </summary>
        /// <param name="typeInfo">The type info.</param>
        /// <returns>A bool.</returns>
        public static bool IsCompiledView(TypeInfo typeInfo)
        {
            //const string ViewComponentTypeNameSuffix = nameof(ViewComponent);

            //var result = typeInfo.IsClass && typeInfo.IsAbstract == false;

            //// public top-level non-nested (regardless of visibility) classes
            //result &= typeInfo.IsPublic && typeInfo.ContainsGenericParameters == false;
            //result &= typeInfo.Name.EndsWith(ViewComponentTypeNameSuffix, StringComparison.OrdinalIgnoreCase);

            //return result;
            return true;
        }

        /// <summary>
        /// Is view component.
        /// </summary>
        /// <param name="typeInfo">The type info.</param>
        /// <returns>A bool.</returns>
        public static bool IsViewComponent(TypeInfo typeInfo)
        {
            const string ViewComponentTypeNameSuffix = nameof(ViewComponent);

            var result = typeInfo.IsClass && typeInfo.IsAbstract == false;

            // public top-level non-nested (regardless of visibility) classes
            result &= typeInfo.IsPublic && typeInfo.ContainsGenericParameters == false;
            result &= typeInfo.Name.EndsWith(ViewComponentTypeNameSuffix, StringComparison.OrdinalIgnoreCase);

            return result;
        }

        //public static void DumpLoadedAssemblies(ILogger logger)
        //{
        //    var entry = Assembly.GetEntryAssembly();
        //    logger.LogTrace($"*** Entry: '{entry.GetName().FullName}'.");
        //    foreach (var asm in entry.GetReferencedAssemblies().OrderBy(x => x.FullName))
        //    {
        //        logger.LogTrace($"'{asm.FullName}'.");
        //    }

        //    var calling = Assembly.GetCallingAssembly();
        //    logger.LogTrace($"*** Calling: '{calling.GetName().FullName}'.");
        //    foreach (var asm in calling.GetReferencedAssemblies().OrderBy(x => x.FullName))
        //    {
        //        logger.LogTrace($"'{asm.FullName}'.");
        //    }

        //    var exec = Assembly.GetEntryAssembly();
        //    logger.LogTrace($"*** Executing: '{exec.GetName().FullName}'.");
        //    foreach (var asm in exec.GetReferencedAssemblies().OrderBy(x => x.FullName))
        //    {
        //        logger.LogTrace($"'{asm.FullName}'.");
        //    }
        //}

        /// <summary>
        /// Components the list of funcs of requestdelegates.
        /// </summary>
        /// <param name="app">The app.</param>
        /// <returns><![CDATA[IList<Func<RequestDelegate, RequestDelegate>>]]></returns>
        public static IList<Func<RequestDelegate, RequestDelegate>> Components(this IApplicationBuilder app)
        {
            var type = app.GetType();
            var field = type.GetField("_components", BindingFlags.Instance | BindingFlags.NonPublic);
            return (IList<Func<RequestDelegate, RequestDelegate>>)field.GetValue(app);
        }

        /// <summary>
        /// Is extension.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="logger">The logger.</param>
        /// <returns>A bool.</returns>
        internal static bool IsExtension(Assembly assembly, ILogger logger)
        {
            try
            {
                var type = assembly?.GetTypes().SingleOrDefault(t => typeof(IExtension).IsAssignableFrom(t));
                var extension = type != null ? (IExtension)Activator.CreateInstance(type) : null;
                return extension != null;
            }
            catch (Exception x)
            {
                logger?.LogError(x.InnerException?.Message ?? x.Message);
                return false;
            }
        }

        /// <summary>
        /// Is type ignored.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A bool.</returns>
        internal static bool IsTypeIgnored(Type type)
        {
            var extensionAttribute = type.GetCustomAttributes().SingleOrDefault(a => a is CoreXFIgnoreAttribute);
            return extensionAttribute != default;
        }
    }
}