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
    public class FieldParserTest
    {
        private UpdateControls.VSAddIn.FieldParser _parser;

        [SetUp]
        public void SetUp()
        {
            _parser = new FieldParser(Language.CS);
        }

        [Test]
        public void TestNone()
        {
            Assert.AreEqual(false, _parser.IsAtom);
            Assert.AreEqual(false, _parser.IsCollection);
        }

        [Test]
        public void TestAtom()
        {
            _parser.Line = "\t\tprivate int _number";
            Assert.AreEqual(true, _parser.IsAtom);
            Assert.AreEqual(false, _parser.IsCollection);
            Assert.AreEqual("int", _parser.Type);
            Assert.AreEqual("_number", _parser.Name);
        }

        [Test]
        public void TestCollection()
        {
            _parser.Line = "\t\tprivate List<int> _numbers";
            Assert.AreEqual(false, _parser.IsAtom);
            Assert.AreEqual(true, _parser.IsCollection);
            Assert.AreEqual("List", _parser.Collection);
            Assert.AreEqual("int", _parser.Type);
            Assert.AreEqual("_numbers", _parser.Name);
        }

        [Test]
        public void TestAtomNoModifier()
        {
            _parser.Line = "\t\tint _number";
            Assert.AreEqual(true, _parser.IsAtom);
            Assert.AreEqual(false, _parser.IsCollection);
            Assert.AreEqual("int", _parser.Type);
            Assert.AreEqual("_number", _parser.Name);
        }

        [Test]
        public void TestCollectionNoModifier()
        {
            _parser.Line = "\t\tList<int> _numbers";
            Assert.AreEqual(false, _parser.IsAtom);
            Assert.AreEqual(true, _parser.IsCollection);
            Assert.AreEqual("List", _parser.Collection);
            Assert.AreEqual("int", _parser.Type);
            Assert.AreEqual("_numbers", _parser.Name);
        }

        [Test]
        public void TestDeepCollectionNoModifier()
        {
            _parser.Line = "\t\tSystem.Collections.Generics.List<int> _numbers";
            Assert.AreEqual(false, _parser.IsAtom);
            Assert.AreEqual(true, _parser.IsCollection);
            Assert.AreEqual("System.Collections.Generics.List", _parser.Collection);
            Assert.AreEqual("int", _parser.Type);
            Assert.AreEqual("_numbers", _parser.Name);
        }
    }
}
