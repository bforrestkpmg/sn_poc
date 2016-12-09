using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GetDataConvertAndExtract;



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
    }
}
