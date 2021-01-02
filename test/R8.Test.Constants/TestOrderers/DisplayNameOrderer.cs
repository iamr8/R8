﻿using System.Collections.Generic;
using System.Linq;

using Xunit;
using Xunit.Abstractions;

namespace R8.Test.Constants.TestOrderers
{
    public class DisplayNameOrderer : ITestCollectionOrderer
    {
        public IEnumerable<ITestCollection> OrderTestCollections(
            IEnumerable<ITestCollection> testCollections) =>
            testCollections.OrderBy(collection => collection.DisplayName);
    }
}