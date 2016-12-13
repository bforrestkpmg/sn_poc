using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using Excel;
using System.Data;

namespace GenDocUnitTesting
{


    class GenDoc
    {

        // used for testing to check output file. saves as same file + csv
        // TODO check previous file
        // TODO check / catch file exists
        // based on https://www.codeproject.com/articles/246772/convert-xlsx-xls-to-csv
        // using  https://github.com/ExcelDataReader/ExcelDataReader (install with nuget ExcelDataReader)
        // ONLY works for single first sheet
        public Boolean SaveExcelXLSAsCSV(String filename)
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
                csvData += "\n";
            }
            StreamWriter csv = new StreamWriter(@csv_file_name, false);
            csv.Write(csvData);
            csv.Close();
            return (true);
        }
        public string GetTempFile(string ext)
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

            if (args.Length < 2)
            {
                System.Console.WriteLine("Usage:: <excel filename> <xml_file> [-test]");
            }
            // have we specified test mode we genereate specific output based on file name
            else if (args.Length >= 3)
            {
                if ((!String.IsNullOrEmpty(args[2])) && (args[2].Equals("-test")))
                {
                    ret = true;
                }
            }
            else
            {
                excel_filename = args[0];
                xml_file = args[1];
                ret = true;
            }
            return (ret);
        }
    }
}
