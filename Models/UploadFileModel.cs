using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using UploadService.Infrastructure;

namespace UploadService.Models
{
    public class UploadFileModel
    {
        [Required(ErrorMessage = "Please select a file.")]
        [DataType(DataType.Upload)]
        [MaxFileSize(1024 * 1024)]
        [AllowedExtensions(new string[] { ".csv", ".xml" })]
        public IFormFile UploadFile { get; set; }
    }
}
