using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Collections.Generic;
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
    class OP
    {
        public void op_text(String s)
        {
            System.Diagnostics.Debug.WriteLine(s);
            Console.WriteLine(s);
        }
    }

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
        public void TestSaveXLSAsCSVOK()
        {
          String testfile = @TestConstants.base_test_file_dir + @"\test_excel_simplefilecontent.xls";
          String csv_op = @TestConstants.base_test_file_dir + @"\test_excel_simplefilecontent.csv";
          String testfile_expected = @TestConstants.base_test_file_dir + @"\refernce_test_excel_simplefilecontent.csv";
          GenDocUnitTesting.GenDoc g = new GenDocUnitTesting.GenDoc();
          Boolean res = g.SaveExcelXLSAsCSV(testfile);
          Assert.IsTrue(File.Exists(csv_op), "file not written");
          FileAssert.AreEqual(csv_op, testfile_expected);
        }
        [TestMethod]
        public void TestSaveAsCSVErrorFileDoesNotExist() { Assert.Inconclusive(); }
        [TestMethod]
        public void TestSaveAsCSVErrorFileAlreadyExists() { Assert.Inconclusive(); }

        [TestMethod]
        public void TestSaveXLSXAsCSVOK()
        {
            String testfile = @TestConstants.base_test_file_dir + @"\test_excel_simplefilecontent.xlsx";
            String csv_op = @TestConstants.base_test_file_dir + @"\test_excel_simplefilecontent.csv";
            String testfile_expected = @TestConstants.base_test_file_dir + @"\refernce_test_excel_simplefilecontent.csv";
            GenDocUnitTesting.GenDoc g = new GenDocUnitTesting.GenDoc();
            Boolean res = g.SaveExcelXLSXAsCSV(testfile);
            Assert.IsTrue(File.Exists(csv_op), "file not written");
            FileAssert.AreEqual(csv_op, testfile_expected);
        }


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
    [TestClass]
    public class GenDocUnitTestingPopulateNamedCell
    {
        // arg = filename, xml file
        // xml file is our populate format
        // populate excel file
        // test by opening file, saving as csv and comparing result
        [TestMethod]
        public void TestPopulateNamedCells()
        {
            String testfile = @TestConstants.base_test_file_dir + @"\Basic_Excel_2cells.xlsx";
            String ref_opput= @TestConstants.base_test_file_dir + @"\reference_Basic_Excel_2cells.csv";
            String new_file = @TestConstants.base_test_file_dir + @"\testgenerated_file_Basic_Excel_2cells.xlsx";
            String new_file_csv = @TestConstants.base_test_file_dir + @"\testgenerated_file_Basic_Excel_2cells.csv";
            GenDocUnitTesting.GenDoc g = new GenDocUnitTesting.GenDoc();
             Boolean res = g.UpdateXLSXRange(testfile, new_file, "cell1rangename", "newvalue1");
            //Boolean res = g.UpdateXLSXRange(testfile, "cell2rangename", "newvalue2");
            res = g.SaveExcelXLSXAsCSV(new_file);
            FileAssert.AreEqual(new_file_csv, ref_opput);
            Assert.IsTrue(res, "update xlsx range is false");
        }
        [TestMethod]
        public void PopulateNamedCellsErrorNoCellName()
        {
            Assert.Inconclusive();
        }
    }
    [TestClass]
    public class GenDocUnitTestinParseXMLSetofRanges
    {
        [TestMethod]
        public void ParseSimpleXMLOK()
        {
            String ip_xml = @"<populate><item><range_name>cell1rangename</range_name><value>newvalue1</value></item></populate>";
            List<RangeValuePair> n = new List<RangeValuePair>();
            n.Add(new RangeValuePair() { RangeName = "cell1rangename", TheValue = "newvalue1" });
            GenDocUnitTesting.GenDoc g = new GenDocUnitTesting.GenDoc();
            var PopulationValues = g.generate_array_of_values(ip_xml);
            //            Boolean res = g.PopulateXLSXBasedOnMultipleRanges(testfile, new_file, "cell1rangename", "newvalue1");
            Assert.IsNotNull(PopulationValues);
            Assert.AreEqual(PopulationValues[0], n[0], "arays not the same  - content");
            Assert.AreEqual(PopulationValues.Count, n.Count, "arays not the same - count");
        }
        [TestMethod]
        public void ParseSimpleXMLErrorMalFormedXML()
        {
            Assert.Inconclusive();
        }
    }

}
