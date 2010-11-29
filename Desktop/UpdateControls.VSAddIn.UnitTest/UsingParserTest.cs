/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2010 Michael L Perry
 * MIT License
 * 
 * http://updatecontrols.net
 * http://updatecontrols.codeplex.com/
 * 
 **********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.VSAddIn;

namespace UpdateControls.VSAddIn.UnitTest
{
    [TestClass]
    public class UsingParserTest
    {
        private UsingParser _usingParser;

        [TestInitialize]
        public void TestInitialize()
        {
            _usingParser = new UsingParser();
        }

        [TestMethod]
        public void TestNothing()
        {
            Assert.AreEqual(false, _usingParser.IsUsingUpdateControls);
            Assert.AreEqual(false, _usingParser.IsUsing);
            Assert.AreEqual(false, _usingParser.IsNamespace);
        }

        [TestMethod]
        public void TestUpdateControls()
        {
            _usingParser.Line = "using UpdateControls;\r\n";
            Assert.AreEqual(true, _usingParser.IsUsingUpdateControls);
            Assert.AreEqual(false, _usingParser.IsUsing);
            Assert.AreEqual(false, _usingParser.IsNamespace);
        }

        [TestMethod]
        public void TestWindowsForms()
        {
            _usingParser.Line = "using Windows.Forms;\r\n";
            Assert.AreEqual(false, _usingParser.IsUsingUpdateControls);
            Assert.AreEqual(true, _usingParser.IsUsing);
            Assert.AreEqual(false, _usingParser.IsNamespace);
        }

        [TestMethod]
        public void TestUpdateControlsForms()
        {
            _usingParser.Line = "using UpdateControls.Forms;\r\n";
            Assert.AreEqual(false, _usingParser.IsUsingUpdateControls);
            Assert.AreEqual(true, _usingParser.IsUsing);
            Assert.AreEqual(false, _usingParser.IsNamespace);
        }

        [TestMethod]
        public void TestBlank()
        {
            _usingParser.Line = "\r\n";
            Assert.AreEqual(false, _usingParser.IsUsingUpdateControls);
            Assert.AreEqual(false, _usingParser.IsUsing);
            Assert.AreEqual(false, _usingParser.IsNamespace);
        }

        [TestMethod]
        public void TestNamespace()
        {
            _usingParser.Line = "namespace MyApp\r\n";
            Assert.AreEqual(false, _usingParser.IsUsingUpdateControls);
            Assert.AreEqual(false, _usingParser.IsUsing);
            Assert.AreEqual(true, _usingParser.IsNamespace);
        }
    }
}
