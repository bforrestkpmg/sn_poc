
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
public static boolean DownloadFile(filename)
{
   return true;
}


class MainClass
    {
        static int Main(string[] args)
        {
            // Test if input arguments were supplied:
            if (args.Length < 2)
            {
                System.Console.WriteLine("Usage:: Please specifiy <url/filename> and <section header>");
                return 1;
            }

            DownloadFile(args[1]);

            System.Console.WriteLine("Finished");

            return 0;
        }
    }