﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using R8.Lib.Localization;

namespace R8.EntityFrameworkCore.EntityBases
{
    public static class EntityLocalizedExtensions
    {
        /// <summary>
        /// Filters a sequence of values based on <c><see cref="key"/></c> and <c><see cref="value"/></c> for json in given <see cref="IQueryable{TResult}"/>.
        /// </summary>
        /// <typeparam name="TSource">A generic type of <see cref="EntityLocalized"/></typeparam>
        /// <param name="source">A <see cref="IQueryable{T}"/> that representing translating query.</param>
        /// <param name="key">An <see cref="string"/> value that representing specific key iso two char name that you looking for.</param>
        /// <param name="value">A <see cref="string"/> value that representing localized name that already stored in <see cref="LocalizerContainer"/> in entity.</param>
        /// <param name="deepCheck">Preview feature.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>Continues chains of <see cref="IQueryable"/> plus adding current filter.</returns>
        /// <remarks>For value type those are not string, you need to first convert your object to json.</remarks>
        public static IQueryable<TSource> WhereJson<TSource>(this IQueryable<TSource> source, string key, string value, bool deepCheck = false) where TSource : EntityLocalized
        {
            // TODO get Expression
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var jsonKey = $"\"{key}\":\"";
            var jsonFull = jsonKey + $"{value}\"";

            if (!deepCheck)
                return source.Where(x =>
                    EF.Functions.Like(x.NameJson, $"%{jsonFull}%"));

            return source.Where(x =>
                value == x.NameJson.Substring(x.NameJson.IndexOf(jsonKey) + jsonKey.Length,
                    x.NameJson.IndexOf("\"", x.NameJson.IndexOf(jsonKey) + jsonKey.Length) - x.NameJson.IndexOf(jsonKey) - jsonKey.Length));
        }

        /// <summary>
        /// Filters a sequence of values based on <c><see cref="key"/></c> and <c><see cref="value"/></c> in given <see cref="IEnumerable{TResult}"/>.
        /// </summary>
        /// <typeparam name="TSource">A generic type of <see cref="EntityLocalized"/></typeparam>
        /// <param name="source">A <see cref="IQueryable{T}"/> that representing translating query.</param>
        /// <param name="key">An <see cref="string"/> value that representing specific key iso two char name that you looking for.</param>
        /// <param name="value">A <see cref="string"/> value that representing localized name that already stored in <see cref="LocalizerContainer"/> in entity.</param>
        /// <param name="deepCheck">Preview feature.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>Continues chains of <see cref="IEnumerable"/> plus adding current filter.</returns>
        /// <remarks>For value type those are not string, you need to first convert your object to json.</remarks>
        public static IEnumerable<TSource> WhereJson<TSource>(this IEnumerable<TSource> source, string key, string value, bool deepCheck = false) where TSource : EntityLocalized
        {
            // TODO get Expression
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var jsonKey = $"\"{key}\":\"";
            var jsonFull = jsonKey + $"{value}\"";

            if (!deepCheck)
                return source.Where(x =>
                    x.NameJson.Contains($"%{jsonFull}%"));

            return source.Where(x =>
                value == x.NameJson.Substring(x.NameJson.IndexOf(jsonKey) + jsonKey.Length,
                    x.NameJson.IndexOf("\"", x.NameJson.IndexOf(jsonKey) + jsonKey.Length) - x.NameJson.IndexOf(jsonKey) - jsonKey.Length));
        }
    }
}