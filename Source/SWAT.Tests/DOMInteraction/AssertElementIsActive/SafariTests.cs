using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace SWAT.Tests.AssertElementIsActive
{
    [TestFixture]
    [Category("Safari")]
    public class SafariTests : AssertElementIsActiveTestFixture
    {
        public SafariTests() : base(BrowserType.Safari) { }
    }
}
