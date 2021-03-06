﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Routing;

using NodaTime;

using R8.AspNetCore.Attributes;
using R8.Lib;

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace R8.AspNetCore.HttpContextExtensions
{
    public static class Extensions
    {
        /// <summary>
        /// Retrieves Absolute Path for current page according to <see cref="HttpContext"/>.
        /// </summary>
        /// <param name="context">An <see cref="HttpContext"/> object.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>An <see cref="string"/> value that representing page's absolute path.</returns>
        /// <remarks>If <see cref="RouteData"/> doesn't have a key as <c>page</c>, result will be null.</remarks>
        public static string GetCurrentPageAbsolutePath(this HttpContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return context.GetRouteData().Values["page"].ToString();
        }

        /// <summary>
        /// Determines whether the beginning of this <see cref="PathString"/> instance matches the specified <see cref="PathString"/>.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="paths"></param>
        /// <returns>true if value matches the beginning of this string; otherwise, false.</returns>
        public static bool StartsWithSegments(this PathString path, params PathString[] paths)
        {
            return paths.Any(path.StartsWithSegments);
        }

        /// <summary>
        /// Converts array form to given type.
        /// </summary>
        /// <typeparam name="T">A type for model.</typeparam>
        /// <param name="form">An <see cref="HttpContext"/>'s <see cref="IFormCollection"/>.</param>
        /// <param name="name">Name of the array included in Requested Form.</param>
        /// <param name="ignoreCase">Indicates case-sensitive for array property names.</param>
        /// <exception cref="MissingMethodException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <remarks>For example returns all form inputs with name : <c>register[firstname]</c></remarks>
        /// <returns>A model filled by given array values.</returns>
        public static T GetForm<T>(this IFormCollection form, string name, bool ignoreCase = true) where T : class
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            var formDictionary = form
                .Where(x => x.Key.StartsWith($"{name}["))
                .ToDictionary(x => x.Key.GetStringBetween('[', ']'), x => x.Value);
            if (!formDictionary.Any())
                return default;

            var model = Activator.CreateInstance<T>();
            var properties = model.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
            if (!properties.Any())
                return default;

            var usedProperties = new List<PropertyInfo>();

            static string GetPropertyName(MemberInfo propertyInfo)
            {
                return propertyInfo.GetCustomAttribute<FormProperty>()?.PropertyName ?? propertyInfo.Name;
            }

            foreach (var (key, values) in formDictionary)
            {
                if (values.Count == 0)
                    continue;

                var property = properties.Find(x => GetPropertyName(x).Equals(key,
                    ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture));
                if (property == null)
                    continue;

                if (usedProperties.Any(x => x.Equals(property)))
                    continue;

                usedProperties.Add(property);
                var propertyType = property.PropertyType;

                var validValue = propertyType.GetEnumerableUnderlyingType() != null
                    ? propertyType.TryParse(values.ToArray(), out var propertyValue)
                    : propertyType.GetUnderlyingType().TryParse(values[0], out propertyValue);
                if (!validValue)
                    continue;

                property.SetValue(model, propertyValue);
            }

            return model;
        }

        public static async Task RedirectWithPostDataAsync(this HttpContext httpContext, string url, NameValueCollection data)
        {
            var bytes = httpContext.RedirectWithPostDataCore(url, data);
            await httpContext.Response.Body.WriteAsync(bytes);
        }

        private static byte[] RedirectWithPostDataCore(this HttpContext httpContext, string url, NameValueCollection data)
        {
            var response = httpContext.Response;
            response.Clear();

            var s = new StringBuilder();
            s.Append("<html>");
            s.AppendFormat("<body onload='document.forms[\"form\"].submit()'>");
            s.AppendFormat("<form name='form' action='{0}' method='post'>", url);
            foreach (string key in data)
                s.AppendFormat("<input type='hidden' name='{0}' value='{1}' />", key, data[key]);

            s.Append("</form></body></html>");

            var bytes = Encoding.ASCII.GetBytes(s.ToString());
            return bytes;
        }

        public static void RedirectWithPostData(this HttpContext httpContext, string url, NameValueCollection data)
        {
            var bytes = httpContext.RedirectWithPostDataCore(url, data);
            httpContext.Response.Body.Write(bytes);
        }

        /// <summary>
        /// Retrieves specific <see cref="DateTimeZone"/> object according to current user time zone.
        /// </summary>
        /// <param name="context">An <see cref="HttpContext"/> object.</param>
        /// <param name="fallBackTimeZoneId">An <see cref="string"/> value that representing a valid time zone id, for using when we have errors in finding users time zone.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A <see cref="DateTimeZone"/> object.</returns>
        public static DateTimeZone GetTimeZoneSession(this HttpContext context, string fallBackTimeZoneId = "Europe/Istanbul")
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            DateTimeZone finalZone;
            if (context.User.Identity.IsAuthenticated)
            {
                finalZone = context.User.GetAuthenticatedUser().TimeZone;
            }
            else
            {
                var session = context.Session.GetString(UserTimeZoneConstant);
                if (!string.IsNullOrEmpty(session))
                {
                    try
                    {
                        finalZone = DateTimeZoneProviders.Tzdb[session];
                    }
                    catch
                    {
                        finalZone = DateTimeZoneProviders.Tzdb[fallBackTimeZoneId];
                    }
                }
                else
                {
                    finalZone = DateTimeZoneProviders.Tzdb[fallBackTimeZoneId];
                }
            }

            return finalZone;
        }

        /// <summary>
        /// Stores given time-zone into <see cref="HttpContext"/> sessions.
        /// </summary>
        /// <param name="context">An <see cref="HttpContext"/> object.</param>
        /// <param name="timeZone">An <see cref="DateTimeZone"/> object.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void SetTimeZoneSession(this HttpContext context, DateTimeZone timeZone)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (context.Session == null)
                throw new ArgumentNullException(nameof(context.Session));
            if (timeZone == null)
                throw new ArgumentNullException(nameof(timeZone));

            var session = context.Session.GetString(UserTimeZoneConstant);
            var hasSession = false;
            if (!string.IsNullOrEmpty(session))
            {
                try
                {
                    var anonyTimeZone = DateTimeZoneProviders.Tzdb[session];
                    hasSession = anonyTimeZone.Id == timeZone.Id;
                }
                catch
                {
                }
            }

            if (!hasSession)
                context.Session.SetString(UserTimeZoneConstant, timeZone.Id);
        }

        /// <summary>
        /// Builds a <see cref="Uri"/> component based on given path and query strings.
        /// </summary>
        /// <param name="httpContext">A <see cref="HttpContext"/> object.</param>
        /// <param name="relativePath">An <see cref="string"/> that representing action endpoint path without query strings.</param>
        /// <param name="queries">A <see cref="Dictionary{TKey,TValue}"/> that representing query strings.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>An <see cref="Uri"/> component.</returns>
        public static Uri BuildUri(this HttpContext httpContext, string relativePath, Dictionary<string, string> queries = null)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            var baseUrl = httpContext.GetBaseUrl();

            var queryBuilder = new QueryBuilder();
            if (queries != null)
                queryBuilder = new QueryBuilder(queries);

            var uri = new UriBuilder(baseUrl + relativePath);
            if (queryBuilder.Any())
                uri.Query = queryBuilder.ToQueryString().ToString();

            return uri.Uri;
        }

        /// <summary>
        /// Builds base url according to current <see cref="HttpContext"/> instance.
        /// </summary>
        /// <param name="httpContext">A <see cref="HttpContext"/> object.</param>
        /// <param name="includeScheme">A <see cref="bool"/> value that representing if scheme should be included.</param>
        /// <returns>An <see cref="string"/> value.</returns>
        public static string GetBaseUrl(this HttpContext httpContext, bool includeScheme = true)
        {
            var text = string.Empty;
            if (includeScheme)
                text += $"{httpContext.Request.Scheme}://";

            text += $"{httpContext.Request.Host}/";
            return text;
        }

        /// <summary>
        /// Returns a <see cref="AuthenticatedUser"/> object according to a collection of <see cref="Claim"/>.
        /// </summary>
        /// <param name="claims">A collection o <see cref="Claim"/> that representing authenticated user claims.</param>
        /// <returns>A <see cref="AuthenticatedUser"/> object.</returns>
        public static IAuthenticatedUser GetAuthenticatedUser(this IEnumerable<Claim> claims)
        {
            var claimsList = claims.ToList();
            if (claimsList?.Any() != true)
                return null;

            var currentUser = new AuthenticatedUser();
            currentUser.AddClaims(claimsList);
            return currentUser;
        }

        private const string UserTimeZoneConstant = "UserTimeZone";

        /// <summary>
        /// Returns a <see cref="AuthenticatedUser"/> object according to an <see cref="IPrincipal"/> interface.
        /// </summary>
        /// <param name="principal">An <see cref="IPrincipal"/> object.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A <see cref="AuthenticatedUser"/> object.</returns>
        public static IAuthenticatedUser GetAuthenticatedUser(this IPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(HttpContext));

            return ((ClaimsPrincipal)principal).Claims.GetAuthenticatedUser();
        }

        /// <summary>
        /// Returns a <see cref="AuthenticatedUser"/> object according to an a collection of claims that stored in <see cref="HttpContext"/>.
        /// </summary>
        /// <param name="httpContext">An <see cref="HttpContext"/> object.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A <see cref="AuthenticatedUser"/> object.</returns>
        public static IAuthenticatedUser GetAuthenticatedUser(this HttpContext httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(HttpContext));

            return !httpContext.User.Identity.IsAuthenticated
                ? null
                : httpContext.User.GetAuthenticatedUser();
        }
    }
}