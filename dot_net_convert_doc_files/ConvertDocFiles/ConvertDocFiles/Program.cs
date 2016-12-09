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

        public static void ExtractSection(String filename, ref String return_buffer, String headingtext)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.Load(filename, Encoding.GetEncoding("iso-8859-1"));
            Boolean inCorrectHeading = false;
            Boolean inTable = false;
            return_buffer = "";
            String extractText = "";
            String line_to_use = "";
            HtmlNode[] nodearray;
            var findclasses = doc.DocumentNode.SelectNodes("//*");
            var outputclasses = new HtmlNodeCollection(null);
            int counter = 0;
            String newstr;
            String bufferchar = "";
            String beforechar = "";
            String afterchar = "";
            Boolean First = true;
            String compare_str = "";
            foreach (HtmlNode node in findclasses)
            {
                counter++;
                newstr = "";
                line_to_use = "";
                afterchar = "";
                beforechar = "";
                // relies on the fact that word docs are flat within body & div
                //System.Console.WriteLine("Looking at node: " + node.Name + ", text: " + node.InnerText);

                if ((node.ParentNode.Name != "body") && (node.ParentNode.Name != "div") && (node.ParentNode.Name != "table") && (node.ParentNode.Name != "tr")) { continue; }
                //System.Console.WriteLine("Looking at node: " + node.Name + ", parent: " + node.ParentNode.Name);
                if (node.Name.StartsWith("h"))
                {
                    compare_str = node.InnerText.Replace("\r", "").Replace("\n", " ");
                    //System.Console.WriteLine("checking header, text: " + compare_str);
                    if (compare_str.Contains(headingtext))
                    {
                        System.Console.WriteLine("in right heading: "+ compare_str);
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
                                beforechar = Environment.NewLine;
                                afterchar = Environment.NewLine;
                                line_to_use = node.InnerText;
                                break;
                            }

                        case "tr":
                            {
                                inTable = true;
                                //System.Console.WriteLine("row");
                                afterchar = Environment.NewLine;
                                First = true;
                                //bufferchar = "_n_";
                                // line_to_use = node.InnerText;
                                break;
                            }
                        case "td":
                            {
                                inTable = true;
                                if (First)
                                {
                                    beforechar = Environment.NewLine + "\"";
                                    First = false;
                                }
                                else
                                {
                                    beforechar = "\"";
                                }
                                // System.Console.WriteLine("cell");

                                // afterchar = "\",";
                                afterchar = "\"\t";

                                line_to_use = node.InnerText;
                                //bufferchar = "_t_";
                                break;
                            }
                    } //switch
                      //  System.Console.WriteLine("Processing node: " + node.Name + ", text: " + node.InnerText);
                    newstr = Regex.Replace(line_to_use, "&nbsp;", " ");
                    newstr = Regex.Replace(newstr, "\r", "");
                    newstr = Regex.Replace(newstr, "\n", "");
                    newstr = Regex.Replace(newstr, "[^\u0000-\u007F]", "");
                    newstr = newstr.Trim();
                    // System.Console.WriteLine("adding newstr: " + newstr);
                    if (newstr != "")
                    {
                        // System.Console.WriteLine("Adding line: " + "beforechar: _" + beforechar + "_, newstr: _" + newstr + "_, afterchar: _" + afterchar + "_");
                        return_buffer = return_buffer + beforechar + newstr + afterchar;
                    }
                }




            } //foreach


        } // ExtractSection
        class MainClass
        {
            static void Main(string[] args)
            {
                //String OverrideFile = @"S:\test\fixtures\files\QuoteAsTable.doc";
                String OverrideFile = @"S:\test\fixtures\files\TestDoc_DocWithHeading_andHeadingNumber_IncludesTable_lesscontent.htm";
                //String headingtext = "Quote Costs";
                String headingtext = "Heading 1";
                String filename_to_use = "";
                String filetouse = "";
                String extractText = "";
                Console.BufferHeight = 999;
                Console.BufferWidth = 200;
                Console.Clear();
                String processing_file = "";

                if (OverrideFile != "") { filename_to_use = OverrideFile; }
                else { filename_to_use = args[0]; }

                System.Console.WriteLine("file: " + filename_to_use);
                String extension = GetExt(filename_to_use);
                System.Console.WriteLine("extension: " + extension);
                if (!extension.StartsWith(".htm"))
                {
                    String tmpfile = GetTempFile(".htm");
                    ConvertDocToHtml(filename_to_use, tmpfile);
                    System.Console.WriteLine("converted: " + tmpfile);
                    processing_file = tmpfile;
                    System.Console.WriteLine("using file: " + processing_file);
                }
                else processing_file = filename_to_use;
                System.Console.WriteLine("using file: " + processing_file);
                ExtractSection(processing_file, ref extractText, headingtext);
                System.Console.WriteLine("output: " + extractText);
                System.Console.WriteLine("finish");

                WriteFile(filename_to_use + ".op.csv", extractText);
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
