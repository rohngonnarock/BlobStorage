using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BlobStorage.Controllers
{
    public class ToneController : ApiController
    {
        // GET: api/Tone
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Tone/5
        //http://localhost:62378/api/tone?name=sdfs
        public string Get(string name)
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            CloudBlobContainer blobContainer = blobClient.GetContainerReference("ringtones");

            var list = blobContainer.ListBlobs();

            List<string> blobNames = list.OfType<CloudBlockBlob>().Select(b => b.Name).ToList();
            string getTone = string.Empty;

            if (name != null)
            {
                getTone = name;
            }
            else
            {
                getTone = blobNames[0];
            }
            CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(getTone);
            CloudBlob blockBlob2 = blobContainer.GetBlobReference(getTone);

            MemoryStream stream = new MemoryStream();
            blockBlob2.DownloadToStream(stream);

            System.Web.HttpContext.Current.Response.ContentType = blockBlob2.Properties.ContentType;
            System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "Attachment; filename=" + getTone.ToString());
            System.Web.HttpContext.Current.Response.AddHeader("Content-Length", blockBlob2.Properties.Length.ToString());
            System.Web.HttpContext.Current.Response.BinaryWrite(stream.ToArray());

            return "value";
        }

        // POST: api/Tone
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Tone/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Tone/5
        //http://localhost:62378/api/tone?name=sdfs
        public void Delete(string name)
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference("ringtones");

            // Retrieve reference to a blob named "myblob.txt".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(name);

            blockBlob.Delete();

            FirebaseCls cls = new FirebaseCls();
            cls.UpdateFireblob(container);
        }
    }
}
