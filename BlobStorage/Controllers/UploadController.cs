using BlobStorage.Models;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace BlobStorage.Controllers
{
    public class UploadController : Controller
    {
        // GET: Upload
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadReport(HttpPostedFileBase files)
        {
            int ReportTypeID = Convert.ToInt32(HttpContext.Request.Form["ReportType"]);

            HttpPostedFileBase uploadedFile = files;
            try
            {
                AjaxResult res = new AjaxResult();
                if (uploadedFile.ContentLength > 0)
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

                    //CloudBlobContainer blobContainer = blobStorage.GetCloudBlobContainer();


                    //string filename = blobStorage.GetReadData("M0JJ0v8.png");
                    string UploadedFileName = uploadedFile.FileName;
                    CloudBlockBlob blob = blobContainer.GetBlockBlobReference(UploadedFileName);
                    blob.UploadFromStream(uploadedFile.InputStream);

                    //string page = System.Web.Configuration.WebConfigurationManager.AppSettings["FileAPIUrl"].ToString();


                    //using (HttpClient client = new HttpClient())
                    //{
                    //    client.BaseAddress = new Uri(page);

                    //    client.Timeout = TimeSpan.FromHours(1);

                    //    var result = client.PostAsync(page + "?UploadedFileName=" + UploadedFileName + "&FileType=" + ReportTypeID.ToString(), null);

                    //    result.Wait();

                    //    string resultContent = result.Result.Content.ReadAsStringAsync().Result;
                    //    if (String.IsNullOrEmpty(resultContent))
                    //    {
                    //        res.isSuccessfull = true;
                    //        res.Data = "File Uploaded Successfully";
                    //    }
                    //    else
                    //    {
                    //        res.isSuccessfull = false;
                    //        res.ErrorMessage = resultContent;
                    //    }
                    //}

                    return Json(res);
                }
                else
                {
                    return Json(new AjaxResult { isSuccessfull = false, Data = "", ErrorMessage = "Empty File" });
                }
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { isSuccessfull = false, Data = "", ErrorMessage = ex.InnerException.Message });
            }
        }

        public ContentResult GetSasUrl()
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


            return Content(GetContainerSasUri(blobContainer));

        }

        static string GetContainerSasUri(CloudBlobContainer container)
        {
            //Set the expiry time and permissions for the container.
            //In this case no start time is specified, so the shared access signature becomes valid immediately.
            SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy();
            sasConstraints.SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24);
            sasConstraints.Permissions = SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.List;

            //Generate the shared access signature on the container, setting the constraints directly on the signature.
            string sasContainerToken = container.GetSharedAccessSignature(sasConstraints);

            //Return the URI string for the container, including the SAS token.
            return container.Uri + sasContainerToken;
        }
    }
}