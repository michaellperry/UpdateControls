/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2008 Mallard Software Designs
 * Licensed under LGPL
 * 
 * http://updatecontrols.net
 * http://www.codeplex.com/updatecontrols/
 * 
 **********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using UpdateControls.VSAddIn;

namespace UpdateControls.VSAddIn.UnitTest
{
    [TestFixture]
    public class UsingParserTest
    {
        private UsingParser _usingParser;

        [SetUp]
        public void SetUp()
        {
            _usingParser = new UsingParser();
        }

        [Test]
        public void TestNothing()
        {
            Assert.AreEqual(false, _usingParser.IsUsingUpdateControls);
            Assert.AreEqual(false, _usingParser.IsUsing);
            Assert.AreEqual(false, _usingParser.IsNamespace);
        }

        [Test]
        public void TestUpdateControls()
        {
            _usingParser.Line = "using UpdateControls;\r\n";
            Assert.AreEqual(true, _usingParser.IsUsingUpdateControls);
            Assert.AreEqual(false, _usingParser.IsUsing);
            Assert.AreEqual(false, _usingParser.IsNamespace);
        }

        [Test]
        public void TestWindowsForms()
        {
            _usingParser.Line = "using Windows.Forms;\r\n";
            Assert.AreEqual(false, _usingParser.IsUsingUpdateControls);
            Assert.AreEqual(true, _usingParser.IsUsing);
            Assert.AreEqual(false, _usingParser.IsNamespace);
        }

        [Test]
        public void TestUpdateControlsForms()
        {
            _usingParser.Line = "using UpdateControls.Forms;\r\n";
            Assert.AreEqual(false, _usingParser.IsUsingUpdateControls);
            Assert.AreEqual(true, _usingParser.IsUsing);
            Assert.AreEqual(false, _usingParser.IsNamespace);
        }

        [Test]
        public void TestBlank()
        {
            _usingParser.Line = "\r\n";
            Assert.AreEqual(false, _usingParser.IsUsingUpdateControls);
            Assert.AreEqual(false, _usingParser.IsUsing);
            Assert.AreEqual(false, _usingParser.IsNamespace);
        }

        [Test]
        public void TestNamespace()
        {
            _usingParser.Line = "namespace MyApp\r\n";
            Assert.AreEqual(false, _usingParser.IsUsingUpdateControls);
            Assert.AreEqual(false, _usingParser.IsUsing);
            Assert.AreEqual(true, _usingParser.IsNamespace);
        }
    }
}
