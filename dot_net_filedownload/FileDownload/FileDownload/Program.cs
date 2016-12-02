using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace FileDownload
{
    class Program
    {
        public static void ExitError(String err)
        {
            
            System.Console.WriteLine(err);
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
                String s=webUrl+'/'+fileRelativeUrl;
                try  {
                client.DownloadFile(webUrl+'/'+fileRelativeUrl, localfile);
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
            tmp_file=GetTempFile(".doc");
            var client = new WebClient();
            client.Credentials = new NetworkCredential(username, password);
            DoDownloadFile(URL, client.Credentials, urlfilename, tmp_file);
            return (tmp_file);
        }

       static void Main(string[] args)
        {
            String f = DownloadFile("QuoteAsTable.doc");
            System.Diagnostics.Debug.WriteLine("Successfully download: " + f);
            Console.WriteLine("Successfully download: " + f);
        }
    }
}
