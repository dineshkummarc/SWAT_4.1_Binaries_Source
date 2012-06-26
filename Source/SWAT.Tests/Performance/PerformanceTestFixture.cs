using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SWAT.DataAccess;

namespace SWAT.Tests.Performance
{
    public abstract class PerformanceTestFixture : BrowserTestFixture
    {
        public PerformanceTestFixture(BrowserType browserType)
            : base(browserType)
        {
        }


        //Performance numbers:
        //Before Changes: 130 seconds After Changes: 30 seconds
        [Test]
        [Timeout(100000)] //Original timeout 60000
        public void PerformanceFindElement()
        {
			try
			{
	            int iterations = 5;
	            _browser.NavigateBrowser(getTestPage("SwatPerfTest.html"));
	            DateTime timer = DateTime.Now;
	            
	            for (int i = 0; i < iterations; i++)
	            {
	                _browser.AssertElementExists(IdentifierType.Id, "z");
	                _browser.AssertElementExists(IdentifierType.InnerHtml, "z", "div");
	                _browser.AssertElementExists(IdentifierType.InnerHtmlContains, "z", "div");
	                _browser.AssertElementExists(IdentifierType.Expression, "name:z;id=z");
	            }

	            TimeSpan elapsed = DateTime.Now.Subtract(timer);

	            // putting 60 to be safe. It's more common for it to be between 25 - 45 seconds
                //changing value to 75 since its running on a VM and it takes longer (9/14/11)
                Assert.Greater(75, elapsed.TotalSeconds, "time: " + elapsed.TotalSeconds);
			}
			finally
			{
				NavigateToSwatTestPage();
			}
        }
    }
}
