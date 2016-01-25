using BlobStorage.Models;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BlobStorage.Controllers
{
    public class RingtonesController : ApiController
    {
        public HttpResponseMessage GetRingtones()
        {
            BlobStorageService blobStorage = new BlobStorageService();

            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            CloudBlobContainer blobContainer = blobClient.GetContainerReference("ringtones");

            // Create the container if it doesn't already exist.
            blobContainer.CreateIfNotExists();

            var list = blobContainer.ListBlobs();

            List<string> blobNames = list.OfType<CloudBlockBlob>().Select(b => b.Name).ToList();

            var returnData = blobNames;
            return Request.CreateResponse(HttpStatusCode.OK, returnData);
        }
    }
}
