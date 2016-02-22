using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlobStorage.Models
{
    public class UploadDataModel
    {
        public string Name { get; set; }
        public string Number { get; set; }
        public string Email { get; set; }
        public string Comments { get; set; }
        public string Category { get; set; }
    }
}