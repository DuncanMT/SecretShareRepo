using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace SecretShareProject.Models
{
    public class FileModels
    { }

    public class FileUploadModel
    {
        public HttpPostedFileBase file { get; set; }
        public int numshares { get; set; }
        public int minshares { get; set; }
    }

    public class FileInfoModel
    {
         public FileInfoModel()
        {
            Id = Guid.NewGuid().ToString();
        }
        [Key]
        public string Id { get; set; }
        [DisplayName("File Name")]
        public string fileName { get; set; }
        public string mimetype { get; set; }
        [DisplayName("Number of Shares")]
        public int numshares { get; set; }
        [DisplayName("Minimum Required Shares")]
        public int minshares { get; set; }
        public string userID { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<ShareModel> shares { get; set; }

    }

    public class ShareModel
    {
        public ShareModel()
        {
            Id = Guid.NewGuid().ToString();
        }
        [Key]
        public string Id { get; set; }
        public string storageService { get; set; }
        public string shareName { get; set; }
        public string fileId { get; set; }
        public virtual FileInfoModel file { get; set; }
    }
}