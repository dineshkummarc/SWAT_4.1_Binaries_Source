using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace SWAT.Tests.AssertElementIsActive
{
    [TestFixture, RequiresSTA]
    [Category("IE")]
    public class InternetExplorerTests : AssertElementIsActiveTestFixture
    {
        public InternetExplorerTests() : base(BrowserType.InternetExplorer) { }
    }
}
