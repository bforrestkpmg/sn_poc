﻿using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    }
 }
