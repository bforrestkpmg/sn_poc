using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;
using System.Xml;
using System.Web;
using System.Xml.Linq;
using System.Xml.Schema;
using GetDataConvertAndExtract;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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

        //public Boolean ConvertDocToXMLTree(object Sourcepath, ref List<OpenXmlElement> param_listOfDocElements)
        //{
        //    List<OpenXmlElement> listOfDocElements = new List<OpenXmlElement>();
        //    try
        //    {


        //        StringBuilder result = new StringBuilder();
        //        WordprocessingDocument wordProcessingDoc = WordprocessingDocument.Open(Sourcepath.ToString(), true);
        //        IEnumerable<Paragraph> paragraphElement = wordProcessingDoc.MainDocumentPart.Document.Descendants<Paragraph>();
        //        foreach (OpenXmlElement section in wordProcessingDoc.MainDocumentPart.Document.Body.Elements<OpenXmlElement>())
        //        {
        //            op_text("in: " + section.GetType().ToString());
        //            if ((section.GetType().Name == "Paragraph") || (section.GetType().Name == "Table"))
        //            {
        //                listOfDocElements.Add(section);
        //                op_text("section: " + section.InnerText.ToString());
        //            }
        //        } // foreach
        //    }
        //    catch (Exception ex)
        //    {
        //        op_text("Error to XML(" + ex.ToString() + ") converting input file: " + Sourcepath);
        //        return (false);
        //    }
        //    param_listOfDocElements = listOfDocElements;
        //    return (true);
        //}

        //public Boolean ConvertDocToXML(object Sourcepath, object TargetPath)
        //{
        //    try
        //    {
        //        StringBuilder result = new StringBuilder();
        //        WordprocessingDocument wordProcessingDoc = WordprocessingDocument.Open(Sourcepath.ToString(), true);
        //        IEnumerable<Paragraph> paragraphElement = wordProcessingDoc.MainDocumentPart.Document.Descendants<Paragraph>();
        //        op_text("here");
        //        foreach (OpenXmlElement section in wordProcessingDoc.MainDocumentPart.Document.Body.Elements<OpenXmlElement>())
        //        {

        //            if (section.GetType().Name == "Paragraph")
        //            {
        //                Paragraph par = (Paragraph)section;
        //                op_text("para: " + par.InnerText.ToString());
        //            }
        //        } // foreach
        //    }
        //    catch (Exception ex)
        //    {
        //        op_text("Error to XML(" + ex.ToString() + ") converting input file: " + Sourcepath + ", to: " + TargetPath);
        //        return (false);
        //    }
        //    return (true);
        //}


        //public Boolean ConvertDocToHtml(object Sourcepath, object TargetPath)
        //{
        //    try
        //    {
        //        Word._Application newApp = new Word.Application();
        //        Word.Documents d = newApp.Documents;
        //        object Unknown = Type.Missing;
        //        Word.Document od = d.Open(ref Sourcepath, ref Unknown,
        //                                 ref Unknown, ref Unknown, ref Unknown,
        //                                 ref Unknown, ref Unknown, ref Unknown,
        //                                 ref Unknown, ref Unknown, ref Unknown,
        //                                 ref Unknown, ref Unknown, ref Unknown, ref Unknown);
        //        object format = Word.WdSaveFormat.wdFormatFilteredHTML;

        //        newApp.ActiveDocument.SaveAs(ref TargetPath, ref format,
        //                    ref Unknown, ref Unknown, ref Unknown,
        //                    ref Unknown, ref Unknown, ref Unknown,
        //                    ref Unknown, ref Unknown, ref Unknown,
        //                    ref Unknown, ref Unknown, ref Unknown,
        //                    ref Unknown, ref Unknown);

        //        newApp.Documents.Close(Word.WdSaveOptions.wdDoNotSaveChanges);
        //    }
        //    catch
        //    {
        //        return (false);
        //    }
        //    return (true);
        //}


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
                return (false);
            }
            return (true);
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
            op_text("fn: " + fn);
            using (WordprocessingDocument wordprocessingDocument = WordprocessingDocument.Open(fn, false))
            {
                Boolean inTable = false;
                Boolean First = true;
                Boolean inCorrectHeader = false;
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
                    try
                    {
                        inHeaderText = textNode.ParentNode.ParentNode.ChildNodes[0].ChildNodes[0].Attributes["w:val"].Value;
                    }
                    catch (Exception ex)
                    {
                        inHeaderText = "";
                    }
                    if (inHeaderText.StartsWith("Heading"))
                    {
                        if (textNode.InnerText == HeadingText)
                        {
                            inCorrectHeader = true;
                        }
                        else {
                            inCorrectHeader = false;
                        }
                    }
                    if (inCorrectHeader)
                    {
                        switch (textNode.Name)
                        {
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

        //public void OldExtractSection(String filename, ref String return_buffer, String headingtext)
        //{
        //    HtmlDocument doc = new HtmlDocument();
        //    doc.Load(filename, Encoding.GetEncoding("iso-8859-1"));
        //    Boolean inCorrectHeading = false;
        //    Boolean inTable = false;
        //    return_buffer = "";
        //    String extractText = "";
        //    String line_to_use = "";
        //    HtmlNode[] nodearray;
        //    var findclasses = doc.DocumentNode.SelectNodes("//*");
        //    var outputclasses = new HtmlNodeCollection(null);
        //    int counter = 0;
        //    String newstr;
        //    String bufferchar = "";
        //    String beforechar = "";
        //    String afterchar = "";
        //    Boolean First = true;
        //    String compare_str = "";
        //    foreach (HtmlNode node in findclasses)
        //    {
        //        counter++;
        //        newstr = "";
        //        line_to_use = "";
        //        afterchar = "";
        //        beforechar = "";
        //        // relies on the fact that word docs are flat within body & div
        //        if ((node.ParentNode.Name != "body") && (node.ParentNode.Name != "div") && (node.ParentNode.Name != "table") && (node.ParentNode.Name != "tr")) { continue; }
        //        //System.Console.WriteLine("Looking at node: " + node.Name + ", parent: " + node.ParentNode.Name);
        //        if (node.Name.StartsWith("h"))
        //        {
        //            compare_str = node.InnerText.Replace("\r", "").Replace("\n", " ");
        //            if (compare_str.Contains(headingtext))
        //            {
        //                // System.Console.WriteLine("in right heading: " + compare_str);
        //                inCorrectHeading = true;
        //                continue;
        //            }
        //            else {
        //                inCorrectHeading = false;
        //                continue;
        //            }
        //        }
        //        if (inCorrectHeading)
        //        {
        //            outputclasses.Add(node);
        //            switch (node.Name)
        //            {
        //                case "p":
        //                    {
        //                        inTable = false;
        //                        beforechar = Environment.NewLine;
        //                        afterchar = Environment.NewLine;
        //                        line_to_use = node.InnerText;
        //                        break;
        //                    }

        //                case "tr":
        //                    {
        //                        inTable = true;
        //                        afterchar = Environment.NewLine;
        //                        First = true;
        //                        break;
        //                    }
        //                case "td":
        //                    {
        //                        inTable = true;
        //                        if (First)
        //                        {
        //                            beforechar = Environment.NewLine + "\"";
        //                            First = false;
        //                        }
        //                        else { beforechar = "\""; }
        //                        afterchar = "\"\t";
        //                        line_to_use = node.InnerText;
        //                        break;
        //                    }
        //            } //switch
        //              //  System.Console.WriteLine("Processing node: " + node.Name + ", text: " + node.InnerText);
        //            newstr = Regex.Replace(line_to_use, "&nbsp;", " ");
        //            newstr = Regex.Replace(newstr, "\r", "");
        //            newstr = Regex.Replace(newstr, "\n", "");
        //            newstr = Regex.Replace(newstr, "[^\u0000-\u007F]", "");
        //            newstr = newstr.Trim();
        //            if (newstr != "")
        //            {
        //                // System.Console.WriteLine("Adding line: " + "beforechar: _" + beforechar + "_, newstr: _" + newstr + "_, afterchar: _" + afterchar + "_");
        //                return_buffer = return_buffer + beforechar + newstr + afterchar;
        //            }
        //        }
        //    } //foreach

        //} // ExtractSection
    }
}


namespace Final_Get_Data_Console
{
    class Program
    {
        public static String empty_str = "";
        public static String filename_to_use = "";
        public static String section_heading = "";

        public static String GenerateXMLOutput(ref String paragraph, String err)
        {
            XElement e;
            if (CheckValidString(err))
            {
                e = new XElement("error", err);
            }
            else {
                e = new XElement("paragraph", paragraph);
            }
            XDocument doc = new XDocument(new XElement("response", e));
            return doc.ToString();
        } // GenerateXMLOutput

        public static void op_text(String s)
        {
            System.Diagnostics.Debug.WriteLine(s);
            Console.WriteLine(s);
        }

        public static Boolean parse_args(String[] args)
        {
            if (args.Length < 2)
            {
                System.Console.WriteLine("Usage:: <url/filename> <section header> [-test]");
                Environment.Exit(0);
            }
            // have we specified test mode we genereate specific output based on file name
            if (args.Length >= 3)
            {
                if ((!String.IsNullOrEmpty(args[2])) && (args[2].Equals("-test")))
                {
                    String op = "";
                    op = output_test_responses_based_on_file_name(args[0], args[1]);
                    op_text(op);
                    Environment.Exit(0);
                }
            }
            Program.filename_to_use = args[0];
            Program.section_heading = args[1];
            // #TODO unit tests for parse_args and return false if problems
            return (true);
        }

        // approach
        // check args (url/file and heading name)
        // check to see its a file to download (http) or a localfile
        // if downlaod , download it
        // check extension of downloaded file
        // if already html we can parse that
        // if not we have to convert to html
        // ...
        static void Main(String[] args)
        {
            int exit_code = 1; //Error
            String actual_file_to_open = "";
            String extractedText = "";
            String optext_string = "";
            if (parse_args(args))
            {
                op_text("Using File: " + filename_to_use);
                if (filename_to_use.StartsWith("http"))
                {
                    actual_file_to_open = DownloadFile(args[1]);
                    op_text("Downloaded: " + actual_file_to_open);
                }
                else {
                    actual_file_to_open = filename_to_use;
                    // assume its a local file
                    op_text("Using localfile: " + actual_file_to_open);
                }
                var cgd = new ConvertGetData();
                extractedText =  cgd.ReadSectionHeadingTextFromDocx(actual_file_to_open, section_heading);
                op_text("extracted: " + extractedText);
                optext_string = GenerateXMLOutput(ref extractedText, "");
                exit_code = 0;
            }
            else {
                optext_string = GenerateXMLOutput(ref empty_str, "Problem parsing arguments");
            }
            Console.WriteLine(optext_string);
            Environment.Exit(exit_code);
        }//END   Main


        public static Boolean CheckValidString(String s)
        {
            return (!((s == null) || s.Equals("")));
        }


        // TEST CODE
        // simulates the outputs we need to generate from  the overall get_data process 
        // allows for SN testing
        // e.g. get_data "testsingleassetid1" "Bill of Materials" will return <response><paragraph>1.0   WS-C4999-E  Cat4500 </paragraph></response>"
        // e.g. get_data "errorheadingnowsowassets" "Quote" will return <response><error>Assets not found in Quote</error></response>"
        public static String output_test_responses_based_on_file_name(String filename, String heading)
        {
            String s = null;
            String e = null;
            if (filename.Equals("testsingleassetid1"))
            {
                if (heading.Equals("Bill of Materials"))
                {
                    s = "1.0   WS-C4999-E  Cat4500 ";
                } // if heading BOM
                else if (heading.Equals("Quote"))
                {
                    s = "Firewall-Infrastructure-New-Complex (WS-C4999-E)  2   $     1,837.33 $   3,674.66 $  176,383.68\n Firewall-Support line 1\t2\t$     324.44    $   648.88  $  31,146.24\nFirewall-Support xline2\t2\t$     324.44  $   648.88  $  31,146.24\nFirewall-Infrastructure-New-Complex (ASA6666)\t2\t$    - $   -    $  -\n Firewall-Infrastructure-New-Complex (WS-c4999-F)\t2  $     1,837.33 $   3,674.66 $  176,383.68\n Firewall-Support YYYY\t2\t$     324.44  $   648.88  $  31,146.24\n Firewall-Infrastructure-New-Blah (Cat4506)   $    470.97 $   1,883.88    $  90,426.24\n";
                } // if heading Quote
            } // if filename = testsingleassetid1
            else if (filename.Equals("testsingleassetid2"))
            {
                if (heading.Equals("Bill of Materials"))
                {
                    // example with 2 asset ids
                    s = "1.0   WS-C4999-E  Cat4500 E-Series 6-Slot Chassis fan no ps   2   $7,461.75 \n1.1 WS-X4748-RJ45-E Catalyst 4500 E-Series 48-Port10/100/1000 Non-Bl;ocking 2   $10,449.44\n2.0 WS-c4999-F  Cat4500 E-Series 6-Slot Chassis fan no ps   2   $7,461.75\n2.1  WS-X4748-RJ45-E Cataqlyst 4500 E-Series 48-Port 10/100/1000 Non-Blcoking    2   $7,461.75";
                } // if heading BOM
                else if (heading.Equals("Quote"))
                {
                    s = "Pricing Table Notes: \n \n 1.    A Contract Variation will be executed between the parties to add the incremental charge to the existing ‘TWS Service 3 – Data Centre to Data Centre’ Resource Unit\n 2. This solution will be delivered under the T&Cs of the existing DNV agreement between Qantas and Telstra. A contract variation will be required to add some new Resource Units (price points), otherwise the service model will be as per the existing agreement\n \n 22\n \n Once Off Charges\n \n \n Consultancy Services ~ GST Excl   Units   Unit Price  Extended Price\n \n \n Proramme Support / Imlementation Da - 8 hours\n 5    1135.68 $5,678.40\n \n Ongoing Resource Unit Charges\n Additional Resource Units -GST Excl  Quanity RU Price(per month) Unit Extended Price Total Contract Value\n Firewall-Infrastructure-New-Complex (WS-C4999-E) 2   $     1,837.33 $   3,674.66 $  176,383.68\n Firewall-Support line 1\t2\t$     324.44    $   648.88  $  31,146.24\nFirewall-Support xline2\t2\t$     324.44  $   648.88  $  31,146.24\nFirewall-Infrastructure-New-Complex (ASA6666)\t2\t$    - $   -    $  -\n Firewall-Infrastructure-New-Complex (WS-c4999-F)\t2  $     1,837.33 $   3,674.66 $  176,383.68\n Firewall-Support YYYY\t2\t$     324.44  $   648.88  $  31,146.24\n Firewall-Infrastructure-New-Blah (Cat4506)   $    470.97 $   1,883.88    $  90,426.24\n";
                } // if heading Quote
            } // if filename = testsingleassetid1
            else if (filename.Equals("errorheadingnotfound"))
            {
                if (heading.Equals("Bill of Materials"))
                {
                    e = "Cannot find Heading Bill of Materials";
                } // if heading BOM
                else if (heading.Equals("Quote"))
                {
                    e = "Cannot find Heading Quote";
                } // if heading Quote
            } // if filename = testsingleassetid1

            else if (filename.Equals("ErrorBOMHasAssetButNotInQuote"))
            {
                if (heading.Equals("Bill of Materials"))
                {
                    s = "1.0   WS-C4999-E  Cat4500 ";
                } // if heading BOM
                else if (heading.Equals("Quote"))
                {
                    s = "Firewall-Infrastructure-New-Complex (WS-NOASSET-E)  2   $     1,837.33 $   3,674.66 $  176,383.68\n Firewall-Support line 1\t2\t$     324.44    $   648.88  $  31,146.24\nFirewall-Support xline2\t2\t$     324.44  $   648.88  $  31,146.24\nFirewall-Infrastructure-New-Complex (NOASSET2)\t2\t$    - $   -    $  -\n Firewall-Infrastructure-New-Complex (NOASSET3)\t2  $     1,837.33 $   3,674.66 $  176,383.68\n Firewall-Support YYYY\t2\t$     324.44  $   648.88  $  31,146.24\n Firewall-Infrastructure-New-Blah (Cat4506)   $    470.97 $   1,883.88    $  90,426.24\n";
                } // if heading Quote
            } // if filename = testsingleassetid1

            else if (filename.Equals("ErrorBOMNoAssets"))
            {
                if (heading.Equals("Bill of Materials"))
                {
                    s = "Blah blah blah";
                }
                else if (heading.Equals("Quote"))
                {
                    e = "hello there we have no\nDescription hello (A-99999)    2   $100.00\nComponent Line 1   1   $10.00\nbut asset info in here that matches\nabove";
                } // if heading Quote
            } // if filename = testsingleassetid1
            String r;
            r = GenerateXMLOutput(ref s, e);
            return r;

        }  // output_test_responses_based_on_file_name


        public static void ExitError(String err)
        {
            String e = GenerateXMLOutput(ref empty_str, err);
            System.Console.WriteLine(e);
            Environment.Exit(1);
        }

        // TODO error check  the creation
        // returns unique/temp filename with extension provided
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
        static void client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            op_text("Finished Downloads");
        }
        private static void DoDownloadFile(string webUrl, ICredentials credentials, string fileRelativeUrl, string localfile)
        {
            var client = new WebClient();

            client.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(client_DownloadFileCompleted);
            using (client)
            {
                client.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
                client.Headers.Add("User-Agent: Other");
                client.Credentials = credentials;
                String s = webUrl + '/' + fileRelativeUrl;
                try
                {
                    Uri ur = new Uri(webUrl + '/' + fileRelativeUrl);
                    client.DownloadFileAsync(ur, localfile);
                }
                catch (WebException wex)
                {
                    if (((HttpWebResponse)wex.Response).StatusCode == HttpStatusCode.NotFound)
                    {
                        ExitError("404 trying to get file: " + s);
                    }
                }
            }

        }

        // TODO check this works with username password for http authentication
        public static string DownloadFile(String urlfilename)
        {
            String tmp_file = "";
            // for our test server username and password are part of http login
            // TODO make these from environment variables
            const String username = "bforrest@kpmg.com.au";
            const String password = "mypassw0rd+";
          //  op_text("DownloadFile URI: " + urlfilename);
            System.Uri uri = new System.Uri(urlfilename);

            String just_file = uri.AbsolutePath;
            String URL = uri.AbsoluteUri;

            String extension = GetExt(urlfilename);
            tmp_file = GetTempFile(extension);
            var client = new WebClient();
            client.Credentials = new NetworkCredential(username, password);
            DoDownloadFile(URL, client.Credentials, just_file, tmp_file);

            return (tmp_file);
        }

    }//END      Program
}//END         Add_Function