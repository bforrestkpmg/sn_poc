using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using Excel;
using System.Data;
using ClosedXML;
using ClosedXML.Excel;

namespace GenDocUnitTesting
{
    public struct RangeValuePair
    {
        public String RangeName;
        public String TheValue;

        public RangeValuePair(String r, String v) { this.TheValue = v;
            this.RangeName = r; }

        public static bool operator ==(RangeValuePair r1, RangeValuePair r2)
        {
            return ((r1.RangeName == r2.RangeName) && (r1.TheValue  == r2.TheValue ));
        }

        public static bool operator !=(RangeValuePair r1, RangeValuePair r2)
        {
            return ((r1.RangeName != r2.RangeName) || (r1.TheValue != r2.TheValue));
        }
    }

    class GenDoc
    {

        // used for testing to check output file. saves as same file + csv
        // TODO check previous file
        // TODO check / catch file exists
        // based on https://www.codeproject.com/articles/246772/convert-xlsx-xls-to-csv
        // using  https://github.com/ExcelDataReader/ExcelDataReader (install with nuget ExcelDataReader)
        // ONLY works for single first sheet
        public  Boolean SaveExcelXLSAsCSV(String filename)
        {
            //String filename_only = Path.GetFileName(filename);
            //String filename_no_extention = Path.GetFileNameWithoutExtension(filename_only);
            String dir = Path.GetDirectoryName(filename);
 String without_extension = @dir + @"\" + Path.GetFileNameWithoutExtension(@filename);
//String without_extension = dir + @"\" + filename_no_extention;
            String csv_file_name = @without_extension + ".csv";
            FileStream stream = File.Open(@filename, FileMode.Open, FileAccess.Read);

            // Reading from a binary Excel file ('97-2003 format; *.xls)
            IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);

            // DataSet - The result of each spreadsheet will be created in the result.Tables
            DataSet result = excelReader.AsDataSet();

            // Free resources (IExcelDataReader is IDisposable)
            excelReader.Close();
            string csvData = "";
            int row_no = 0;
            int ind = 0; // TABLE index = sheet

            while (row_no < result.Tables[ind].Rows.Count) // ind is the index of table
                                                           // (sheet name) which you want to convert to csv
            {
                for (int i = 0; i < result.Tables[ind].Columns.Count; i++)
                {
                    csvData += result.Tables[ind].Rows[row_no][i].ToString();
                    if (row_no < result.Tables[ind].Rows.Count-1)
                    {
                        csvData += ",";
                    }
                }
                row_no++;
                csvData += Environment.NewLine;
            }
            StreamWriter csv = new StreamWriter(@csv_file_name, false);
            csv.Write(csvData);
            csv.Close();
            return (true);
        }
        // based on
        // http://stackoverflow.com/questions/28503437/most-efficient-way-of-coverting-a-datatable-to-csv
        public String DataTableToCSV(DataTable datatable, char seperator)
        {
            int row_count = 0;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < datatable.Columns.Count; i++)
            {
                sb.Append(datatable.Columns[i]);
                if (i < datatable.Columns.Count - 1)
                    sb.Append(seperator);
            }
            //sb.AppendLine();
            sb.Append(Environment.NewLine);
            foreach (DataRow dr in datatable.Rows)
            {
                row_count += 1;
                for (int i = 0; i < datatable.Columns.Count; i++)
                {
                    sb.Append(dr[i].ToString());

                    if (i < datatable.Columns.Count - 1)
                        sb.Append(seperator);
                }
                if (row_count < datatable.Rows.Count)
                {
            sb.Append(Environment.NewLine);
                }
            }
            return sb.ToString();
        }

        // https://msdn.microsoft.com/en-us/library/office/hh298534.aspx
        public  Boolean SaveExcelXLSXAsCSV(String filename, int SheetNum = 1)
        {
            String dir = Path.GetDirectoryName(filename);
            String without_extension = @dir + @"\" + Path.GetFileNameWithoutExtension(@filename);
            String csv_file_name = @without_extension + ".csv";
            //Create a new DataTable.
            DataTable dt = new DataTable();
            using (XLWorkbook workBook = new XLWorkbook(@filename) )
            {
                IXLWorksheet workSheet = workBook.Worksheet(SheetNum);
                //Loop through the Worksheet rows.
                bool firstRow = true;
                foreach (IXLRow row in workSheet.Rows())
                {
                    //Use the first row to add columns to DataTable.
                    if (firstRow)
                    {
                        foreach (IXLCell cell in row.Cells())
                        {
                            dt.Columns.Add(cell.Value.ToString());
                        }
                        firstRow = false;
                    }
                    else
                    {
                        //Add rows to DataTable.
                        dt.Rows.Add();
                        int i = 0;
                        foreach (IXLCell cell in row.Cells())
                        {
                            dt.Rows[dt.Rows.Count - 1][i] = cell.Value.ToString();
                            i++;
                        }
                    }
                }
            }   //using

            String csvData = "";
            csvData = DataTableToCSV(dt, ',');
            StreamWriter csv = new StreamWriter(@csv_file_name, false);
            csv.Write(csvData);
            csv.Close();
            return (true);
        }

        //nOTWORKING
        public Boolean UpdateXLSRange(String filename, String newfilename, String range_name, String value)
        {
            return(false);
;
        }

        public Boolean UpdateXLSXRange(String filename, String newname, String range_name, String value)
        {
            var workbook = new XLWorkbook(@filename);
            var worksheet = workbook.Worksheets.Worksheet(1);
            worksheet.Cell(range_name).SetValue(value);
            workbook.SaveAs(@newname);
            return (true);
        }
        public Boolean UpdateXLSXRange(String filename, String newname, List<RangeValuePair> ranges_xml, int worksheet_num = 1)
        {
            var workbook = new XLWorkbook(@filename);
            var worksheet = workbook.Worksheets.Worksheet(worksheet_num);
            //foreach (RangeValuePair item in ranges_xml)
            //{
            //    worksheet.Cell(item.RangeName).SetValue(item.TheValue);
            //}
            workbook.SaveAs(@newname);
            return (true);
        }

        public  string GetTempFile(string ext)
        {
            string fileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ext;
            return fileName;
        }
        //returns extension
        public string GetExt(String filename)
        {
            return Path.GetExtension(filename);
        }
        public static void op_text(String s)
        {
            System.Diagnostics.Debug.WriteLine(s);
            Console.WriteLine(s);
        }
        
        public Boolean ParseArgs(String[] args, ref String excel_filename, ref String xml_file)
        {
            Boolean ret = false;

            if (args.Length < 2) {
                System.Console.WriteLine("Usage:: <excel filename> <xml_file> [-test]");
            }
            // have we specified test mode we genereate specific output based on file name
            else if (args.Length >= 3)
            {
                if ((!String.IsNullOrEmpty(args[2])) && (args[2].Equals("-test"))) {
                    ret = true;
                }
            }
            else {
                excel_filename = args[0];
                xml_file = args[1];
                ret = true;
            }
            return (ret);
        }

        public List<RangeValuePair>  generate_array_of_values(String ip_xml)
        {

            List<RangeValuePair> n = new List<RangeValuePair>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(ip_xml);
            string xpath = "/populate/*";

            var nodes = xmlDoc.SelectNodes(xpath);  
            foreach (XmlNode childrenNode in nodes)
            {
                op_text(childrenNode.SelectSingleNode(".//range_name").InnerText);
                op_text(childrenNode.SelectSingleNode(".//value").InnerText);
                 n.Add(new RangeValuePair() { RangeName = childrenNode.SelectSingleNode(".//range_name").InnerText, TheValue = childrenNode.SelectSingleNode(".//value").InnerText });
            }
            return (n);
        } //RangeValuePair
    }
}
