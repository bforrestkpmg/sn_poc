// Alternative 1

// tODO fugure out best way to approach this converstion of EOC to textBuildert
//http://stackoverflow.com/questions/1011234/how-to-extract-text-from-ms-office-documents-in-c-sharp
// need reference DocumentFormat.OpenXml.dll,

// We actually need doc to xml, so we can look forw headings etc.

// TODO check this works with tables
// TODO turn this into a full convert to text file (so loop through buffers as they fill up or are strings big enough - doubt it?)
public static string TextFromWord(SPFile file)
    {
        const string wordmlNamespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";

        StringBuilder textBuilder = new StringBuilder();
        using (WordprocessingDocument wdDoc = WordprocessingDocument.Open(file.OpenBinaryStream(), false))
        {
            // Manage namespaces to perform XPath queries.  
            NameTable nt = new NameTable();
            XmlNamespaceManager nsManager = new XmlNamespaceManager(nt);
            nsManager.AddNamespace("w", wordmlNamespace);

            // Get the document part from the package.  
            // Load the XML in the document part into an XmlDocument instance.  
            XmlDocument xdoc = new XmlDocument(nt);
            xdoc.Load(wdDoc.MainDocumentPart.GetStream());

            XmlNodeList paragraphNodes = xdoc.SelectNodes("//w:p", nsManager);
            foreach (XmlNode paragraphNode in paragraphNodes)
            {
                XmlNodeList textNodes = paragraphNode.SelectNodes(".//w:t", nsManager);
                foreach (System.Xml.XmlNode textNode in textNodes)
                {
                    textBuilder.Append(textNode.InnerText);
                }
                textBuilder.Append(Environment.NewLine);
            }

        }
        return textBuilder.ToString();
    }

    public static string ConvertWordToXML(string filename)
    {
        var string opfile;
        opfile=ConvertWordToOther("XML", filename);
        return opfile;
    }
    // ISSUE  need to check it preserves table layout
     public static string ConvertWordToText(string filename)
    {
        var string opfile;
        opfile=ConvertWordToOther("TXT", filename);
        return opfile;
    }
    public static string ConvertWordToOther(string Type, string filename)
    {
       //https://www.codeproject.com/articles/5273/how-to-convert-doc-into-other-formats-using-c 
    }


    // Alternative 2
    // 
    // Convert to HTML
    // use regex (see below ) to strip out HTML tags & preserve table layout
    // e.g. 
    // `cat TestDoc_DocWithHeading_andHeadingNumber_IncludesTable.htm | tr -d "\n" | sed -e $'s/\<h1\>/\\\n\<h1\>/g' | sed -e $'s/\<table/\\\n\<table/g' |  sed -e $'s/\<\/tr\>/\\\n/g' | sed -e $'s/\<\/h1\>/\\\n/g' | sed 's/\<\/td\>/        /g' | sed 's/<[^>]*>//g' | sed 's/\&nbsp\;//g'`   
    // # NOTE: make sure use Ctrl+V to insert tab s/\<\/td\>/        /g
    // 
    // returns text

    public static string SearchAndReplace(ref string buf, string reg, string rep)
    {
       Regex r = new Regex(@regex_str, RegexOptions.Multiline);
       buf=buf.replace(reg, rep);
    }

    public static string ConvertWordToText(string filename)
    {
        var string opfilename;
        opfilename=ConvertWordToHTML("HTML", filename);
        var string buffer; //ISSUE can a string hold the entire file? string length limitations? Google seems to suggest it can be up 2GB run time so should be ok
        var string simplified_text;
        // now read file into string 
        // ISSUE can a string hold the entire file?
        // otherwise how do we do the regex?

        // Read the file and display it line by line.
        System.IO.StreamReader file = new System.IO.StreamReader(opfile);
        while((line = file.ReadLine()) <> null)
        {
          buffer = buffer+line;
        }
       file.Close();

       // then perform regex over multi line
       // TODO work out how \n is expressed

       // remove new lines
       string regex_str="\n";
       SearchAndReplace(buffer, @regex_str, "");

       // replace header with newline header
       regex_str="\<h1\>";
       string replace_str="\n\<h1\>";
       SearchAndReplace(buffer, @regex_str, replace_str);

       // replace table with newline table 
       regex_str="\<table";
       replace_str="\n\<table";
       SearchAndReplace(buffer, @regex_str, replace_str);

       // replace h1 end tag with \n
       regex_str="\<\/h1\>";
       replace_str="\n";
       SearchAndReplace(buffer, @regex_str, replace_str);

       // replace table cell with tab
       regex_str="\<\/td\>";
       replace_str="    ";
       SearchAndReplace(buffer, @regex_str, replace_str);

       // replace all remaining html tags
       regex_str="<(.|\n)*?>";
       replace_str="";
       SearchAndReplace(buffer, @regex_str, replace_str);

       // replace all &nbsp; that are left over
       regex_str="\&nbsp\;"
       replace_str="";
       SearchAndReplace(buffer, @regex_str, replace_str);

        // return text
        return buffer;
    }