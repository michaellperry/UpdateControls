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

namespace UpdateControls.VSAddIn.UnitTest
{
    [TestFixture]
    public class VBFieldParserTest
    {
        private UpdateControls.VSAddIn.FieldParser _parser;

        [SetUp]
        public void SetUp()
        {
            _parser = new FieldParser(Language.VB);
        }

        [Test]
        public void TestField()
        {
            _parser.Line = "\t\tPrivate _name As String = \"Aggie\"";

            Assert.AreEqual(true, _parser.IsAtom);
            Assert.AreEqual(false, _parser.IsCollection);
            Assert.AreEqual("String", _parser.Type);
            Assert.AreEqual("_name", _parser.Name);
        }

        [Test]
        public void TestCollection()
        {
            _parser.Line = "\t\tPrivate _people As New List( Of Person )";

            Assert.AreEqual(false, _parser.IsAtom);
            Assert.AreEqual(true, _parser.IsCollection);
            Assert.AreEqual("Person", _parser.Type);
            Assert.AreEqual("List", _parser.Collection);
            Assert.AreEqual("_people", _parser.Name);
        }

        [Test]
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
