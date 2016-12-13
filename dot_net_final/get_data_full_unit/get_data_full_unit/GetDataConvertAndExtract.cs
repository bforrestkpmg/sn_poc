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
using System.IO;
using System.IO.Packaging;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;

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
        public static void op_text(String s)
        {
            System.Diagnostics.Debug.WriteLine(s);
            Console.WriteLine(s);
        }

        public Boolean ConvertDocToXMLTree(object Sourcepath, ref List<OpenXmlElement> param_listOfDocElements)
        {
            List<OpenXmlElement> listOfDocElements = new List<OpenXmlElement>();
            try
            {


                StringBuilder result = new StringBuilder();
                WordprocessingDocument wordProcessingDoc = WordprocessingDocument.Open(Sourcepath.ToString(), true);
                IEnumerable<Paragraph> paragraphElement = wordProcessingDoc.MainDocumentPart.Document.Descendants<Paragraph>();
                foreach (OpenXmlElement section in wordProcessingDoc.MainDocumentPart.Document.Body.Elements<OpenXmlElement>())
                {
                    op_text("in: " + section.GetType().ToString());
                    if ((section.GetType().Name == "Paragraph") || (section.GetType().Name == "Table"))
                    {
                        listOfDocElements.Add(section);
                        op_text("section: " + section.InnerText.ToString());
                    }
                } // foreach
            }
            catch (Exception ex)
            {
                op_text("Error to XML(" + ex.ToString() + ") converting input file: " + Sourcepath);
                return (false);
            }
            param_listOfDocElements = listOfDocElements;
            return (true);
        }

        public Boolean ConvertDocToXML(object Sourcepath, object TargetPath)
        {
            try
            {
                StringBuilder result = new StringBuilder();
                WordprocessingDocument wordProcessingDoc = WordprocessingDocument.Open(Sourcepath.ToString(), true);
                IEnumerable<Paragraph> paragraphElement = wordProcessingDoc.MainDocumentPart.Document.Descendants<Paragraph>();
                op_text("here");
                foreach (OpenXmlElement section in wordProcessingDoc.MainDocumentPart.Document.Body.Elements<OpenXmlElement>())
                {

                    if (section.GetType().Name == "Paragraph")
                    {
                        Paragraph par = (Paragraph)section;
                        op_text("para: " + par.InnerText.ToString());
                    }
                    } // foreach
            }
            catch (Exception ex)
            {
                op_text("Error to XML(" + ex.ToString() + ") converting input file: " + Sourcepath + ", to: " + TargetPath);
                return (false);
            }
            return (true);
        }


        public Boolean ConvertDocToHtml(object Sourcepath, object TargetPath)
        {
            try
            {
                Word._Application newApp = new Word.Application();
                Word.Documents d = newApp.Documents;
                object Unknown = Type.Missing;
                Word.Document od = d.Open(ref Sourcepath, ref Unknown,
                                         ref Unknown, ref Unknown, ref Unknown,
                                         ref Unknown, ref Unknown, ref Unknown,
                                         ref Unknown, ref Unknown, ref Unknown,
                                         ref Unknown, ref Unknown, ref Unknown, ref Unknown);
                object format = Word.WdSaveFormat.wdFormatFilteredHTML;

                newApp.ActiveDocument.SaveAs(ref TargetPath, ref format,
                            ref Unknown, ref Unknown, ref Unknown,
                            ref Unknown, ref Unknown, ref Unknown,
                            ref Unknown, ref Unknown, ref Unknown,
                            ref Unknown, ref Unknown, ref Unknown,
                            ref Unknown, ref Unknown);

                newApp.Documents.Close(Word.WdSaveOptions.wdDoNotSaveChanges);
            }
            catch
            {
                return(false);
            }
            return(true);
        }


        public Boolean WriteFile(String fn, String buffer)
        {
            string[] lines = buffer.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            try
            {

            using (StreamWriter outputFile = new StreamWriter(@fn))
            {
                foreach (string line in lines)
                    outputFile.WriteLine(line);
            }

            }
            catch
            {
                return(false);
            }
            return(true);
        }

        public String ReadAllTextFromDocx(String fn)
        {
            StringBuilder stringBuilder;
            using (WordprocessingDocument wordprocessingDocument = WordprocessingDocument.Open(fn, false))
            {
                Boolean inTable = false;
                Boolean First = true;
                String afterchar = "";
                String beforechar = "";
                Boolean inSection = false;
                NameTable nameTable = new NameTable();
                XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(nameTable);
                xmlNamespaceManager.AddNamespace("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");

                string wordprocessingDocumentText;
                using (StreamReader streamReader = new StreamReader(wordprocessingDocument.MainDocumentPart.GetStream()))
                {
                    wordprocessingDocumentText = streamReader.ReadToEnd();
                }

                stringBuilder = new StringBuilder(wordprocessingDocumentText.Length);

                XmlDocument xmlDocument = new XmlDocument(nameTable);
                xmlDocument.LoadXml(wordprocessingDocumentText);

                XmlNodeList allNodes = xmlDocument.SelectNodes(".//w:tbl | .//w:p  |  .//w:tr | .//w:r | .//w:t | .//w:pStyle", xmlNamespaceManager);
                foreach (XmlNode textNode in allNodes)
                {


                    //op_text("textnode: " + textNode.Name +  ", content: " + textNode.InnerText );
                    switch (textNode.Name)
                    {
                        case "w:pStyle":
                            {
                                //if (textNode.Value.ToString() == "Heading1")
                                //{
                                //    op_text("In Heading1");
                                //    inTable = false;
                                //}
                                // we have a style feature
                                break;
                            }
                        case "w:tr":  // Row
                            inTable = true;
                            stringBuilder.Append(Environment.NewLine);
                            break;
                        case "w:t": // Text
                            switch (textNode.ParentNode.ParentNode.ParentNode.Name)
                            {
                                case "w:tc": // in cell or header
                                    stringBuilder.Append("\t");
                                    stringBuilder.Append(textNode.InnerText);
                                    break;
                                case "w:body": // in normal paragaph
                                    stringBuilder.Append(Environment.NewLine);
                                    stringBuilder.Append(textNode.InnerText);
                                    stringBuilder.Append(Environment.NewLine);
                                    break;
                            }
                            break;
                    } // switch
                } // foreach
            }

            return stringBuilder.ToString();
        }

        public String ReadSectionHeadingTextFromDocx(String fn, String HeadingText)
        {
            StringBuilder stringBuilder;
            using (WordprocessingDocument wordprocessingDocument = WordprocessingDocument.Open(fn, false))
            {
                Boolean inTable = false;
                Boolean First = true;
                Boolean inCorrectHeader  = false;
                String afterchar = "";
                String beforechar = "";
                NameTable nameTable = new NameTable();
                XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(nameTable);
                xmlNamespaceManager.AddNamespace("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");

                string wordprocessingDocumentText;
                using (StreamReader streamReader = new StreamReader(wordprocessingDocument.MainDocumentPart.GetStream()))
                {
                    wordprocessingDocumentText = streamReader.ReadToEnd();
                }

                stringBuilder = new StringBuilder(wordprocessingDocumentText.Length);

                XmlDocument xmlDocument = new XmlDocument(nameTable);
                xmlDocument.LoadXml(wordprocessingDocumentText);
                string inHeaderText = "";
                XmlNodeList allNodes = xmlDocument.SelectNodes(".//w:tbl | .//w:p  |  .//w:tr | .//w:r | .//w:t | .//w:pStyle", xmlNamespaceManager);
                foreach (XmlNode textNode in allNodes)
                {
                inHeaderText = "";
                    //op_text("textnode: " + textNode.Name +  ", content: " + textNode.InnerText );Value

                    //  < w:p w14:paraId = "7D4723BA" w14: textId = "77777777" w: rsidR = "001116D0" w: rsidRDefault = "00C84334" w: rsidP = "004C6690" >
                    //< w:pPr >
                    //    < w:pStyle w:val = "Heading1" />
                    //  </ w:pPr >
                    //   < w:r >
                    //       < w:t > Heading 1 </ w:t >
                    try {
                        inHeaderText = textNode.ParentNode.ParentNode.ChildNodes[0].ChildNodes[0].Attributes["w:val"].Value;
                    }
                    catch (Exception ex)
                    {
                        inHeaderText = "";
                    }
                    if (inHeaderText.StartsWith("Heading")) {
                        if (textNode.InnerText == HeadingText) {
                            inCorrectHeader = true;
                        }
                        else {
                            inCorrectHeader = false;
                        }
                    }
                        if (inCorrectHeader) {
                            switch (textNode.Name) {
                                case "w:tr":  // Row
                                    inTable = true;
                                    stringBuilder.Append(Environment.NewLine);
                                    break;
                                case "w:t": // Text
                                    switch (textNode.ParentNode.ParentNode.ParentNode.Name)
                                    {
                                        case "w:tc": // in cell or header
                                            stringBuilder.Append("\t");
                                            stringBuilder.Append(textNode.InnerText);
                                            break;
                                        case "w:body": // in normal paragaph
                                            stringBuilder.Append(Environment.NewLine);
                                            stringBuilder.Append(textNode.InnerText);
                                            stringBuilder.Append(Environment.NewLine);
                                            break;
                                    }
                                    break;
                            } // switch
                        }
                    } // foreach
            }

            return stringBuilder.ToString();
        }

        public void ExtractSection(String filename, ref String return_buffer, String headingtext)
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
                       // System.Console.WriteLine("in right heading: " + compare_str);
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
