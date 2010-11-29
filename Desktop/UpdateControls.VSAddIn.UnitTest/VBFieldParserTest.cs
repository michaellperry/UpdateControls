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

namespace UpdateControls.VSAddIn.UnitTest
{
    [TestClass]
    public class VBFieldParserTest
    {
        private UpdateControls.VSAddIn.FieldParser _parser;

        [TestInitialize]
        public void TestInitialize()
        {
            _parser = new FieldParser(Language.VB);
        }

        [TestMethod]
        public void TestField()
        {
            _parser.Line = "\t\tPrivate _name As String = \"Aggie\"";

            Assert.AreEqual(true, _parser.IsAtom);
            Assert.AreEqual(false, _parser.IsCollection);
            Assert.AreEqual("String", _parser.Type);
            Assert.AreEqual("_name", _parser.Name);
        }

        [TestMethod]
        public void TestCollection()
        {
            _parser.Line = "\t\tPrivate _people As New List( Of Person )";

            Assert.AreEqual(false, _parser.IsAtom);
            Assert.AreEqual(true, _parser.IsCollection);
            Assert.AreEqual("Person", _parser.Type);
            Assert.AreEqual("List", _parser.Collection);
            Assert.AreEqual("_people", _parser.Name);
        }

        [TestMethod]
        public void TestDeepCollection()
        {
            _parser.Line = "\t\tPrivate _people As New System.Collections.Generics.List( Of Person )";

            Assert.AreEqual(false, _parser.IsAtom);
            Assert.AreEqual(true, _parser.IsCollection);
            Assert.AreEqual("Person", _parser.Type);
            Assert.AreEqual("System.Collections.Generics.List", _parser.Collection);
            Assert.AreEqual("_people", _parser.Name);
        }
    }
}
