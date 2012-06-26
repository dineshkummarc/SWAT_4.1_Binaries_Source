using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace SWAT.Tests.Performance
{
    [TestFixture, RequiresSTA]
    [Category("IE")]
    public class InternetExplorerTest : PerformanceTestFixture
    {
        public InternetExplorerTest()
            : base(BrowserType.InternetExplorer)
        {
        }

    }
}
