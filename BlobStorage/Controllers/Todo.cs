using BlobStorage.Models;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace BlobStorage.Controllers
{
    public class Todo
    {
        public string name { get; set; }
        public string url { get; set; }
        public string Category { get; internal set; }
        public string UserName { get; internal set; }
        public string Number { get; internal set; }
        public string Email { get; internal set; }
        public string Comments { get; internal set; }

        public static void AddContainerMetadata(CloudBlockBlob blob, object fileUploadObj)
        {
            var obj = ((BlobStorage.Models.UploadDataModel)fileUploadObj);
            //Add some metadata to the container.


            if (IsNotNull(obj.Category))
                blob.Metadata["category"] = obj.Category;
            else
                blob.Metadata["category"] = "Other";

            if (IsNotNull(obj.Name))
                blob.Metadata["name"] = obj.Name;
            else
                blob.Metadata["name"] = "Null";

            if (IsNotNull(obj.Number))
                blob.Metadata["number"] = obj.Number;
            else
                blob.Metadata["number"] = "Null";

            if (IsNotNull(obj.Email))
                blob.Metadata["email"] = obj.Email;
            else
                blob.Metadata["email"] = "Null";

            if (IsNotNull(obj.Comments))
                blob.Metadata["comments"] = obj.Comments;
            else
                blob.Metadata["comments"] = "Null";

            //Set the container's metadata.
            blob.SetMetadata();
        }

        private static bool IsNotNull(string name)
        {
            if (name == null || name == string.Empty)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static void ListContainerMetadata(CloudBlobContainer container)
        {
            //Fetch container attributes in order to populate the container's properties and metadata.
            container.FetchAttributes();

            //Enumerate the container's metadata.

            //Console.WriteLine("Container metadata:");
            //foreach (var metadataItem in container.Metadata)
            //{
            //    Console.WriteLine("\tKey: {0}", metadataItem.Key);
            //    Console.WriteLine("\tValue: {0}", metadataItem.Value);
            //}
        }
    }

    public class FirebaseCls
    {
        public static string FirebaseContainer { get; internal set; }

        public async Task UpdateFireblob(CloudBlobContainer blobContainer)
        {

            try
            {
                IFirebaseConfig config = new FirebaseConfig
                {
                    AuthSecret = "Gg5t1fSLC0WWPVM1VMoNxlM29qO1s53dEso7Jrfp",
                    BasePath = "https://ringtoneapp.firebaseio.com/"
                };

                IFirebaseClient client = new FirebaseClient(config);
                var list = blobContainer.ListBlobs();
                List<CloudBlockBlob> blobNames = list.OfType<CloudBlockBlob>().ToList();

                // SET
                var todo = new Todo();

                List<Todo> todoList = new List<Todo>();
                List<UploadDataModel> MetaList = new List<UploadDataModel>();
                foreach (var blb in blobNames)
                {
                    blb.FetchAttributes();
                    Todo td = new Todo();
                    td.name = blb.Name;
                    td.url = blb.Uri.AbsoluteUri.ToString();
                    if (blb.Metadata.Values.Count > 0)
                    {
                        td.Category = blb.Metadata.Values.ElementAt(0);
                        td.UserName = blb.Metadata.Values.ElementAt(1);
                        td.Number = blb.Metadata.Values.ElementAt(2);
                        td.Email = blb.Metadata.Values.ElementAt(3);
                        td.Comments = blb.Metadata.Values.ElementAt(4);
                    }
                    todoList.Add(td);
                }

                SetResponse response = await client.SetAsync(FirebaseContainer, todoList);
                List<Todo> setresult = response.ResultAs<List<Todo>>();
            }
            catch (Exception e)
            {

            }

            //GET
            //FirebaseResponse getresponse = await client.GetAsync("ringtones");
            //List<Todo> gettodo = response.ResultAs<List<Todo>>(); //The response will contain the data being retreived
        }
    }
}