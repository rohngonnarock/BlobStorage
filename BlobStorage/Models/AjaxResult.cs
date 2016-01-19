using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlobStorage.Models
{
    public class AjaxResult
    {
        public bool isSuccessfull { get; set; }
        public string Data { get; set; }
        public string ErrorMessage { get; set; }
    }
}