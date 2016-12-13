using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;

namespace GenDocUnitTesting
{
    [TestClass]
    public class SupportFunctions
    {
        [TestMethod]
        public void TestGetTempFile()
        {
            GenDocUnitTesting.GenDoc g = new GenDocUnitTesting.GenDoc();
            String res = g.GetTempFile(".blah");
            Assert.IsNotNull(res, "Get Temp file not returning filename");
            Assert.IsTrue(res.EndsWith(".blah"), "filename incorrect extension");
        }

        [TestMethod]
        public void TestGetExtOK()
        {
            GenDocUnitTesting.GenDoc g = new GenDocUnitTesting.GenDoc();
            String res = g.GetExt("hello.there");
            Assert.IsNotNull(res, "Get Ext not returning filename");
            Assert.IsTrue(res.EndsWith(".there"), "filename incorrect extension");
        }

        [TestMethod]
        public void TestGetExthandlesblank()
        {
            GenDocUnitTesting.GenDoc g = new GenDocUnitTesting.GenDoc();
            String res = g.GetExt("hello");
            Assert.IsNotNull(res, "Get Ext not returning filename");
            Assert.AreEqual(res, "", "filename incorrect extension");
        }

    } // Test Support Functions
    [TestClass]
    public class GenDocUnitTestingOpenFile
    {
        [TestMethod]
        public void ParseArgsOK()
        {
            GenDocUnitTesting.GenDoc g = new GenDocUnitTesting.GenDoc();
            String[] test_args = new String[2] { "hello", "there" };
            String fn1 = "";
            String fn2 = "";
            Boolean res = g.ParseArgs(test_args, ref fn1, ref fn2);
            Assert.IsTrue(res, "ParseArgs is false");
            Assert.AreEqual(fn1, "hello", "first arg incorrect");
            Assert.AreEqual(fn2, "there", "first arg incorrect");
        }
    }
}
