using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace get_data
{
    class Program
    {
        static void parse_args(String[] args)
        {
            if (args.Length < 2) {
                System.Console.WriteLine("Usage:: <url/filename> <section header> [-test]");
                Environment.Exit(0);
            }
            // have we specified test mode we genereate specific output based on file name
            if ((!String.IsNullOrEmpty(args[2])) && (args[2].Equals("-test")))
            {
               // output_test_responses_based_on_file_name(arg[0]);
                Environment.Exit(0);
            }
        }
        static void Main(string[] args)
        {
            parse_args(args);
        }
    }
}
