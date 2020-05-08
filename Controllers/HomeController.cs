using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using UploadService.Models;
using System.Xml.Linq;
using System.Xml;

namespace UploadService.Controllers
{
    public class HomeController : Controller
    {

        SqlConnection con;
        string sqlConn;
        private readonly IConfiguration _configuration;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IConfiguration configuration, ILogger<HomeController> logger)
        {
            _configuration = configuration;
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
                var fileExtension = Path.GetExtension(file.FileName);
                var filePath = Path.GetFullPath(file.FileName);

                if (System.IO.File.Exists(file.FileName))
                {
                    System.IO.File.Delete(file.FileName);
                }

                // Create new local file and copy contents of uploaded file
                using (var localFile = System.IO.File.OpenWrite(file.FileName))
                using (var uploadedFile = file.OpenReadStream())
                {
                    uploadedFile.CopyTo(localFile);
                }

                if (fileExtension == ".csv")
                    ProcessCSVFile(filePath);
                if (fileExtension == ".xml")
                    ProcessXMLFile(filePath);
            }

            ViewBag.Message = "Files were successfully uploaded";

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private void connection()
        {
            sqlConn = _configuration.GetConnectionString("DefaultConnectionString");
            con = new SqlConnection(sqlConn);

        }

        private void ProcessCSVFile(string filePath)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Transaction Id");
            table.Columns.Add("Amount");
            table.Columns.Add("Currency Code");
            table.Columns.Add("Transaction Date");
            table.Columns.Add("Status");


            using (StreamReader sr = new StreamReader(filePath))
            {
                string line;
                // Read and display lines from the file until the end of 
                // the file is reached.
                while ((line = sr.ReadLine()) != null)
                {
                    //Adding each row into datatable  
                    table.Rows.Add();
                    int count = 0;
                    foreach (string FileRec in line.Split(','))
                    {
                        table.Rows[table.Rows.Count - 1][count] = FileRec;
                        count++;
                    }
                }
            }
            InsertRecords(table);
        }

        private void ProcessXMLFile(string filePath)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Transaction Id");
            table.Columns.Add("Amount");
            table.Columns.Add("Currency Code");
            table.Columns.Add("Transaction Date");
            table.Columns.Add("Status");

            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            foreach (XmlNode node in doc.DocumentElement)
            {
                string transactionId = node.Attributes[0].InnerText;
                int count = 0;
                foreach (XmlNode child in doc.ChildNodes)
                {
                    table.Rows[table.Rows.Count - 1][count] = child;
                    count++;
                }
            }
            //var result = doc.Descendants("Transactions");
            //var data = result.ToList();

            //foreach (var xe in data.First().Descendants())
            //    table.Columns.Add(xe.Name.LocalName, typeof(string));
            //// fill the data
            //foreach (var item in data)
            //{
            //    var row = table.NewRow();
            //    foreach (var xe in item.Descendants())
            //        row[xe.Name.LocalName] = xe.Value;
            //    table.Rows.Add(row);
            //}

            InsertRecords(table);
        }

        private void InsertRecords(DataTable table)
        {
            connection();
            //creating object of SqlBulkCopy    
            SqlBulkCopy objbulk = new SqlBulkCopy(con);
            //assigning Destination table name    
            objbulk.DestinationTableName = "UploadedInfo";
            //Mapping Table column    
            objbulk.ColumnMappings.Add("Transaction Id", "TransactionId");
            objbulk.ColumnMappings.Add("Amount", "Amount");
            objbulk.ColumnMappings.Add("Currency Code", "CurrencyCode");
            objbulk.ColumnMappings.Add("Transaction Date", "TransactionDate");
            objbulk.ColumnMappings.Add("Status", "StatusCode");
            //inserting Datatable Records to DataBase    
            con.Open();
            objbulk.WriteToServer(table);
            con.Close();
        }
    }
}
