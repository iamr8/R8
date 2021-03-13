﻿using R8.Lib.Test.Enums;
using R8.Lib.Test.FakeObjects;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Xunit;

namespace R8.Lib.Test
{
    public class PropertyReflectionsTests
    {
        [Fact]
        public void CallGetPropertyInfo()
        {
            // Assets
            Expression<Func<FakeObj, string>> func = o => o.Name;

            // Act
            var act = func.GetPropertyInfo();

            Assert.NotNull(act);
            Assert.Equal("Name", act.Name);
        }

        [Fact]
        public void CallGetUnderlyingType()
        {
            // Assets
            var list = new List<object>();

            // Act
            var act = list.GetType().GetUnderlyingType();

            Assert.Equal(typeof(object), act);
        }

        [Fact]
        public void CallGetUnderlyingType2()
        {
            // Assets
            var list = new List<object?>();

            // Act
            var act = list.GetType().GetUnderlyingType();

            Assert.Equal(typeof(object), act);
        }

        [Fact]
        public void CallGetUnderlyingType3()
        {
            // Assets
            var list = new List<int?>();

            // Act
            var act = list.GetType().GetUnderlyingType(false);

            Assert.Equal(typeof(int?), act);
        }

        [Fact]
        public void CallGetUnderlyingType4()
        {
            // Assets
            var list = new List<int?>();

            // Act
            var act = list.GetType().GetUnderlyingType(false);

            Assert.NotEqual(typeof(int), act);
        }

        [Fact]
        public void CallGetUnderlyingType5()
        {
            // Assets
            int? list = 5;

            // Act
            var act = list.GetType().GetUnderlyingType(false);

            Assert.Equal(typeof(int), act);
        }

        [Fact]
        public void CallGetExpressionValue()
        {
            // Assets
            Expression<Func<FakeObj, string>> func = o => o.Name;

            // Act
            var act = func.GetExpressionValue();

            Assert.NotNull(act);
            Assert.IsType<Func<FakeObj, string>>(act);
        }

        [Fact]
        public void CallGetDisplayName2()
        {
            // Assets
            Expression<Func<FakeObj, string>> func = o => o.LastName;

            // Act
            var act = func.GetPropertyInfo().GetDisplayName();

            Assert.NotNull(act);
            Assert.Equal("LastName", act);
        }

        [Fact]
        public void CallHasBaseType2()
        {
            // Assets
            Expression<Func<FakeObj, string>> func = o => o.LastName;

            // Act
            var act = typeof(FakeObj).HasBaseType(typeof(string));

            Assert.False(act);
        }

        [Fact]
        public void CallGetLambda()
        {
            // Assets
            Expression<Func<FakeObj, string>> func = o => o.LastName;

            // Act
            var act = func.TryGetLambda(out var lambda);

            Assert.True(act);
            Assert.NotNull(lambda);
        }

        [Fact]
        public void CallGetDisplayName()
        {
            // Assets
            Expression<Func<FakeObj, string>> func = o => o.Name;

            // Act
            var act = func.GetPropertyInfo().GetDisplayName();

            Assert.NotNull(act);
            Assert.Equal("Arash", act);
        }

        // [Fact]
        // public void CallGetMemberValue()
        // {
        //     // Assets
        //     Expression<Func<FakeObj, string>> func = o => o.Name;
        //
        //     // Act
        //     var act = func.GetMemberName();
        //
        //     Assert.NotNull(act);
        //     Assert.Equal("Name", act);
        // }

        [Fact]
        public void CallGetMemberName()
        {
            // Assets
            Expression<Func<FakeObj, string>> func = o => o.Name;

            // Act
            var act = func.GetMemberName();

            Assert.NotNull(act);
            Assert.Equal("Name", act);
        }

        [Fact]
        public void CallTrySetValue_DateTime2()
        {
            // Assets
            var input = "2020/05/223";
            var type = typeof(DateTime);

            // Act
            var method = type.TryConvertFrom(input, out var output);

            var expected = new DateTime(2020, 05, 23);
            DateTime.SpecifyKind(expected, DateTimeKind.Utc);

            // Arrange
            Assert.False(method);
            Assert.Null(output);
        }

        [Fact]
        public void CallTrySetValue_DateTime()
        {
            // Assets
            var input = "2020/05/23";
            var type = typeof(DateTime);

            // Act
            var method = type.TryConvertFrom(input, out var output);

            var expected = new DateTime(2020, 05, 23);
            DateTime.SpecifyKind(expected, DateTimeKind.Utc);

            // Arrange
            Assert.True(method);
            Assert.NotNull(output);
            Assert.IsType<DateTime>(output);
            Assert.Equal(expected, output);
        }

        [Fact]
        public void CallTrySetValue_DateTime3()
        {
            // Assets
            var input = "2020";
            var type = typeof(DateTime);

            // Act
            var method = type.TryConvertFrom(input, out var output);

            var expected = new DateTime(2020, 01, 01);
            DateTime.SpecifyKind(expected, DateTimeKind.Utc);

            // Arrange
            Assert.True(method);
            Assert.NotNull(output);
            Assert.IsType<DateTime>(output);
            Assert.Equal(expected, output);
        }

        [Fact]
        public void CallTrySetValue_DateTime4()
        {
            // Assets
            var input = "05-2020";
            var type = typeof(DateTime);

            // Act
            var method = type.TryConvertFrom(input, out var output);

            var expected = new DateTime(2020, 05, 01);
            DateTime.SpecifyKind(expected, DateTimeKind.Utc);

            // Arrange
            Assert.True(method);
            Assert.NotNull(output);
            Assert.IsType<DateTime>(output);
            Assert.Equal(expected, output);
        }

        [Fact]
        public void CallTrySetValue_DateTime5()
        {
            // Assets
            var input = "2020/05";
            var type = typeof(DateTime);

            // Act
            var method = type.TryConvertFrom(input, out var output);

            var expected = new DateTime(2020, 05, 01);
            DateTime.SpecifyKind(expected, DateTimeKind.Utc);

            // Arrange
            Assert.True(method);
            Assert.NotNull(output);
            Assert.IsType<DateTime>(output);
            Assert.Equal(expected, output);
        }

        [Fact]
        public void CallTrySetValue_DateTime6()
        {
            // Assets
            var input = "Arash";
            var type = typeof(DateTime);

            // Act
            var method = type.TryConvertFrom(input, out var output);

            // Arrange
            Assert.False(method);
            Assert.Null(output);
        }

        [Fact]
        public void CallTrySetValue_DateTime7()
        {
            // Assets
            string input = null;
            var type = typeof(DateTime);

            // Act
            var method = type.TryConvertFrom(input, out var output);

            // Arrange
            Assert.False(method);
            Assert.Null(output);
        }

        [Fact]
        public void CallTrySetValue_Boolean()
        {
            // Assets
            var input = true.ToString();
            var type = typeof(bool);

            // Act
            var method = type.TryConvertFrom(input, out var output);

            // Arranges
            Assert.True(method);
            Assert.NotNull(output);
            Assert.IsType<bool>(output);
            Assert.True((bool)output);
        }

        [Fact]
        public void CallTrySetValue_Boolean3()
        {
            // Assets
            var input = "off";
            var type = typeof(bool);

            // Act
            var method = type.TryConvertFrom(input, out var output);

            // Arranges
            Assert.True(method);
            Assert.NotNull(output);
            Assert.IsType<bool>(output);
            Assert.False((bool)output);
        }

        [Fact]
        public void CallTrySetValue_Boolean2()
        {
            // Assets
            var input = "100";
            var type = typeof(bool);

            // Act
            var method = type.TryConvertFrom(input, out var output);

            // Arrange
            Assert.False(method);
            Assert.Null(output);
        }

        [Fact]
        public void CallTrySetValue_Int()
        {
            // Assets
            var input = "1000000000000";
            var type = typeof(int);

            // Act
            var method = type.TryConvertFrom(input, out var output);

            // Arrange
            Assert.False(method);
            Assert.Null(output);
        }

        [Fact]
        public void CallTrySetValue_Int2()
        {
            // Assets
            var input = "1000000000000000000000";
            var type = typeof(int);

            // Act
            var method = type.TryConvertFrom(input, out var output);

            // Arrange
            Assert.False(method);
            Assert.Null(output);
        }

        [Fact]
        public void CallTrySetValue_Int3()
        {
            // Assets
            var input = "100";
            var type = typeof(int);

            // Act
            var method = type.TryConvertFrom(input, out var output);

            // Arrange
            Assert.True(method);
            Assert.NotNull(output);
            Assert.IsType<int>(output);
            Assert.Equal(100, output);
        }

        [Fact]
        public void CallTrySetValue_Int4()
        {
            // Assets
            var input = "100.2";
            var type = typeof(int);

            // Act
            var method = type.TryConvertFrom(input, out var output);

            // Arrange
            Assert.False(method);
            Assert.Null(output);
        }

        [Fact]
        public void CallTrySetValue_Double()
        {
            // Assets
            var input = "100..2";
            var type = typeof(double);

            // Act
            var method = type.TryConvertFrom(input, out var output);

            // Arrange
            Assert.False(method);
            Assert.Null(output);
        }

        [Fact]
        public void CallTrySetValue_Double2()
        {
            // Assets
            var input = "100.2";
            var type = typeof(double);

            // Act
            var method = type.TryConvertFrom(input, out var output);

            // Arrange
            Assert.True(method);
            Assert.NotNull(output);
            Assert.IsType<double>(output);
            Assert.Equal(100.2, output);
        }

        [Fact]
        public void CallTrySetValue_Long()
        {
            // Assets
            var input = "100";
            var type = typeof(long);

            // Act
            var method = type.TryConvertFrom(input, out var output);

            // Arrange
            Assert.True(method);
            Assert.NotNull(output);
            Assert.IsType<long>(output);
        }

        [Fact]
        public void CallTrySetValue_Long2()
        {
            // Assets
            var input = "100.1";
            var type = typeof(long);

            // Act
            var method = type.TryConvertFrom(input, out var output);

            // Arrange
            Assert.False(method);
            Assert.Null(output);
        }

        [Fact]
        public void CallTrySetValue_Long3()
        {
            // Assets
            var input = "Fake";
            var type = typeof(long);

            // Act
            var method = type.TryConvertFrom(input, out var output);

            // Arrange
            Assert.False(method);
            Assert.Null(output);
        }

        [Fact]
        public void CallTrySetValue_Enum()
        {
            // Assets
            var input = Flags.Success.ToString();
            var type = typeof(Flags);

            // Act
            var method = type.TryConvertFrom(input, out var output);

            // Arrange
            Assert.True(method);
            Assert.NotNull(output);
            Assert.IsType<Flags>(output);
        }

        [Fact]
        public void CallTrySetValue_Enum2()
        {
            // Assets
            var input = "Fake";
            var type = typeof(Flags);

            // Act
            var method = type.TryConvertFrom(input, out var output);

            // Arrange
            Assert.False(method);
            Assert.Null(output);
        }
    }
}