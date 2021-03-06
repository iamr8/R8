﻿using R8.AspNetCore.Test;
using R8.Lib.Localization;
using R8.Lib.Test.Enums;
using R8.Lib.Test.FakeObjects;

using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

using Xunit;

namespace R8.Lib.Test.MethodTests
{
    public class ExtensionsTests
    {
        private readonly Localizer _localizer;

        private static List<CultureInfo> SupportedCultures => new List<CultureInfo>
        {
            CultureInfo.GetCultureInfo("tr"),
            CultureInfo.GetCultureInfo("en"),
            CultureInfo.GetCultureInfo("fa"),
        };

        public ExtensionsTests()
        {
            var configuration = new LocalizerConfiguration
            {
                SupportedCultures = SupportedCultures,
                Provider = new LocalizerJsonProvider
                {
                    Folder = Constants.GetLocalizerDictionaryPath(),
                    FileName = Constants.JsonFileName,
                }
            };

            _localizer = new Localizer(configuration, null);
        }

        // [Fact]
        // public void CallGetMessage_LocalizerNull()
        // {
        //     // Assets
        //     var response = new FakeResponse(Flags.Success);
        //
        //     // Act
        //     CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fa");
        //     var message = response.Message;
        //
        //     var expected = "Success";
        //
        //     // Arrange
        //     Assert.Equal(expected, message);
        // }

        // [Fact]
        // public async Task CallGetMessage()
        // {
        //     // Assets
        //     var response = new FakeResponse(Flags.Success);
        //     await _localizer.RefreshAsync();
        //     response.SetLocalizer(_localizer);
        //
        //     // Act
        //     CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fa");
        //     var message = response.Message;
        //
        //     var expected = "عملیات به موفقیت انجام شد";
        //
        //     // Arrange
        //     Assert.Equal(expected, message);
        // }
    }
}