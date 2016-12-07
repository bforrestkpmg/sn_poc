using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using Word=Microsoft.Office.Interop.Word;
using HtmlAgilityPack;

namespace ConvertDocFiles
{
    class Program
    {

        public static string GetTempFile(string ext)
        {
            string fileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ext;
            return fileName;
        }
        //returns extension
        public static string GetExt(String filename)
        {
            return Path.GetExtension(filename);
        }
        
        public static void ConvertDocToHtml(object Sourcepath, object TargetPath)
        {

            Word._Application newApp = new Word.Application();
            Word.Documents d = newApp.Documents;
            object Unknown = Type.Missing;
            Word.Document od = d.Open(ref Sourcepath, ref Unknown,
                                     ref Unknown, ref Unknown, ref Unknown,
                                     ref Unknown, ref Unknown, ref Unknown,
                                     ref Unknown, ref Unknown, ref Unknown,
                                     ref Unknown, ref Unknown, ref Unknown, ref Unknown);
            object format = Word.WdSaveFormat.wdFormatHTML;

            newApp.ActiveDocument.SaveAs(ref TargetPath, ref format,
                        ref Unknown, ref Unknown, ref Unknown,
                        ref Unknown, ref Unknown, ref Unknown,
                        ref Unknown, ref Unknown, ref Unknown,
                        ref Unknown, ref Unknown, ref Unknown,
                        ref Unknown, ref Unknown);

            newApp.Documents.Close(Word.WdSaveOptions.wdDoNotSaveChanges);


        }

        class MainClass
        {
            static void Main(string[] args)
            {
                String tmpfile = GetTempFile(".htm");
                System.Console.WriteLine("file: " + args[0]);
                ConvertDocToHtml(args[0], tmpfile);
                System.Console.WriteLine("converted: " + tmpfile);

                HtmlDocument doc = new HtmlDocument();
                doc.Load(tmpfile);
                System.Console.WriteLine("converted: " + tmpfile);
                var h1Elements = doc.DocumentNode.Descendants("h1").Select(nd => nd.InnerText);
                string h1Text = string.Join(" ", h1Elements);
                System.Console.WriteLine("op: " + h1Text);
                
            


            //XmlDocument htmlDocument = new XmlDocument();
            //htmlDocument.Load(@tmpfile);

                //System.IO.StreamReader file = new System.IO.StreamReader(tmpfile);
                //String filestring = "";
                //String line;
                //while ((line = file.ReadLine()) != null)
                //{
                //    filestring = filestring + System.Environment.NewLine;
                //}
                //file.Close();
                //htmlDocument.LoadXml(filestring);
                //Console.WriteLine(htmlDocument.InnerXml);

                // Select the body tag
               // String bodyNode = htmlDocument.GetElementsByTagName("body").Item(0);

                Console.WriteLine("Press ESC to stop");
                do
                {
                    while (!Console.KeyAvailable)
                    {                       // Do something
                    }
                } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
            }
        }
    }
}
