using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace GenDocUnitTesting
{


    class GenDoc
    {
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
