using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace SWAT.Tests.AssertElementIsActive
{
    [TestFixture]
    [Category("FireFox")]
    public class FirefoxTests : AssertElementIsActiveTestFixture
    {
        public FirefoxTests() : base(BrowserType.FireFox) { }
    }
}
