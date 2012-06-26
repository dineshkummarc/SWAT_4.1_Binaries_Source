/********************************************************************************
    This file is part of Simple Web Automation Toolkit, 
    Copyright (C) 2007 by Ultimate Software, Inc. All rights reserved.

    Simple Web Automation Toolkit is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License version 3 as published by
    the Free Software Foundation; 

    Simple Web Automation Toolkit is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.

 */

/********************************************************************************/


using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using SWAT.Fitnesse;
using System.Diagnostics;
using System.Threading;

namespace SWAT.Tests.Fitnesse
{
  [TestFixture]
  [Category("Misc")]
  public class FitnesseTestFixture
  {
      [SetUp]
      public virtual void TestSetup()
      {
          TestManager.ResetForNewTest();
      }

     
    [TestCase("@CommandName", false)]
    [TestCase("@@CommandName", true)]
    [TestCase("@@@CommandName", false)]
    [TestCase("CommandName", true)]
    public void TestCommandIsCritical(string command, bool isCritical)
    {
      SWAT.Fitnesse.Command objCommand = new Command(command);
      Assert.AreEqual(isCritical, objCommand.IsCritical);
    }

     
    [TestCase("?CommandName", IfStatementType.Single)]
    [TestCase("??CommandName", IfStatementType.Block)]
    [TestCase("CommandName", IfStatementType.None)]
    public void TestIfStatement(string command, IfStatementType ifStatementType)
    {
      SWAT.Fitnesse.Command objCommand = new Command(command);
      Assert.AreEqual(ifStatementType, objCommand.IfStatementType);
    }

     
    [TestCase("<>CommandName", true)]
    [TestCase("@<>CommandName", true)]
    [TestCase("@@<>CommandName", true)]
    [TestCase("@@@<>CommandName", true)]
    [TestCase("?<>CommandName", true)]
    [TestCase("?!<>CommandName", true)]
    [TestCase("??<>CommandName", true)]
    [TestCase("??!<>CommandName", true)]
    public void TestCommandIsInverse(string command, bool isInverse)
    {
        SWAT.Fitnesse.Command objCommand = new Command(command);
        Assert.AreEqual(isInverse, objCommand.IsInverse);
    }
      
     
    [TestCase("@commandName", "commandName")]
    [TestCase("@@commandName", "commandName")]
    [TestCase("@@@commandName", "commandName")]
    [TestCase("?commandName", "commandName")]
    [TestCase("??commandName", "commandName")]
    [TestCase("?!commandName", "commandName")]
    [TestCase("??!commandName", "commandName")]
    [TestCase("<>commandName", "commandName")]
    [TestCase("@<>commandName", "commandName")]
    [TestCase("@@<>commandName", "commandName")]
    [TestCase("@@@<>commandName", "commandName")]
    [TestCase("?<>commandName", "commandName")]
    [TestCase("?!<>commandName", "commandName")]
    [TestCase("??<>commandName", "commandName")]
    [TestCase("??!<>commandName", "commandName")]
    public void TestCommandName(string command, string name)
    {
      Command objCommand = new Command(command);
      Assert.AreEqual(name, objCommand.Name);
    }


    [Test]
    public void TestManagerCommandFailedTestSingleIf()
    {
      //TestManager mngr = new TestManager(new SWATFixture());
      
      Command failedCommand = new Command("?myCommandSingleIf");
      failedCommand.Passed = false;

      TestManager.LogCommand(failedCommand);
        //mngr.LogCommand(failedCommand);


      Command nextCommand = new Command("executeMeOnlyIfLastCommandPassed");

      //Assert.AreEqual(false, mngr.ShouldExecute(nextCommand));

      //mngr.LogCommand(nextCommand);
        TestManager.LogCommand(nextCommand);

      Command anotherCommand = new Command("thiscommandshouldexecute");

      //Assert.AreEqual(true, mngr.ShouldExecute(nextCommand));
      Assert.AreEqual(true, TestManager.ShouldExecute(nextCommand));


    }

    [Test]
    public void TestManagerCommandFailedTestBlockIf()
    {
      //TestManager mngr = new TestManager(new SWATFixture());

      Command failedCommand = new Command("??myCommandBlockIf");
      failedCommand.Passed = false;

      //mngr.LogCommand(failedCommand);
        TestManager.LogCommand(failedCommand);

      Command nextCommand = new Command("executeMeOnlyIfLastCommandPassed");

      //Assert.AreEqual(false, mngr.ShouldExecute(nextCommand));
        Assert.AreEqual(false, TestManager.ShouldExecute(nextCommand));


      //mngr.LogCommand(nextCommand);
        TestManager.LogCommand(nextCommand);

      Command anotherCommand = new Command("thiscommandshouldnotexecute");

      //Assert.AreEqual(false, mngr.ShouldExecute(anotherCommand));
        Assert.AreEqual(false, TestManager.ShouldExecute(anotherCommand));

      //mngr.LogCommand(anotherCommand);
        TestManager.LogCommand(anotherCommand);

      //let's test when we leave the table.
      //mngr.ResetForNewTable();
        TestManager.ResetForNewTable();

      Command oneMoreCommand = new Command("thiscommandshouldexecute");

      //Assert.AreEqual(true, mngr.ShouldExecute(oneMoreCommand));
        Assert.AreEqual(true, TestManager.ShouldExecute(oneMoreCommand));

    }

    [Test]
    public void TestManagerCommandFailedTestSingleIfNot()
    {
      //TestManager mngr = new TestManager(new SWATFixture());

      Command firstCommand = new Command("?!myCommandSingleIf");
      firstCommand.Passed = true;

      //mngr.LogCommand(firstCommand);
        TestManager.LogCommand(firstCommand);

      Command nextCommand = new Command("executeMeOnlyIfLastCommandFailed");

      Assert.AreEqual(false, TestManager.ShouldExecute(nextCommand));

      TestManager.LogCommand(nextCommand);

      Command anotherCommand = new Command("thiscommandshouldexecute");

      Assert.AreEqual(true, TestManager.ShouldExecute(nextCommand));


    }

    [Test]
    public void TestManagerCommandFailedTestBlockIfNot()
    {
      //TestManager TestManager = new TestManager(new SWATFixture());

      Command firstCommand = new Command("??!myCommandBlockIf");
      firstCommand.Passed = true;

      TestManager.LogCommand(firstCommand);


      Command nextCommand = new Command("executeMeOnlyIfLastCommandFailed");

      Assert.AreEqual(false, TestManager.ShouldExecute(nextCommand));

      TestManager.LogCommand(nextCommand);

      Command anotherCommand = new Command("thiscommandshouldnotexecute");

      Assert.AreEqual(false, TestManager.ShouldExecute(anotherCommand));

      TestManager.LogCommand(anotherCommand);

      //let's test when we leave the table.
      TestManager.ResetForNewTable();

      Command oneMoreCommand = new Command("thiscommandshouldexecute");

      Assert.AreEqual(true, TestManager.ShouldExecute(oneMoreCommand));

    }

    [Test]
    public void TestManagerShouldExecute()
    {
      SWAT.Fitnesse.Command prevCommand = new Command("myCommand");
      SWAT.Fitnesse.Command objCommand = new Command("myCommand");
      //TestManager TestManager = new TestManager(new SWATFixture());

      prevCommand.Passed = false;
      TestManager.LogCommand(prevCommand);
      Assert.AreEqual(false, TestManager.ShouldExecute(objCommand));

      prevCommand.Passed = true;
      TestManager.LogCommand(prevCommand);
      Assert.AreEqual(false, TestManager.ShouldExecute(objCommand)); //manager is aware we are in a completely failed state and will block everything


      //prevCommand.Passed = true;
      //mngr.LogCommand(prevCommand);
      //Assert.AreEqual(true, objCommand.ShouldExecute(prevCommand));
      //Assert.AreEqual(true, mngr.ShouldExecute);
    }

    [Test]
    public void TestManagerGetSetIgnoreRemainingTableRows()
    {
        TestManager.IgnoreRemainingTableRows = false;
        Assert.IsFalse(TestManager.IgnoreRemainingTableRows, "IgnoreRemaningTableRows is not setting values correctly");
        TestManager.IgnoreRemainingTableRows = true;
        Assert.IsTrue(TestManager.IgnoreRemainingTableRows, "IgnoreRemaningTableRows is not setting values correctly");
    }

    [Test]
    public void TestManagerGetSetIgnoreRemainingTestRows()
    {
        TestManager.IgnoreRemainingTestRows = false;
        Assert.IsFalse(TestManager.IgnoreRemainingTestRows, "IgnoreRemainingTestRows is not setting values correctly");
        TestManager.IgnoreRemainingTestRows = true;
        Assert.IsTrue(TestManager.IgnoreRemainingTestRows, "IgnoreRemainingTestRows is not setting values correctly");
    }

    [Test]
    public void TestManagerGetSetInCompareData()
    {
        TestManager.InCompareData = true;
        Assert.IsTrue(TestManager.InCompareData, "InCompareData is not setting values correctly");
        TestManager.InCompareData = false;
        Assert.IsFalse(TestManager.InCompareData, "InCompareData is not setting values correctly");
    }

    [Test]
    public void TestManagerGetSetInCompareDataIsCritical()
    {
        TestManager.InCompareDataIsCritical = true;
        Assert.IsTrue(TestManager.InCompareDataIsCritical, "InCompareData is not setting values correctly");
        TestManager.InCompareDataIsCritical = false;
        Assert.IsFalse(TestManager.InCompareDataIsCritical, "InCompareData is not setting values correctly");
    }

    [Test]
    public void TestManagerCommandWithInverseModifier()
    {
        TestManager.ResetForNewTest();
        Command inverseCommand = new Command("<>myCommandInverse");
        TestManager.LogCommand(inverseCommand);
        Command nextCommand = new Command("executeMeOnlyIfLastCommandDidNotPassed"); //Command will execute if the previous command was logged correctly.
        TestManager.LogCommand(nextCommand);
        Assert.IsTrue(TestManager.ShouldExecute(nextCommand), "Inverse Moidifier is not working correctly");
        inverseCommand.Passed = false;
        TestManager.LogCommand(inverseCommand);
        TestManager.LogCommand(nextCommand);
        Assert.IsFalse(TestManager.ShouldExecute(nextCommand), "Inverse Moidifier is not working correctly");
        TestManager.ResetForNewTest();
    }

    [Test]
    public void TestCommandShouldReport()
    {
        Command myCommand = new Command("AnyCommand");
        Assert.IsTrue(myCommand.ShouldReport);

        myCommand = new Command("?AnyCommand");
        Assert.IsFalse(myCommand.ShouldReport);
    }

    [Test]
    public void TestManagerGetSetPreviusCommand()
    {
        TestManager.PreviousCommand = new Command("MyCommand"); //set
        Assert.IsNotNull(TestManager.PreviousCommand, "Previous command is not setting propertly"); //get
    }

    [Test]
    public void TestManagerResetForNewTest()
    {
        TestManager.IgnoreRemainingTableRows = true;
        TestManager.IgnoreRemainingTestRows = true;
        TestManager.PreviousCommand = new Command("MyCommand");

        TestManager.ResetForNewTest();

        Assert.IsFalse(TestManager.IgnoreRemainingTableRows, "ResetForNewTest is not setting values properly");
        Assert.IsFalse(TestManager.IgnoreRemainingTestRows, "ResetForNewTest is not setting values properly");
        if (TestManager.PreviousCommand != null)
            throw new Exception("ResetForNewTest is not setting values properly");
    }

    [Test]
    public void TestManagerShouldExecuteWorksWithAbandonTest()
    {
        Command abandonCommand = new Command("AbandonTest");
        Command arbitraryCommandWithModifier = new Command("@@OpenBrowser");
        Assert.IsTrue(TestManager.ShouldExecute(abandonCommand), "ShouldExecute should return true for AbandonTest!");
        Assert.IsTrue(TestManager.AbandonTest, "TestManager.AbandonTest should assert true after ShouldExecute method is run with AbandonTest");
        Assert.IsFalse(TestManager.ShouldExecute(arbitraryCommandWithModifier), "TestManager.ShouldExecute() should return false after AbandonTest has been run");
        TestManager.ResetForNewTest();
    }

    [Test]
    public void TestManagerShouldExecuteIgnoreAbandonTest()
    {
        Command abandonCommand = new Command("AbandonTest");
        TestManager.IgnoreRemainingTableRows = true;
        TestManager.ShouldExecute(abandonCommand);
        Assert.IsFalse(TestManager.AbandonTest, "TestManager should ignore AbandonTest when IgnoreRemainingTableRows is true");
        TestManager.IgnoreRemainingTableRows = false;

        TestManager.IgnoreRemainingTestRows = true;
        TestManager.ShouldExecute(abandonCommand);
        Assert.IsFalse(TestManager.AbandonTest, "TestManager should ignore AbandonTest when IgnoreRemainingTestRows is true");
        TestManager.IgnoreRemainingTestRows = false;
        TestManager.ResetForNewTest();

    }
    [Test]
    public void TestManagerShouldExecuteWorksWithResumeTest()
    {
        Command resumeCommand = new Command("ResumeTest");
        Assert.IsTrue(TestManager.ShouldExecute(resumeCommand), "ShouldExecute should return true for ResumeTest!");
        Command abandonCommand = new Command("AbandonTest");
        TestManager.ShouldExecute(abandonCommand);
        TestManager.ShouldExecute(resumeCommand);
        Assert.IsFalse(TestManager.AbandonTest, "TestManager.AbandonTest should assert false after ShouldExecute method is run with ResumeTest");
        TestManager.ResetForNewTest();
    }
  }
}
