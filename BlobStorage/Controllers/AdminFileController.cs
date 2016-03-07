﻿using BlobStorage.Models;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace BlobStorage.Controllers
{
    public class AdminFileController : ApiController
    {
        public async Task<HttpResponseMessage> AdminUpload()
        {
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                {
                    this.Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);
                }

                var provider = GetMultipartProvider();
                var result = await Request.Content.ReadAsMultipartAsync(provider);


                // On upload, files are given a generic name like "BodyPart_26d6abe1-3ae1-416a-9429-b35f15e6e5d5"
                // so this is how you can get the original file name
                var originalFileName = GetDeserializedFileName(result.FileData.First());

                // uploadedFileInfo object will give you some additional stuff like file length,
                // creation time, directory name, a few filesystem methods etc..
                var uploadedFileInfo = new FileInfo(result.FileData.First().LocalFileName);

                // Remove this line as well as GetFormData method if you're not
                // sending any form data with your upload request
                var fileUploadObj = GetFormData<UploadDataModel>(result);

                BlobStorageService blobStorage = new BlobStorageService();

                // Retrieve storage account from connection string.
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

                // Create the blob client.
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                // Retrieve a reference to a container.
                CloudBlobContainer blobContainer = blobClient.GetContainerReference("ringtones");

                // Create the container if it doesn't already exist.
                blobContainer.CreateIfNotExists();

                //HttpPostedFileBase uploadedFile = result;
                CloudBlockBlob blob = blobContainer.GetBlockBlobReference(originalFileName);
                using (var fileStream = System.IO.File.OpenRead(uploadedFileInfo.FullName))
                {
                    blob.UploadFromStream(fileStream);
                }

                // Adding Meta Information
                FirebaseCls.FirebaseContainer = "ringtones";
                Todo.AddContainerMetadata(blob, fileUploadObj);

                FirebaseCls cls = new FirebaseCls();
                cls.UpdateFireblob(blobContainer);

                // Through the request response you can return an object to the Angular controller
                // You will be able to access this in the .success callback through its data attribute
                // If you want to send something to the .error callback, use the HttpStatusCode.BadRequest instead
                var returnData = "Sucess";
                return this.Request.CreateResponse(HttpStatusCode.OK, new { returnData });
            }
            catch (Exception e)
            {
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new { e.InnerException });
            }
        }

        // You could extract these two private methods to a separate utility class since
        // they do not really belong to a controller class but that is up to you
        private MultipartFormDataStreamProvider GetMultipartProvider()
        {
            var uploadFolder = "~/App_Data/Tmp/FileUploads"; // you could put this to web.config
            var root = HttpContext.Current.Server.MapPath(uploadFolder);
            Directory.CreateDirectory(root);
            return new MultipartFormDataStreamProvider(root);
        }

        // Extracts Request FormatData as a strongly typed model
        private object GetFormData<T>(MultipartFormDataStreamProvider result)
        {
            if (result.FormData.HasKeys())
            {
                var unescapedFormData = Uri.UnescapeDataString(result.FormData.GetValues(0).FirstOrDefault() ?? String.Empty);
                if (!String.IsNullOrEmpty(unescapedFormData))
                    return JsonConvert.DeserializeObject<T>(unescapedFormData);
            }

            return null;
        }

        private string GetDeserializedFileName(MultipartFileData fileData)
        {
            var fileName = GetFileName(fileData);
            return JsonConvert.DeserializeObject(fileName).ToString();
        }

        public string GetFileName(MultipartFileData fileData)
        {
            return fileData.Headers.ContentDisposition.FileName;
        }
    }
}
