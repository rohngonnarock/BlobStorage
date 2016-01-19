using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace BlobStorage.Models
{
    public class BlobStorageService
    {
        public CloudBlobContainer GetCloudBlobContainer()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudBlobClient blobclient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer blobcontainer = blobclient.GetContainerReference(System.Web.Configuration.WebConfigurationManager.AppSettings["BlobStorageContainerName"]);
            if (blobcontainer.CreateIfNotExists())
            {
                blobcontainer.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            }
            return blobcontainer;
        }

        public string GetReadData(string filename)
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["StorageConnectionString"]);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference(System.Web.Configuration.WebConfigurationManager.AppSettings["BlobStorageContainerName"]);

            // Retrieve reference to a blob named "myblob.csv"
            CloudBlockBlob blockBlob2 = container.GetBlockBlobReference(filename);

            string text;
            using (var memoryStream = new MemoryStream())
            {
                blockBlob2.DownloadToStream(memoryStream);
                text = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
            }

            return text;
        }

        public void DeleteBlob(string filename)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["StorageConnectionString"]);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference(System.Web.Configuration.WebConfigurationManager.AppSettings["BlobStorageContainerName"]);

            //foreach (IListBlobItem item in container.ListBlobs(null, false))
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(filename);

            blockBlob.DeleteIfExists();

        }
    }
}