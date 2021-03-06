﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace R8.Lib
{
    public static class Texts
    {
        private static readonly HashSet<char> DefaultNonWordCharacters = new HashSet<char> { ',', '.', ':', ';' };

        /// <summary>
        /// Reports specified index of given value in string.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="value"></param>
        /// <param name="nth"></param>
        /// <param name="startIndex"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public static int IndexOfNth(this string str, char value, int nth, int? startIndex = null)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(message: "param cannot be less than 0",
                    paramName: nameof(startIndex));
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value));

            var nSign = Math.Sign(nth);
            var inputLength = str?.Length ?? 0;
            int index;
            int count;

            switch (nSign)
            {
                case 1:
                    index = startIndex ?? 0;
                    count = inputLength - index;
                    break;

                case -1:
                    index = startIndex ?? inputLength - 1;
                    count = index + 1;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(message: "param cannot be equal to 0", paramName: nameof(nth));
            }

            while (count-- > 0)
            {
                if (str[index] == value && (nth -= nSign) == 0)
                    return index;

                index += nSign;
            }

            return -1;
        }

        /// <summary>
        /// Returns a <see cref="string"/> between to given characters.
        /// </summary>
        /// <param name="str">A <see cref="string"/> value to check in</param>
        /// <param name="start">Starting <see cref="char"/></param>
        /// <param name="end">Ending <see cref="char"/></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns>A <see cref="string"/> between two characters.</returns>
        public static string GetStringBetween(this string str, char start, char end)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            if (start <= 0)
                throw new ArgumentOutOfRangeException(nameof(start));
            if (end <= 0)
                throw new ArgumentOutOfRangeException(nameof(end));

            return new string(str.SkipWhile(c => c != start)
                .Skip(1)
                .TakeWhile(c => c != end)
                .ToArray()).Trim();
        }

        /// <summary>
        /// Returns short UUID for given guid.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static string ToShort(this Guid guid)
        {
            return Convert.ToBase64String(guid.ToByteArray())[..^2]
                .Replace('+', '-')
                .Replace('/', '_')
                .Split("_")[0];
        }

        /// <summary>
        /// Replaces NewLine characters to specific character.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public static string ReplaceEnterChar(this string str, string replacement)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            str = str.Replace("\r\n", replacement)
                .Replace("\n", replacement)
                .Replace("\r", replacement)
                .Replace(Environment.NewLine, replacement);

            return str;
        }

        /// <summary>
        /// Converts string to camelCase.
        /// </summary>
        /// <param name="s">An <see cref="string"/> value containing string to convert.</param>
        /// <param name="culture">An object that supplies culture-specific casing rules.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>An <see cref="string"/> value</returns>
        public static string ToCamelCase(this string s, CultureInfo culture = null)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));
            var currentCulture = culture ?? CultureInfo.CurrentCulture;

            var result = s.Humanize(culture: currentCulture);
            result = result[..1].ToLower(currentCulture) + result[1..];
            result = result.Replace(" ", string.Empty, true, currentCulture);

            return result;
        }

        /// <summary>
        /// Converts kebab-case string to normal string
        /// </summary>
        /// <param name="s">An <see cref="string"/> value containing string to convert.</param>
        /// <param name="culture">An object that supplies culture-specific casing rules.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>An <see cref="string"/> value</returns>
        public static string FromKebabCase(this string s, CultureInfo culture = null)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));
            var currentCulture = culture ?? CultureInfo.CurrentCulture;
            return s.Replace("-", " ", true, currentCulture);
        }

        /// <summary>
        /// Converts string to kebab-case
        /// </summary>
        /// <param name="s">An <see cref="string"/> value containing string to convert.</param>
        /// <param name="culture">An object that supplies culture-specific casing rules.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>An <see cref="string"/> value</returns>
        public static string ToKebabCase(this string s, CultureInfo culture = null)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));
            var currentCulture = culture ?? CultureInfo.CurrentCulture;
            return s.Humanize(culture: currentCulture).ToLower(currentCulture)
                .Replace(" ", "-", true, currentCulture);
        }

        /// <summary>
        /// Removes unescaped characters from string
        /// </summary>
        /// <param name="s">An <see cref="string"/> value containing string to convert.</param>
        /// <param name="ignoreSpace"></param>
        /// <param name="culture">An object that supplies culture-specific casing rules.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>An <see cref="string"/> value</returns>
        public static string ToUnescaped(this string s, bool ignoreSpace = false, CultureInfo culture = null)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));
            var currentCulture = culture ?? CultureInfo.CurrentCulture;

            if (!string.IsNullOrEmpty(s))
                s = Regex.Replace(s, @"[^\w\d-. ]", "");

            if (ignoreSpace)
                s = s.Replace(" ", string.Empty, true, currentCulture);

            return s;
        }

        /// <summary>
        /// Removes unescaped characters from string
        /// </summary>
        /// <param name="s">An <see cref="string"/> value containing string to convert.</param>
        /// <param name="ignoreSpace"></param>
        /// <param name="culture">An object that supplies culture-specific casing rules.</param>
        /// <param name="forceToTitleCase">If true, output value will be in TitleCase</param>
        /// <returns>An <see cref="string"/> value</returns>
        public static string Humanize(this string s, bool ignoreSpace = false, CultureInfo culture = null, bool forceToTitleCase = false)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            var currentCulture = culture ?? CultureInfo.CurrentCulture;

            // The only way we can recognize if language supports upper-lower cases
            if (currentCulture.TextInfo.IsRightToLeft)
                return s;

            var textInfo = currentCulture.TextInfo;
            var result = new StringBuilder();
            var stringArray = s.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            if (stringArray.Length == 1)
            {
                if (ignoreSpace)
                    return s;

                // ThisIsFake
                // IELTS&TOMER

                var str = stringArray[0];
                if (str.Length == 1)
                    return str;

                if (str.Equals(str.ToLower(currentCulture)))
                    return textInfo.ToTitleCase(str);

                const string escapedChars = @"!@#$%^&*()_+?><';/.,\|`~=-";
                var startingIndex = 0;

                var canCarryOn = true;
                while (canCarryOn)
                {
                    var currentWord = string.Empty;
                    if (startingIndex > str.Length)
                    {
                        canCarryOn = false;
                    }
                    else
                    {
                        currentWord = str[startingIndex..];
                        canCarryOn = !string.IsNullOrEmpty(currentWord);
                    }
                    if (!canCarryOn)
                        break;

                    var currentChar = currentWord.FirstOrDefault();
                    var currentCharIndex = str.IndexOf(currentChar, startingIndex);

                    if (char.IsUpper(currentChar) || char.IsLower(currentChar))
                    {
                        if (currentCharIndex == str.Length - 1)
                        {
                            result.Append(currentChar);
                            startingIndex = str.Length;
                        }
                        else
                        {
                            var targetChar = str[currentCharIndex + 1];
                            Func<char, bool> desiredCharacter;
                            if (!char.IsUpper(targetChar))
                            {
                                desiredCharacter = x =>
                                    char.IsUpper(x) || char.IsDigit(x) || escapedChars.Contains(x);
                            }
                            else
                            {
                                desiredCharacter = x =>
                                    char.IsLower(x) || char.IsDigit(x) || escapedChars.Contains(x);
                            }

                            var startOfNextWord = currentWord
                                .Skip(1)
                                .FirstOrDefault(desiredCharacter);
                            var startOfNextWordIndex = startOfNextWord != 0
                                ? string.Join("", str.Skip(1)).IndexOf(startOfNextWord) + 1
                                : str.Length;
                            var endCharIndex = startOfNextWordIndex - 1;

                            var word = str[new Range(currentCharIndex, endCharIndex + 1)];
                            result.Append(word);
                            result.Append(" ");

                            startingIndex = startOfNextWordIndex;
                        }
                    }
                    else if (char.IsDigit(currentChar) || escapedChars.Contains(currentChar))
                    {
                        if (currentCharIndex > 0)
                        {
                            var prevWord = str[new Range(0, currentCharIndex)];
                            result.Append(currentChar);
                        }
                        else
                        {
                            result.Append(currentChar);
                        }
                        result.Append(" ");
                        startingIndex = currentCharIndex + 1;
                    }
                }
            }
            else
            {
                // This Is Fake
                foreach (var str in stringArray)
                {
                    var tempLower = str.ToLower(currentCulture);

                    var finalString = str;
                    if (forceToTitleCase)
                    {
                        if (str.Length > 0)
                        {
                            var firstChar = str[0];
                            finalString = firstChar.ToString().ToUpper(culture);

                            var restOfChars = str[new Range(1, ^0)];
                            finalString += restOfChars.ToString().ToLower(culture);
                        }
                    }
                    else
                    {
                        if (str.Equals(tempLower, StringComparison.CurrentCulture))
                            finalString = textInfo.ToTitleCase(str);
                    }

                    result.Append(finalString);

                    if (!ignoreSpace)
                        result.Append(" ");
                }
            }

            return result.ToString().Trim();
        }

        /// <summary>
        /// Replaces a list of strings inside string to specific value
        /// </summary>
        /// <param name="s">An <see cref="string"/> value</param>
        /// <param name="oldValues">An <see cref="IEnumerable{T}"/> of strings to be replaced/</param>
        /// <param name="newValue">New value should been replaced with old values</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>An <see cref="string"/> value</returns>
        public static string Replace(this string s, string newValue, params string[] oldValues)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            if (oldValues == null)
                throw new ArgumentNullException(nameof(oldValues));
            if (newValue == null)
                throw new ArgumentNullException(nameof(newValue));

            return oldValues
                .ToList()
                .Aggregate(s, (value, oldValue) => value.Replace(oldValue, newValue));
        }

        /// <summary>
        ///     Fixes common writing mistakes caused by using a bad keyboard layout,
        ///     such as replacing Arabic Ye with Persian one and so on ...
        /// </summary>
        /// <param name="text">Text to process</param>
        /// <returns>Processed Text</returns>
        private static string ApplyPersianYeKe(this string text)
        {
            return string.IsNullOrEmpty(text)
                ? string.Empty
                : text.Replace(ArabicYeChar, PersianYeChar).Replace(ArabicKeChar, PersianKeChar).Trim();
        }

        private const char ArabicKeChar = (char)1603;
        private const char ArabicYeChar = (char)1610;
        private const char PersianKeChar = (char)1705;
        private const char PersianYeChar = (char)1740;

        /// <summary>
        /// Adds half-space char between word and prefix/suffix
        /// </summary>
        /// <param name="text">Text to process</param>
        /// <returns>Processed Text</returns>
        private static string ApplyHalfSpaceRule(this string text)
        {
            //put zwnj between word and prefix (mi* nemi*)
            var phase1 = Regex.Replace(text, @"\s+(ن?می)\s+", " $1‌");

            //put zwnj between word and suffix (*tar *tarin *ha *haye)
            return Regex.Replace(phase1, @"\s+(تر(ی(ن)?)?|ها(ی)?)\s+", "‌$1 ");
        }

        /// <summary>
        /// Fixes non-unicode characters
        /// </summary>
        /// <param name="s">An <see cref="string"/> value that containing text to be fixed</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>An <see cref="string"/> value</returns>
        public static string FixUnicode(this string s)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            return s
                .ApplyPersianYeKe()
                .ApplyHalfSpaceRule()
                .YeHeHalfSpace()
                .CleanupZwnj()
                .FixExtraMarks()
                .FixUnicodeNumbers()
                .FixExtraMarks()
                .Replace((char)1610, (char)1740)
                .Replace((char)1603, (char)1705)
                .Trim();
        }

        /// <summary>
        ///     Replaces more than one ! or ? mark with just one
        /// </summary>
        /// <param name="text">Text to process</param>
        /// <returns>Processed Text</returns>
        private static string FixExtraMarks(this string text)
        {
            var phrase = Regex.Replace(text, "(!){2,}", "$1");
            return Regex.Replace(phrase, "(؟){2,}", "$1");
        }

        /// <summary>
        ///     Removes unnecessary zwnj char that are succeeded/preceded by a space
        /// </summary>
        /// <param name="text">Text to process</param>
        /// <returns>Processed Text</returns>
        private static string CleanupZwnj(this string text)
        {
            return Regex.Replace(text, @"\s+‌|‌\s+", " ");
        }

        /// <summary>
        ///     Converts ه ی to ه‌ی
        /// </summary>
        /// <param name="text">Text to process</param>
        /// <returns>Processed Text</returns>
        private static string YeHeHalfSpace(this string text)
        {
            return Regex.Replace(text, @"(\S)(ه[\s‌]+[یي])(\s)", "$1ه‌ی‌$3"); // fix zwnj
        }
    }
}