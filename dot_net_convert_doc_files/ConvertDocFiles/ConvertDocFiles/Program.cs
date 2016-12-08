using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;
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


        public static void WriteFile(String fn, String buffer)
        {
            string[] lines = buffer.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            using (StreamWriter outputFile = new StreamWriter( @fn))
            {
                foreach (string line in lines)
                    outputFile.WriteLine(line);
            }
        }
        class MainClass
        {
            static void Main(string[] args)
            {
                String filetouse = "";
                Console.BufferHeight = 999;
                Console.BufferWidth = 200;
                Console.Clear();

                System.Console.WriteLine("file: " + args[0]);
                String extension = GetExt(args[0]);
                System.Console.WriteLine("extension: " + extension);
                if (!GetExt(args[0]).StartsWith(".htm"))
                {
                    String tmpfile = GetTempFile(".htm");
                    ConvertDocToHtml(args[0], tmpfile);
                    System.Console.WriteLine("converted: " + tmpfile);
                    filetouse = tmpfile;
                }

                else { filetouse = args[0];  }

                System.Console.WriteLine("using file: " + filetouse);
                HtmlDocument doc = new HtmlDocument();
                doc.Load(filetouse, Encoding.GetEncoding("iso-8859-1"));
               
                Boolean inCorrectHeading = false;
                Boolean inTable = false;
                String headingtext = "Quote Costs";
                String extractText = "";
                String ActualHeadingText = "";
                String line_to_use="";
                HtmlNode[] nodearray;
                var findclasses = doc.DocumentNode.SelectNodes("//*");
                var outputclasses = new HtmlNodeCollection(null);
                int counter = 0;
                String newstr;
                String bufferchar = "";
                String beforechar = "";
                String afterchar = "";
                Boolean First = true;

                foreach (HtmlNode node in findclasses) {
                    counter++;
                    newstr = "";
                    line_to_use = "";
                    afterchar = "";
                    beforechar = "";
                    // relies on the fact that word docs are flat within body & div
                    //System.Console.WriteLine("Looking at node: " + node.Name + ", text: " + node.InnerText);

                    if ((node.ParentNode.Name != "body") && (node.ParentNode.Name != "div")  && (node.ParentNode.Name != "table") && (node.ParentNode.Name != "tr")) { continue; }
                    System.Console.WriteLine("Looking at node: " + node.Name + ", parent: " + node.ParentNode.Name);
                    if (node.Name.StartsWith("h")) 
                    {
                       // System.Console.WriteLine("checking header");
                        if (node.InnerText.Contains(headingtext))  {
                            //System.Console.WriteLine("in right heading: "+ node.InnerText);
                            inCorrectHeading = true;
                            continue;
                        }
                        else { 
                           // System.Console.WriteLine("in a different heading");
                            inCorrectHeading = false;
                            continue;
                        }   
                    }
                    if (inCorrectHeading)
                    {
                        outputclasses.Add(node);

                        switch (node.Name)
                        {

                            case "p":
                                {
                                    inTable = false;
                                    System.Console.WriteLine("In para");
                                    beforechar = Environment.NewLine;
                                    afterchar = Environment.NewLine;
                                    line_to_use = node.InnerText;
                                    break;
                                }
                            case "table":
                                {
                                    System.Console.WriteLine("table");
                                    afterchar = Environment.NewLine;
                                    //bufferchar = "_n_";
                                    inTable = true;
                                    break;
                                }
                            case "tr":
                                {
                                    System.Console.WriteLine("row");
                                    beforechar = Environment.NewLine;
                                    afterchar = Environment.NewLine;
                                    First = true;
                                    //bufferchar = "_n_";
                                    // line_to_use = node.InnerText;
                                    break;
                                }
                            case "td":
                                {
                                    
                                    if (First)
                                    {
                                        beforechar = Environment.NewLine + "\"";
                                        First = false;
                                    }
                                    else
                                    {
                                      
                                        beforechar = "\"";
                                    }
                                    System.Console.WriteLine("cell");

                                    // afterchar = "\",";
                                    afterchar = "\"   ";

                                    line_to_use = node.InnerText;
                                    //bufferchar = "_t_";
                                    break;
                                }
                        } //switch
                        newstr = Regex.Replace(line_to_use, "&nbsp;", " ");
                        newstr = Regex.Replace(newstr, "[^\u0000-\u007F]", "");
                        System.Console.WriteLine("adding newstr: " + newstr);
                        if (newstr != "")
                        {
                            extractText = extractText + beforechar + newstr + afterchar;
                        }
                    }

                    


                } //foreach

                //System.Console.WriteLine("output: " + extractText);
                System.Console.WriteLine("finish");

                WriteFile(filetouse + ".op.csv", extractText);
                //// iterate over our new collection
                ////foreach (HtmlNode node in findclasses)
                ////{
                ////}
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
