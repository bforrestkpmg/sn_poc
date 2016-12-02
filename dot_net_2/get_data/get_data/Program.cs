﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace Add_Function
{
    class Program
    {


        // approach
        // check args (url/file and heading name)
        // create temporary file name
        // download file
        // open file and search for heading
        static void Main(String[] args)
        {
            parse_args(args);

            string downloaded_file = DownloadFile(args[0]);
            op_text("Downloaded: " + downloaded_file);

            //string txt_filename = ConvertWordToText(downloaded_file);
            //System.Console.WriteLine("Converted to text: " + txt_filename);

            //string paragraph = GetParagraphFromTextDocBasedonHeadingText(txt_filename);
            //string response = GenerateXMLOutput(paragraph, null);
            //System.Console.WriteLine(response);

        }//END   Main

        public static void op_text(String s)
        {
            System.Diagnostics.Debug.WriteLine(s);
            Console.WriteLine(s);
        }

        public static void parse_args(String[] args)
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
        }

        public static Boolean CheckValidString(String s)
        {
            return (!( (s == null) || s.Equals("")));
        }

        public static String GenerateXMLOutput(String paragraph, String err)
        {
            XElement e;
            if (CheckValidString(err)) {
                e= new XElement("error", err);
            }
            else {
                e= new XElement("paragraph", paragraph);
            }
            XDocument doc = new XDocument( new XElement("response", e ) );
            return doc.ToString();
        } // GenerateXMLOutput

        // TEST CODE
        // simulates the outputs we need to generate from  the overall get_data process 
        // allows for SN testing
        // e.g. get_data "testsingleassetid1" "Bill of Materials" will return <response><paragraph>1.0   WS-C4999-E  Cat4500 </paragraph></response>"
        // e.g. get_data "errorheadingnowsowassets" "Quote" will return <response><error>Assets not found in Quote</error></response>"
        public static String output_test_responses_based_on_file_name(String filename, String heading)
        {
            String s=null;
            String e=null;
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
            r=GenerateXMLOutput(s, e);
            return r; 

        }  // output_test_responses_based_on_file_name


        

public static void ExitError(String err)
{
    String e=GenerateXMLOutput("", err);
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
private static void DoDownloadFile(string webUrl, ICredentials credentials, string fileRelativeUrl, string localfile)
{
    using (var client = new WebClient())
    {
        client.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
        client.Headers.Add("User-Agent: Other");
        client.Credentials = credentials;
        String s = webUrl + '/' + fileRelativeUrl;
        try
        {
            client.DownloadFile(webUrl + '/' + fileRelativeUrl, localfile);
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
    const String URL = "https://sntestportal.portalfront.com/Shared%20Documents";
    tmp_file = GetTempFile(".doc");
    var client = new WebClient();
    client.Credentials = new NetworkCredential(username, password);
    DoDownloadFile(URL, client.Credentials, urlfilename, tmp_file);
    return (tmp_file);
}


        
// using regex extract a known heading
// we cannot rely on heading number
//
// approach
// read line by line
// when match regex of heading text (we add numbers for section headerer) e.g. if someone specifies
// heading "Bill of Materials", we have to search for 1.0 Bill of Materials
// read all lines of text paragraph until new section/heading (including "Appendix")
//
// todo handle error: no such file and missing regex
public static string GetParagraphFromTextDocBasedonHeadingText(String filename, String HeadingText)
{
    if (!CheckValidString(filename) && !CheckValidString(HeadingText)) {
        return null;
    }
    // match 1.0, or 11.0 etc then space then heading text e.g. 2.0 HeadingText
    // also match
    //     ^(([1-9][0-9]*\.[0-9]|[0-9][1-9]*)[\t ])?Heading Text matches tested here: http://regexstorm.net/tester
    //     2 Quote Costs
    // 3.0 Quote Costs
    // 2   Quote Costs
    // Quote Costs
    // NOTE regex tester doesn't seem to like start of line

    string reg_str=@"^(([1-9][0-9]*\.[0-9]|[0-9][1-9]*)[\t ])?"+HeadingText+"$";


    // to figure out if the next line is a header for next section match
    // Appendix 2 - Blah
    // Appendix 2: Blah
    // Appendix 3: Blah
    // Appendix 3 Blah
    // 3 Blah
    // 3.0 Blah
    // 2 Blah
    String  reg_for_next_heading=@"^(Appendix[\t ]+|)([1-9][0-9]*\.[0-9]|[0-9][1-9]*)(([ \:\-])+)[A-Z][a-z]+$";
    Regex rgx = new Regex(@reg_for_next_heading);
    Match  match;
    bool inHeader = false;
    int counter = 0;
    string line;
    string paragraph = "";

    // Read the file and display it line by line.
    System.IO.StreamReader file = new System.IO.StreamReader(filename);
    while((line = file.ReadLine()) != null)
    {
      counter++;
        if (inHeader) {
            // check e haven't reached end of paragraph in below header, if so finish
           if (rgx.IsMatch(line)) { break; }
            paragraph=paragraph + System.Environment.NewLine + line;
            continue;
        }

      // not in header, lets see if this line is start of header we need
      match = rgx.Match(line);

      // if not go to hnext line otherwise we are now in the header
      if (!match.Success) { continue; } 
      else { inHeader=true; }
    }
   file.Close();
   return(paragraph);
}


    }//END      Program
}//END         Add_Function