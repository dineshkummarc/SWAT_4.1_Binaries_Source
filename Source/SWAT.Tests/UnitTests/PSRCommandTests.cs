using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SWAT;
using SWAT.Fitnesse;
using SWAT.Tests;

namespace SWAT.Tests.UnitTests
{
    [TestFixture]
    [Category("Misc")]
    public class PSRCommandTests
    {
        WebBrowser _browser;

        public PSRCommandTests()
        {
            _browser = new WebBrowser(BrowserType.InternetExplorer);
        }

        [Test]
        public void TimerResetTest()
        {
            _browser.StartTimer("test timer");
            _browser.Sleep(200);
            _browser.CheckTimer("test timer", "GreaterThanOrEqualTo", 100);
            _browser.ResetTimer("test timer");
            _browser.CheckTimer("test timer", "LessThan", 2);
        }

        [Test]
        public void CreatingTimersTest()
        {
            //Make a bunch of timers to make sure they will all add to the dict
            for (int i = 0; i < 500; i++)
                _browser.StartTimer(i + "");
            for (int i = 0; i < 500; i++) // make sure all timers still exist in the dict
                _browser.ResetTimer(i + "");
        }

         
        [TestCase("GreaterThan")]
        [TestCase("GreaterThanOrEqualTo")]
        [TestCase("LessThan")]
        [TestCase("LessThanOrEqualTo")]
        public void TimerPassesTest(string comparator)
        {
            try
            {
                _browser.StartTimer("pass timer");
            }
            catch
            {
                _browser.ResetTimer("pass timer");
            }
            _browser.Sleep(1000);
            if (comparator.Equals("LessThan") || comparator.Equals("LessThanOrEqualTo"))
                _browser.CheckTimer("pass timer", comparator, 123445);
            else
                _browser.CheckTimer("pass timer", comparator, 200);
        }

         
        [ExpectedException(typeof(AssertionFailedException))]
        [TestCase("EqualTo")]
        [TestCase("GreaterThan")]
        [TestCase("GreaterThanOrEqualTo")]
        [TestCase("LessThan")]
        [TestCase("LessThanOrEqualTo")]
        public void TimerFailsTest(string comparator)
        {
            try
            {
                _browser.StartTimer("fail timer");
            }
            catch
            {
                _browser.ResetTimer("fail timer");
            }
            _browser.Sleep(1000);
            if (comparator.Equals("EqualTo") || comparator.Equals("LessThan") || comparator.Equals("LessThanOrEqualTo"))
            {
                _browser.CheckTimer("fail timer", comparator, 300);
            }
            else
                _browser.CheckTimer("fail timer", comparator, 92384);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void InvalidOperatorFailsTest()
        {
            _browser.StartTimer("testOpTimer");
            _browser.CheckTimer("testOpTimer", "invalid op", 500);
        }

        [Test]
        [ExpectedException(typeof(TimerDoesNotExistException))]
        public void NonexistentTimerFailsInResetTimerTest()
        {
            _browser.StartTimer("this timer does exist");
            _browser.ResetTimer("but this one doesnt");
        }

        [Test]
        [ExpectedException(typeof(TimerDoesNotExistException))]
        public void NonexistentTimerFailsInCheckTimerTest()
        {
            _browser.CheckTimer("i still dont exist", "LessThan", 5000);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TimerFailsWithNegativeValueTest()
        {
            _browser.StartTimer("negValTest");
            _browser.CheckTimer("negValTest", "GreaterThan", -56);
        }

        [Test]
        [ExpectedException(typeof(TimerDoesNotExistException))]
        public void GetTimerValueTimerDoesNotExistTest()
        {
            _browser.GetTimerValue("aTimer");
        }

        [Test]
        public void DisplayTimerValueReturnsCorrectValueTest()
        {
            _browser.StartTimer("timer1");
            _browser.Sleep(1500);
            Assert.IsInstanceOfType(typeof(int), _browser.DisplayTimerValue("timer1"));
        }
    }
}
