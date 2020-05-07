using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UploadService.Models;

namespace UploadService.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload(IFormFile[] files)
        {
            foreach (var file in files)
            {
                var fileName = System.IO.Path.GetFileName(file.FileName);
                var extension = System.IO.Path.GetExtension(file.FileName);

                if (System.IO.File.Exists(fileName))
                {
                    System.IO.File.Delete(fileName);
                }

                using (var localFile = System.IO.File.OpenWrite(fileName))
                using (var uploadedFile = file.OpenReadStream())
                {
                    uploadedFile.CopyTo(localFile);
                }

                if (file == null || file.Length == 0)
                {
                    return await Task.FromResult((string)null);
                }

                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    return await reader.ReadToEndAsync();
                }

                //string ReadCSV = System.IO.File.ReadAllText(CSVFilePath);

                //foreach (string csvRow in ReadCSV.Split('\n'))
                //{
                //    if (!string.IsNullOrEmpty(csvRow))
                //    {
                //        //Adding each row into datatable  
                //        tblcsv.Rows.Add();
                //        int count = 0;
                //        foreach (string FileRec in csvRow.Split(','))
                //        {
                //            tblcsv.Rows[tblcsv.Rows.Count - 1][count] = FileRec;
                //            count++;
                //        }
                //    }
                //}
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
