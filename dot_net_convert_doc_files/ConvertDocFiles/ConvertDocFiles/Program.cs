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
                Console.BufferHeight = 999;
                String tmpfile = GetTempFile(".htm");
                System.Console.WriteLine("file: " + args[0]);
                ConvertDocToHtml(args[0], tmpfile);
                System.Console.WriteLine("converted: " + tmpfile);

                HtmlDocument doc = new HtmlDocument();
                doc.Load(tmpfile);
                System.Console.WriteLine("converted: " + tmpfile);

               
                Boolean inCorrectHeading = false;
                String headingtext = "Quote Costs";
                String extractText = "";
                String ActualHeadingText = "";
                var findclasses = doc.DocumentNode.SelectNodes("//body//*");
                 var outputclasses = new HtmlNodeCollection(null);
                int counter = 0;
                foreach (HtmlNode node in findclasses) {
                    counter++;
                    if (node.Name.StartsWith("h")) 
                    {
                        if (node.InnerText.Contains(headingtext))
                        {
                            //System.Console.WriteLine("in right heading: "+ node.InnerText);
                            inCorrectHeading = true;
                            continue;
                        }
                        else
                        {
                            //System.Console.WriteLine("in a different heading");
                            inCorrectHeading = false;
                            continue;
                        }
                        
                    }
                    if (inCorrectHeading)
                    {
                        outputclasses.Add(node);
                        System.Console.WriteLine(counter.ToString() + " : " + "adding node: " + node.InnerText.ToString());
                        extractText = extractText + node.InnerText.ToString() + Environment.NewLine;
                    }


                } //foreach

                //System.Console.WriteLine("output: " + extractText);
                // iterate over our new collection
                //foreach (HtmlNode node in findclasses)
                //{
                //}
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
