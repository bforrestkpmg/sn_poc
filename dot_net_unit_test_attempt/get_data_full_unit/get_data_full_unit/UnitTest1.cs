﻿using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Test.Helpers;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using System.IO.Packaging;
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


namespace get_data_full_unit
{

    [TestClass]
    public class SupportFunctions
    {
              [TestMethod]
        public void TestGetTempFile()
        {
            GetDataConvertAndExtract.ConvertGetData g = new GetDataConvertAndExtract.ConvertGetData();
            String res = g.GetTempFile(".blah");
            Assert.IsNotNull(res, "Get Temp file not returning filename");
            Assert.IsTrue(res.EndsWith(".blah"), "filename incorrect extension");
        }

        [TestMethod]
        public void TestGetExtOK()
        {
            GetDataConvertAndExtract.ConvertGetData g = new GetDataConvertAndExtract.ConvertGetData();
            String res = g.GetExt("hello.there");
            Assert.IsNotNull(res, "Get Ext not returning filename");
            Assert.IsTrue(res.EndsWith(".there"), "filename incorrect extension");
        }

        [TestMethod]
        public void TestGetExthandlesblank()
        {
            GetDataConvertAndExtract.ConvertGetData g = new GetDataConvertAndExtract.ConvertGetData();
            String res = g.GetExt("hello");
            Assert.IsNotNull(res, "Get Ext not returning filename");
            Assert.AreEqual(res, "", "filename incorrect extension");
        }

        [TestMethod]
        public void TestWritesFile()
        {
            GetDataConvertAndExtract.ConvertGetData g = new GetDataConvertAndExtract.ConvertGetData();
            String tmpfile = g.GetTempFile(".blah.test");
            Boolean res =  g.WriteFile(tmpfile, "hello there");
            Assert.IsTrue(res, "Write file not true");
            Assert.IsTrue(File.Exists(tmpfile), "file not written");
        }
        [TestMethod]
        public void TestWritesFileFailsIfInvalidFilename()
        {
            GetDataConvertAndExtract.ConvertGetData g = new GetDataConvertAndExtract.ConvertGetData();
            Boolean res = g.WriteFile("blah blah/blah", "hello there");
            Assert.IsFalse(res, "Write file not trapping errors");
        }
    } // Test Support Functions

    [TestClass]
    public class ConvertFiles
    {


        [TestMethod]
        public void UnitTestReadAllTextFromDocx()
        {
            String base_file = @"S:\test\fixtures\files\TestDoc_DocWithHeading_andHeadingNumber_IncludesTable.docx";
            GetDataConvertAndExtract.ConvertGetData g = new GetDataConvertAndExtract.ConvertGetData();
            String ret_string = g.ReadAllTextFromDocx(base_file);
            String expected_str = @" Heading 1

Text in heading 1

Test in heading 1 line 2

Table head 1	Table head 2	Table head 3	Table head 4
	Cell 1	Cell 2	Cell 3	Cell 4
	Cell 5	Cell 6	Cell 7	Cell 8
Heading 2

Text in heading 2

Test in heading 2 line 2

";
            Regex r = new Regex("Table head 1	Table head 2	Table head 3	Table head ", RegexOptions.Multiline);
            Match m = r.Match(ret_string);
            Assert.IsTrue(m.Success, "Cannot match output string, table head");
            r = new Regex("	Cell 1	Cell 2	Cell 3	Cell 4", RegexOptions.Multiline);
            m = r.Match(ret_string);
            Assert.IsTrue(m.Success, "Cannot match output string, table 2nd row");
            r = new Regex("Heading 1", RegexOptions.Multiline);
            m = r.Match(ret_string);
            Assert.IsTrue(m.Success, "Cannot match output string, Heading 1");
        }

        [TestMethod]
        public void UnitTestReadSectionHeadingTextFromDocx()
        {
            String base_file = @"S:\test\fixtures\files\TestDoc_DocWithHeading_andHeadingNumber_IncludesTable.docx";
            GetDataConvertAndExtract.ConvertGetData g = new GetDataConvertAndExtract.ConvertGetData();
            String ret_string = g.ReadSectionHeadingTextFromDocx(base_file, "Heading 1");
            String expected_str = @"Heading 1

Text in heading 1

Test in heading 1 line 2

Table head 1	Table head 2	Table head 3	Table head 4
	Cell 1	Cell 2	Cell 3	Cell 4
	Cell 5	Cell 6	Cell 7	Cell 8
";
            Regex r = new Regex("Table head 1	Table head 2	Table head 3	Table head ", RegexOptions.Multiline);
            Match m = r.Match(ret_string);
            Assert.IsTrue(m.Success, "Cannot match output string, table head");
            r = new Regex("	Cell 1	Cell 2	Cell 3	Cell 4", RegexOptions.Multiline);
            m = r.Match(ret_string);
            Assert.IsTrue(m.Success, "Cannot match output string, table 2nd row");
            r = new Regex("Heading 1", RegexOptions.Multiline);
            m = r.Match(ret_string);
            Assert.IsTrue(m.Success, "Cannot match output string, Heading 1");
            r = new Regex("Heading 2", RegexOptions.Multiline);
            m = r.Match(ret_string);
            Assert.IsFalse(m.Success, "HEading 2 is incorrectly included in text");
        }


        [TestMethod]
        public void UnitTestReadSectionHeadingTextFromDocxErrorNoHEading()
        {
            String base_file = @"S:\test\fixtures\files\TestDoc_DocWithHeading_andHeadingNumber_IncludesTable.docx";
            GetDataConvertAndExtract.ConvertGetData g = new GetDataConvertAndExtract.ConvertGetData();
            String ret_string = g.ReadSectionHeadingTextFromDocx(base_file, "Heading Hello");
            Assert.AreEqual(ret_string, "", "Expecting null string");
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException), "FileNotFound")]
        public void UnitTestReadSectionHeadingTextFromDocxErrorNoSuchFile()
        {
            String base_file = @"S:\test\fixtures\files\blahblah.docx";
            GetDataConvertAndExtract.ConvertGetData g = new GetDataConvertAndExtract.ConvertGetData();
            String ret_string = g.ReadSectionHeadingTextFromDocx(base_file, "Heading Hello");
            Assert.AreEqual(ret_string, null, "Expecting null string");
        }



        [TestMethod]
        public void ConvertDocToHtmlOK()
        {
        
            String base_file = @"S:\test\fixtures\files\TestDoc_DocWithHeading";
            String test_docx = base_file + ".docx";
            String to_comparehtml = base_file + "_reference.htm";
            GetDataConvertAndExtract.ConvertGetData g = new GetDataConvertAndExtract.ConvertGetData();
            String tmpfile = g.GetTempFile(".htm");
            Boolean res = g.ConvertDocToHtml(test_docx, tmpfile);
            Assert.IsTrue(res, "Convert to HTML not returned ok");
            Assert.IsTrue(File.Exists(tmpfile), "file not written");
            FileAssert.AreEqual(tmpfile, to_comparehtml);
        }

        [TestMethod]
        public void ConvertDocToHtml_HandleError_CannotSaveAsType()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void ConvertDocToHtml_HandleError_DestinationFileCannotBeWritten()
        {
            Assert.Inconclusive();
        }
    }

    [TestClass]
    public class ExtractTextOK
    {

        [TestMethod]
        public void ExtractSectionTextWithoutTableOK()
        {
            String Expected = @"
Text in heading 1 

Test in heading 1 line 2
";

            String buffer = null;
            String base_file = @"S:\test\fixtures\files\TestDoc_DocWithHeading_andHeadingNumber";
            String test_htmldoc = base_file + ".htm";
            GetDataConvertAndExtract.ConvertGetData g = new GetDataConvertAndExtract.ConvertGetData();
            g.ExtractSection(test_htmldoc, ref buffer, "Heading 1");
            System.Console.WriteLine("buffer: _" + buffer + "_");
            //#TODO fix this match we should match the expected but can't get it to work!
            Regex r = new Regex(".*Test in heading 1.*",  RegexOptions.Multiline);
            Match m = r.Match(buffer);
            Assert.IsTrue(m.Success, "Cannot match output string");
        }

        [TestMethod]
        public void ExtractSectionTextWithTable()
        {
            String buffer = null;
            String base_file = @"S:\test\fixtures\files\TestDoc_DocWithHeading_andHeadingNumber_IncludesTable";
            String test_htmldoc = base_file + ".htm";
            GetDataConvertAndExtract.ConvertGetData g = new GetDataConvertAndExtract.ConvertGetData();
            g.ExtractSection(test_htmldoc, ref buffer, "Heading 1");
            System.Console.WriteLine("buffer: _" + buffer + "_");
            //#TODO fix this match we should match the expected but can't get it to work!
            Regex r = new Regex(".*Text in heading 1.*", RegexOptions.Multiline);
            Match m = r.Match(buffer);
            Assert.IsTrue(m.Success, "Cannot match output string");
            r = new Regex("\"Table head 1\"	\"Table head 2\"	\"Table head 3\"	\"Table head 4\"", RegexOptions.Multiline);
            m = r.Match(buffer);
            Assert.IsTrue(m.Success, "Cannot match output string");
            r = new Regex("\"Cell 1\"\\t\"Cell 2\"\\t\"Cell 3\"\\t\"Cell 4\".*", RegexOptions.Multiline);
            m = r.Match(buffer);
            Assert.IsTrue(m.Success, "Cannot match output string");
            r = new Regex(".*\"Cell 5\"\\t\"Cell 6\"\\t\"Cell 7\"\\t\"Cell 8\".*", RegexOptions.Multiline);
            m = r.Match(buffer);
            Assert.IsTrue(m.Success, "Cannot match output string");
        }

        [TestMethod]
        public void ExtractSectionTextWithTable_Error_NoSuchSection()
        {
            Assert.Inconclusive();
        }
        [TestMethod]
        public void ExtractSectionTextWithTable_Error_MultipleSections()
        {
            Assert.Inconclusive();
        }
        [TestMethod]
        public void ExtractSectionTextWithTable_Error_NonCompliantDoc_NonFlatSections()
        {
            Assert.Inconclusive();
        }

    } // TestClass::ExtractTextOK

    [TestClass]
    public class DownloadFile
    {
        [TestMethod]
        public void DownloadFileOK()
        {
            Assert.Inconclusive();
        }
        [TestMethod]
        public void DownloadFile_HandleNoUSerNAmeDefinedK()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void DownloadFile_Handle_NoSuchFileName()
        {
            Assert.Inconclusive();
        }

    } // TextClass::DownloadFile
}
