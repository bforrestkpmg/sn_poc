using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;
using Word = Microsoft.Office.Interop.Word;
using HtmlAgilityPack;
using System.Threading.Tasks;

namespace GetDataConvertAndExtract
{
    class ConvertGetData
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

        public void ConvertDocToHtml(object Sourcepath, object TargetPath)
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


        public void WriteFile(String fn, String buffer)
        {
            string[] lines = buffer.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            using (StreamWriter outputFile = new StreamWriter(@fn))
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
                if ((node.ParentNode.Name != "body") && (node.ParentNode.Name != "div") && (node.ParentNode.Name != "table") && (node.ParentNode.Name != "tr")) { continue; }
                //System.Console.WriteLine("Looking at node: " + node.Name + ", parent: " + node.ParentNode.Name);
                if (node.Name.StartsWith("h"))
                {
                    compare_str = node.InnerText.Replace("\r", "").Replace("\n", " ");
                    if (compare_str.Contains(headingtext))
                    {
                        System.Console.WriteLine("in right heading: " + compare_str);
                        inCorrectHeading = true;
                        continue;
                    }
                    else {
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
                                afterchar = Environment.NewLine;
                                First = true;
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
                                else { beforechar = "\""; }
                                afterchar = "\"\t";
                                line_to_use = node.InnerText;
                                break;
                            }
                    } //switch
                      //  System.Console.WriteLine("Processing node: " + node.Name + ", text: " + node.InnerText);
                    newstr = Regex.Replace(line_to_use, "&nbsp;", " ");
                    newstr = Regex.Replace(newstr, "\r", "");
                    newstr = Regex.Replace(newstr, "\n", "");
                    newstr = Regex.Replace(newstr, "[^\u0000-\u007F]", "");
                    newstr = newstr.Trim();
                    if (newstr != "")
                    {
                        // System.Console.WriteLine("Adding line: " + "beforechar: _" + beforechar + "_, newstr: _" + newstr + "_, afterchar: _" + afterchar + "_");
                        return_buffer = return_buffer + beforechar + newstr + afterchar;
                    }
                }
            } //foreach

        } // ExtractSection
    }
}
