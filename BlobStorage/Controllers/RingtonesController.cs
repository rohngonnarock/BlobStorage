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



            // Loop over items within the container and output the length and URI.
            foreach (IListBlobItem item in blobContainer.ListBlobs(null, false))
            {
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob blob = (CloudBlockBlob)item;

                    Console.WriteLine("Block blob of length {0}: {1}", blob.Properties.Length, blob.Uri);

                }
                else if (item.GetType() == typeof(CloudPageBlob))
                {
                    CloudPageBlob pageBlob = (CloudPageBlob)item;

                    Console.WriteLine("Page blob of length {0}: {1}", pageBlob.Properties.Length, pageBlob.Uri);

                }
                else if (item.GetType() == typeof(CloudBlobDirectory))
                {
                    CloudBlobDirectory directory = (CloudBlobDirectory)item;

                    Console.WriteLine("Directory: {0}", directory.Uri);
                }
            }





            // Create the container if it doesn't already exist.
            blobContainer.CreateIfNotExists();

            var list = blobContainer.ListBlobs();

            List<string> blobNames = list.OfType<CloudBlockBlob>().Select(b => b.Name).ToList();

            var returnData = blobNames;
            return Request.CreateResponse(HttpStatusCode.OK, returnData);
        }
    }
}
