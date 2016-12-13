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
using Test.Helpers;

namespace Test.Helpers
{
    public static class FileAssert
    {
        static string GetFileHash(string filename)
        {
            Assert.IsTrue(File.Exists(filename));

            using (var hash = new SHA1Managed())
            {
                var clearBytes = File.ReadAllBytes(filename);
                var hashedBytes = hash.ComputeHash(clearBytes);
                return ConvertBytesToHex(hashedBytes);
            }
        }

        static string ConvertBytesToHex(byte[] bytes)
        {
            var sb = new StringBuilder();

            for (var i = 0; i < bytes.Length; i++)
            {
                sb.Append(bytes[i].ToString("x"));
            }
            return sb.ToString();
        }

        public static void AreEqual(string filename1, string filename2)
        {
            string hash1 = GetFileHash(filename1);
            string hash2 = GetFileHash(filename2);

            Assert.AreEqual(hash1, hash2);
        }
    }
}


namespace GenDocUnitTesting
{
    public static class TestConstants
    {
    public const String base_test_file_dir = @"S:\test\fixtures\files";

    }

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
    public class GenDocUnitTestingCSVFile
    {
        [TestMethod]
        public void TestSaveAsCSVOK()
        {
          String testfile = @TestConstants.base_test_file_dir + @"\test_excel_simplefilecontent.xls";
          String csv_op = @TestConstants.base_test_file_dir + @"\test_excel_simplefilecontent.csv";
          String testfile_expected = @TestConstants.base_test_file_dir + @"\refernce_test_excel_simplefilecontent.csv";
          GenDocUnitTesting.GenDoc g = new GenDocUnitTesting.GenDoc();
          Boolean res = g.SaveExcelXLSAsCSV(testfile);
          Assert.IsTrue(File.Exists(csv_op), "file not written");
          FileAssert.AreEqual(csv_op, testfile_expected);
        }
        public void TestSaveAsCSVErrorFileDoesNotExist() { Assert.Inconclusive(); }
        public void TestSaveAsCSVErrorFileAlreadyExists() { Assert.Inconclusive(); }

    }
    [TestClass]
    public class GenDocUnitTestingArgParsing
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
        [TestMethod]
        public void ParseArgsLessThan2()
        {
            GenDocUnitTesting.GenDoc g = new GenDocUnitTesting.GenDoc();
            String[] test_args = new String[1] { "hello" };
            String fn1 = "";
            String fn2 = "";
            Boolean res = g.ParseArgs(test_args, ref fn1, ref fn2);
            Assert.IsFalse(res, "ParseArgs is true");
        }
        [TestMethod]
        public void ParseArgsLessGreater3() { Assert.Inconclusive(); }
        [TestMethod]
        public void ParseArgsTeestMode() { Assert.Inconclusive(); }
    }
    [TestClass]
    public class GenDocUnitTestingOpenFile
    {
        [TestMethod]
        public void OpenExcelFile()
        {
            Assert.Inconclusive();
        }
        [TestMethod]
        public void OpenExcelFileErrorNoSuchFile()
        {
            Assert.Inconclusive();
        }
    }
    public class GenDocUnitTestingPopulateNamedCell
    {
        [TestMethod]
        // arg = filename, xml file
        // xml file is our populate format
        // populate excel file
        // test by opening file, saving as csv and comparing result
        public void PopulateNamedCells()
        {
            Assert.Inconclusive();
        }
        [TestMethod]
        public void PopulateNamedCellsErrorNoCellNAme()
        {
            Assert.Inconclusive();
        }
    }
}
