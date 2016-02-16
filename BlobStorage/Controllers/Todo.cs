using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlobStorage.Controllers
{
    public class Todo
    {
        public string name { get; set; }
        public int priority { get; set; }
        public string url { get; set; }
    }

    public class FirebaseCls
    {
        public async Task UpdateFireblob(CloudBlobContainer blobContainer)
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
            foreach (var blb in blobNames)
            {
                Todo td = new Todo();
                td.name = blb.Name;
                td.url = blb.Uri.AbsoluteUri.ToString();
                todoList.Add(td);
            }

            SetResponse response = await client.SetAsync("ringtones", todoList);
            List<Todo> setresult = response.ResultAs<List<Todo>>();

            //GET
            //FirebaseResponse getresponse = await client.GetAsync("ringtones");
            //List<Todo> gettodo = response.ResultAs<List<Todo>>(); //The response will contain the data being retreived
        }
    }
}