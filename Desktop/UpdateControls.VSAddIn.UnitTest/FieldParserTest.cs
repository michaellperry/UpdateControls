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
    public class FieldParserTest
    {
        private UpdateControls.VSAddIn.FieldParser _parser;

        [TestInitialize]
        public void TestInitialize()
        {
            _parser = new FieldParser(Language.CS);
        }

        [TestMethod]
        public void TestNone()
        {
            Assert.AreEqual(false, _parser.IsAtom);
            Assert.AreEqual(false, _parser.IsCollection);
        }

		[TestMethod]
        public void TestAtom()
        {
            _parser.Line = "\t\tprivate int _number";
            Assert.AreEqual(true, _parser.IsAtom);
            Assert.AreEqual(false, _parser.IsCollection);
            Assert.AreEqual("int", _parser.Type);
            Assert.AreEqual("_number", _parser.Name);
        }

		[TestMethod]
		public void TestNullable()
		{
			_parser.Line = "\t\tprivate int? _number";
			Assert.AreEqual(true, _parser.IsAtom);
			Assert.AreEqual(false, _parser.IsCollection);
			Assert.AreEqual("int?", _parser.Type);
			Assert.AreEqual("_number", _parser.Name);
		}

		[TestMethod]
        public void TestCollection()
        {
            _parser.Line = "\t\tprivate List<int> _numbers";
            Assert.AreEqual(false, _parser.IsAtom);
            Assert.AreEqual(true, _parser.IsCollection);
            Assert.AreEqual("List", _parser.Collection);
            Assert.AreEqual("int", _parser.Type);
            Assert.AreEqual("_numbers", _parser.Name);
        }

		[TestMethod]
        public void TestCollectionOfNullable()
        {
            _parser.Line = "\t\tprivate List<int?> _numbers";
            Assert.AreEqual(false, _parser.IsAtom);
            Assert.AreEqual(true, _parser.IsCollection);
            Assert.AreEqual("List", _parser.Collection);
            Assert.AreEqual("int?", _parser.Type);
            Assert.AreEqual("_numbers", _parser.Name);
        }

		[TestMethod]
        public void TestAtomNoModifier()
        {
            _parser.Line = "\t\tint _number";
            Assert.AreEqual(true, _parser.IsAtom);
            Assert.AreEqual(false, _parser.IsCollection);
            Assert.AreEqual("int", _parser.Type);
            Assert.AreEqual("_number", _parser.Name);
        }

		[TestMethod]
		public void TestNullableNoModifier()
		{
			_parser.Line = "\t\tint? _number";
			Assert.AreEqual(true, _parser.IsAtom);
			Assert.AreEqual(false, _parser.IsCollection);
			Assert.AreEqual("int?", _parser.Type);
			Assert.AreEqual("_number", _parser.Name);
		}

		[TestMethod]
        public void TestCollectionNoModifier()
        {
            _parser.Line = "\t\tList<int> _numbers";
            Assert.AreEqual(false, _parser.IsAtom);
            Assert.AreEqual(true, _parser.IsCollection);
            Assert.AreEqual("List", _parser.Collection);
            Assert.AreEqual("int", _parser.Type);
            Assert.AreEqual("_numbers", _parser.Name);
        }

		[TestMethod]
        public void TestDeepCollectionNoModifier()
        {
            _parser.Line = "\t\tSystem.Collections.Generics.List<int> _numbers";
            Assert.AreEqual(false, _parser.IsAtom);
            Assert.AreEqual(true, _parser.IsCollection);
            Assert.AreEqual("System.Collections.Generics.List", _parser.Collection);
            Assert.AreEqual("int", _parser.Type);
            Assert.AreEqual("_numbers", _parser.Name);
        }

		[TestMethod]
		public void TestCollectionOfNullableNoModifier()
		{
			_parser.Line = "\t\tList<int?> _numbers";
			Assert.AreEqual(false, _parser.IsAtom);
			Assert.AreEqual(true, _parser.IsCollection);
			Assert.AreEqual("List", _parser.Collection);
			Assert.AreEqual("int?", _parser.Type);
			Assert.AreEqual("_numbers", _parser.Name);
		}
	}
}
