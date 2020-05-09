using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using UploadService.Interfaces;

namespace UploadService.Infrastructure
{
    public class UploadFilesService : IUploadFilesService
    {
        private readonly IUploadFilesRepository _uploadFilesRepository;
        private readonly ILogger<UploadFilesService> _logger;
        private bool allLinesAreProper = true;

        public UploadFilesService(IUploadFilesRepository uploadFilesRepository, ILogger<UploadFilesService> logger)
        {
            _uploadFilesRepository = uploadFilesRepository;
            _logger = logger;
        }
        public void ProcessCSVFile(string filePath)
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
                while ((line = sr.ReadLine()) != null)
                {
                    table.Rows.Add();
                    int count = 0;
                    foreach (string FileRec in line.Split(','))
                    {
                        CheckUploadingRecordAndLogInfo(FileRec,line);

                        table.Rows[table.Rows.Count - 1][count] = FileRec;
                        count++;
                    }
                }
            }
            if (allLinesAreProper)
                _uploadFilesRepository.InsertRecords(table);
            else
            {
                CancellUploadingAndLogInfo();
                return;
            }
        }

        public void ProcessXMLFile(string filePath)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Transaction Id");
            table.Columns.Add("Transaction Date");
            table.Columns.Add("Amount");
            table.Columns.Add("Currency Code");
            table.Columns.Add("Status");

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);

            foreach (XmlNode node in xmlDoc.DocumentElement)
            {
                table.Rows.Add();
                int count = 0;

                CheckUploadingRecordAndLogInfo(node.Attributes[0].InnerText, node.OuterXml);
                table.Rows[table.Rows.Count - 1][count] = node.Attributes[0].InnerText;
                count++;
                foreach (XmlNode child in node.ChildNodes)
                {
                    CheckUploadingRecordAndLogInfo(child.InnerText, node.OuterXml);

                    switch (child.Name)
                    {
                        case "TransactionDate":
                            table.Rows[table.Rows.Count - 1][count] = Convert.ToDateTime(child.InnerText);
                            count++;
                            break;
                        case "PaymentDetails":
                            for (int i = 0; i < child.ChildNodes.Count; i++)
                            {
                                CheckUploadingRecordAndLogInfo(child.ChildNodes[i].InnerText, node.OuterXml);

                                table.Rows[table.Rows.Count - 1][count] = child.ChildNodes[i].InnerText;
                                count++;
                            }
                            break;
                        default:
                            table.Rows[table.Rows.Count - 1][count] = child.InnerText;
                            count++;
                            break;
                    }
                }
            }
            if (allLinesAreProper)
                _uploadFilesRepository.InsertRecords(table);
            else
            {
                CancellUploadingAndLogInfo();
                return;
            }
        }

        private void CheckUploadingRecordAndLogInfo(string cell, string line)
        {
            if (string.IsNullOrEmpty(cell) || string.IsNullOrWhiteSpace(cell))
            {
                allLinesAreProper = false;

                _logger.LogInformation($"========={DateTime.Now}=========");
                _logger.LogInformation("Something wrong in line:");
                _logger.LogInformation(line);
            }
        }
        private void CancellUploadingAndLogInfo()
        {
            _logger.LogInformation("=====================================");
        }

        public string GetAllTranactionsByCurrency(string currency)
        {
            return _uploadFilesRepository.GetByCurrency(currency);
        }

        public string GetAllTranactionsByDateRange(string startDate, string enddate)
        {
            return _uploadFilesRepository.GetByDateRange(startDate, enddate);
        }

        public string GetAllTranactionsByStatus(string status)
        {
            return _uploadFilesRepository.GetByStatus(status);
        }

    }
}
