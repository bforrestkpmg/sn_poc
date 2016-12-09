using System;
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
    }
}
