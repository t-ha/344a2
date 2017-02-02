using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using System.Diagnostics;
using System.Configuration;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;

namespace WebRole1
{
    /// <summary>
    /// Summary description for getQuerySuggestions
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    [ScriptService]
    public class getQuerySuggestions : System.Web.Services.WebService
    {
        private PerformanceCounter memProcess = new PerformanceCounter("Memory", "Available MBytes");
        private static string wikiFileStream { get; set; }
        private static Trie wikiTrie;

        private float GetAvailableMBytes()
        {
            float memUsage = memProcess.NextValue();
            return memUsage;
        }

        [WebMethod]
        public string DownloadWiki()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("pa2container");
            CloudBlockBlob blockBlob = container.GetBlockBlobReference("wikiTitles.txt");

            using (var fileStream = System.IO.File.OpenWrite(System.IO.Path.GetTempFileName()))
            {
                wikiFileStream = fileStream.Name;
                blockBlob.DownloadToStream(fileStream);
            }

            return "Download complete.";
        }


        [WebMethod]
        public string BuildTrie()
        {
            wikiTrie = new Trie();
            string line = "";
            using (StreamReader sr = new StreamReader(wikiFileStream))
            {
                float availableMBytes = GetAvailableMBytes();
                int iter = 0;
                while (sr.EndOfStream == false && availableMBytes > 20)
                {
                    line = sr.ReadLine();
                    wikiTrie.AddTitle(line);
                    if (iter % 10000 == 0)
                    {
                        availableMBytes = GetAvailableMBytes();
                    }
                    iter++;
                }
            }
            return "Trie built. Last title in trie: [" + line + "]";
        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SearchTrie(string prefix)
        {
            return new JavaScriptSerializer().Serialize(wikiTrie.SearchForPrefix(prefix));
        }
    }
}