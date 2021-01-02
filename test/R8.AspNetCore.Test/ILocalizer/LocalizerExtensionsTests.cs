﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using R8.AspNetCore.TagBuilders;
using R8.Lib.Localization;
using R8.Test.Constants;
using Xunit;

namespace R8.AspNetCore.Test.ILocalizer
{
    public class LocalizerExtensionsTests
    {
        private readonly Localizer _localizer;

        private static List<CultureInfo> SupportedCultures => new List<CultureInfo>
        {
            CultureInfo.GetCultureInfo("tr"),
            CultureInfo.GetCultureInfo("en"),
            CultureInfo.GetCultureInfo("fa"),
        };

        public LocalizerExtensionsTests()
        {
            var configuration = new LocalizerConfiguration
            {
                SupportedCultures = SupportedCultures,
                Provider = new LocalizerJsonProvider
                {
                    Folder = Constants.FolderPath,
                    FileName = Constants.JsonFileName,
                }
            };
            _localizer = new Localizer(configuration, null);
        }

        [Fact]
        public async Task CallLocalizerHtml()
        {
            // Assets
            var key = "CookieBanner";
            var tags = new List<Func<string, IHtmlContent>>();
            tags.Add(str => new HtmlString("<a class=\"inline-link\" asp-page=\"typeof(PrivacyModel).GetPagePath()\">" +
                                           _localizer["PrivacyPolicy"] + "</a>"));

            // Acts
            CultureInfo.CurrentCulture = Constants.DefaultCulture;
            await _localizer.RefreshAsync();
            var act = _localizer.Html(key, tags.ToArray());
            var html = act.GetString();

            var expected =
                "Daha iyi bir kullanıcı deneyimi sağlamak, site trafiğini analiz etmek ve hedefli reklamlar sunmak için çerezleri kullanıyoruz. Bu web sitesini kullanmaya devam ederek, <a asp-page='typeof(PrivacyModel).GetPagePath()' class='inline-link'>Gizlilik Politikası</a> uygun olarak çerezlerin kullanılmasına izin vermiş olursunuz.";

            // Arrange
            Assert.Equal(expected, html);
        }

        [Fact]
        public async Task CallLocalizerFormat_NullTags2()
        {
            // Assets
            var key = "Copyright";

            // Acts
            CultureInfo.CurrentCulture = Constants.DefaultCulture;
            await _localizer.RefreshAsync();

            // Arrange
            Assert.Throws<NullReferenceException>(() => _localizer.Format(key, (List<string>)null));
        }

        [Fact]
        public async Task CallLocalizerFormat_NullKey()
        {
            // Assets
            var key = (string)null;
            var tags = new List<string>();
            var year = DateTime.Now.Year.ToString();
            tags.Add(year);

            // Acts
            CultureInfo.CurrentCulture = Constants.DefaultCulture;
            await _localizer.RefreshAsync();

            // Arrange
            Assert.Throws<ArgumentNullException>(() => _localizer.Format(key, tags.ToArray()));
        }

        [Fact]
        public void CallLocalizerFormat_NullLocalizer()
        {
            // Arrange
            Assert.Throws<ArgumentNullException>(() => LocalizerExtensions.Format(null, null, (List<string>)null));
        }

        [Fact]
        public async Task CallLocalizerFormat()
        {
            // Assets
            var key = "Copyright";
            var tags = new List<string>();
            var year = DateTime.Now.Year.ToString();
            tags.Add(year);

            // Acts
            CultureInfo.CurrentCulture = Constants.DefaultCulture;
            await _localizer.RefreshAsync();
            var act = _localizer.Format(key, tags.ToArray());
            var html = act.GetString();

            var expected = $"Telif Hakkı © {DateTime.Now.Year} EKOHOS Kurumsal";

            // Arrange
            Assert.Equal(expected, html);
        }

        [Fact]
        public void CallLocalizerFormat_Html_NullKey()
        {
            // Assets
            var key = (string)null;
            var tags = new List<Func<string, IHtmlContent>>();
            var year = DateTime.Now.Year.ToString();
            tags.Add(str => new HtmlString(year));

            // Arrange
            Assert.Throws<ArgumentNullException>(() => _localizer.Format(key, tags.ToArray()));
        }

        [Fact]
        public void CallLocalizerFormat_Html_NullLocalizer()
        {
            // Assets
            var key = (string)null;
            var tags = new List<Func<string, IHtmlContent>>();
            var year = DateTime.Now.Year.ToString();
            tags.Add(str => new HtmlString(year));

            // Arrange
            Assert.Throws<ArgumentNullException>(() => LocalizerExtensions.Format(null, key, tags.ToArray()));
        }

        [Fact]
        public async Task CallLocalizerFormat_Html()
        {
            // Assets
            const string key = "Copyright";
            var tags = new List<Func<string, IHtmlContent>>();
            var year = DateTime.Now.Year.ToString();
            tags.Add(str => new HtmlString(year));

            // Acts
            CultureInfo.CurrentCulture = Constants.DefaultCulture;
            await _localizer.RefreshAsync();
            var act = _localizer.Format(key, tags.ToArray()).GetString();

            var expected = $"Telif Hakkı © {DateTime.Now.Year} EKOHOS Kurumsal";

            // Arrange
            Assert.Equal(expected, act);
        }

        [Fact]
        public void CallLocalizerHtml_NullTags()
        {
            // Assets
            var key = "CookieBanner";

            // Arranges
            Assert.Throws<ArgumentNullException>(() => _localizer.Html(key));
        }

        [Fact]
        public void CallLocalizerHtml_NullKey()
        {
            // Assets
            string key = null;

            // Arranges
            Assert.Throws<ArgumentNullException>(() => _localizer.Html(key));
        }

        [Fact]
        public void CallLocalizerHtml_NullLocalizer()
        {
            // Assets
            string key = null;

            // Arranges
            Assert.Throws<ArgumentNullException>(() => LocalizerExtensions.Html(null, key));
        }
    }
}