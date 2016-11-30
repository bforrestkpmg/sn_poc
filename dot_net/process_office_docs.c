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
     public static string ConvertWordToText(string filename)
    {
        var string opfile;
        opfile=ConvertWordToOther("XML", filename);
        return opfile;
    }
    public static string ConvertWordToOther(string Type, string filename)
    {
       //https://www.codeproject.com/articles/5273/how-to-convert-doc-into-other-formats-using-c 
    }