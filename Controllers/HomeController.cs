using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using UploadService.Models;
using UploadService.Interfaces;

namespace UploadService.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUploadFilesService _uploadFilesService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IConfiguration configuration, ILogger<HomeController> logger, IUploadFilesService uploadFilesService)
        {
            _configuration = configuration;
            _logger = logger;
            _uploadFilesService = uploadFilesService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(IFormFile[] files)
        {
            foreach (var file in files)
            {
                var fileName = file.FileName;
                var fileExtension = Path.GetExtension(fileName);
                var filePath = Path.GetFullPath(fileName);

                if (System.IO.File.Exists(fileName))
                {
                    System.IO.File.Delete(fileName);
                }

                using (var localFile = System.IO.File.OpenWrite(fileName))
                using (var uploadedFile = file.OpenReadStream())
                {
                    uploadedFile.CopyTo(localFile);
                }

                if (fileExtension == ".csv")
                    _uploadFilesService.ProcessCSVFile(filePath);
                if (fileExtension == ".xml")
                    _uploadFilesService.ProcessXMLFile(filePath);
            }

            ViewBag.Message = "Files were successfully uploaded";

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
