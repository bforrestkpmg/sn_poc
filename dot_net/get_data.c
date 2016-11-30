
// use this
// https://code.msdn.microsoft.com/office/file-from-SharePoint-Online-cc418dba
//
// or this could be better
// https://blogs.msdn.microsoft.com/sowmyancs/2007/09/14/how-to-download-files-from-a-sharepoint-document-library-remotely-via-lists-asmx-webservice-sps-2003-moss-2007/
//
// or this looks even sipmler
// https://www.codeproject.com/questions/436092/download-file-from-sharepoint-document-library
//
// username / pwd: http://stackoverflow.com/questions/6025687/download-a-file-with-password-and-username-with-c-sharp
//
//

using System.Xml;
using System.Xml.Schema;




// for our test server username and password are part of http login
username='bforrest@kpmg.com.au';
password='mypassw0rd+';
// this should be passed into as command line but for testing...
URL="https://sntestportal.portalfront.com"

public static bool CheckValidString(s)
{
    if ((s == null) || (s.length <=0)) { return false; }
    return true;
}

// returns xml as per our agreed standard
// if error is not "" or not null, then returns <response><error> otherwise <response><paragraph>
public static string GenerateXMLOutput(string paragraph, string error)
{
    var e;
    if CheckValidString(error) {
        e= new XElement("error", error)
    }
    else {
        e= new XElement("paragraph", paragraph)
    }
    XDocument doc = new XDocument( new XElement("response", e ) );
    return doc.ToString();
} // GenerateXMLOutput


public static ExitError(err)
{
    e=GenerateXMLOutput("", err);
    System.Console.WriteLine(response);
    Environment.Exit(1);
}

// TODO error check creates ok
// returns unique/temp filename with extension provided
public static string GetTempFile(string ext)
{
string fileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ext;
return fileName
}
//returns extension
public static string GetExt(String filename)
{
    return Path.GetExtension(myFilePath);
}
//returns temp file name of downloadedfile
// TODO check this works with username password for http authentication
public static string DownloadFile(String urlfilename)
{
    using (WebClient client = new WebClient()) {
            client.Credentials = new NetworkCredential(username, password);
            try {
                tmp_file=GetTempFile(GetExt(urlfilename);
                client.DownloadFile(urlfilename, @tmp_file);
            }
            catch {
                ExitError("DownloadFile failed.")
            }
            return tmp_file;
        }
}

// using regex extract a known heading
// we cannot rely on heading number
//
// approach
// read line by line
// when match regex  we add numbers before
// read all lines of text paragraph until new section/heading
//
// todo handle error: no such file and missing regex

public static string GetParagraphFromTextDocBasedonHeadingText(String filename, String HeadingText)
{
    if (!CheckValidString(filename)  && !CheckValidString(HeadingText))
        return null;
    // match 1.0, or 11.0 etc then space then heading text e.g. 2.0 HeadingText
    // also match
    //     ^(([1-9][0-9]*\.[0-9]|[0-9][1-9]*)[\t ])?Heading Text matches tested here: http://regexstorm.net/tester
    //     2 Quote Costs
    // 3.0 Quote Costs
    // 2   Quote Costs
    // Quote Costs
    // NOTE regex tester doesn't seem to like start of line

    reg_str="^(([1-9][0-9]*\.[0-9]|[0-9][1-9]*)[\t ])?"+HeadingText+"$"


    // to figure out if the next line is a header for next section match
    // Appendix 2 - Blah
    // Appendix 2: Blah
    // Appendix 3: Blah
    // Appendix 3 Blah
    // 3 Blah
    // 3.0 Blah
    // 2 Blah
    reg_for_next_heading=="^(Appendix[\t ]+|)([1-9][0-9]*\.[0-9]|[0-9][1-9]*)(([ \:\-])+)[A-Z][a-z]+$"
    Regex r = new Regex(@reg_str, RegexOptions.Singleline);
    Match match;
    bool inHeader;
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
            match = reg_for_next_heading.Match(line);
            if (!match.Success) { break; }
            paragraph=paragraph + System.Environment.NewLine + line;
            continue;
        }

      // not in header, lets see if this line is start of header we need
      match = regex.Match(line);

      // if not go to hnext line otherwise we are now in the header
      if (!match.Success) { continue; } 
      else { inHeader=true; }
    }
   file.Close();
   return(paragraph);
}



// approach
// check args (url/file and heading name)
// create temporary file name
// download file
// open file and search for heading

class MainClass
    {
        static int Main(string[] args)
        {
            // Test if input arguments were supplied:
            if (args.Length < 2) {
                System.Console.WriteLine("Usage:: Please specifiy <url/filename> and <section header>");
                return 1;
            }

            downloaded_file=DownloadFile(args[1]);
            System.Console.WriteLine("Downloaded: " + downloaded_file);

            txt_filename=ConvertWordToText(downloaded_file);
            System.Console.WriteLine("Converted to text: " + txt_filename);

            paragraph=GetParagraphFromTextDocBasedonHeadingText(txt_filename);
            string response=GenerateXMLOutput(paragraph, null);
            System.Console.WriteLine(response);

            return 0;
        } // Main
    } // class MainClass