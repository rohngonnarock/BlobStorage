using BlobStorage.Models;
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
    public class DownloadController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Download()
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            CloudBlobContainer blobContainer = blobClient.GetContainerReference("ringtones");

            var list = blobContainer.ListBlobs();

            List<string> blobNames = list.OfType<CloudBlockBlob>().Select(b => b.Name).ToList();

            CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(blobNames[0]);
            CloudBlob blockBlob2 = blobContainer.GetBlobReference(blobNames[0]);

            MemoryStream stream = new MemoryStream();
            blockBlob2.DownloadToStream(stream);

            System.Web.HttpContext.Current.Response.ContentType = blockBlob2.Properties.ContentType;
            System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "Attachment; filename=" + blobNames[0].ToString());
            System.Web.HttpContext.Current.Response.AddHeader("Content-Length", blockBlob2.Properties.Length.ToString());
            System.Web.HttpContext.Current.Response.BinaryWrite(stream.ToArray());
            
            return Request.CreateResponse(HttpStatusCode.OK, "Success");
        }
    }
}
