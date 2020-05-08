﻿using System;
using System.Data;
using System.IO;
using System.Xml;
using UploadService.Interfaces;

namespace UploadService.Infrastructure
{
    public class UploadFilesService : IUploadFilesService
    {
        private readonly IUploadFilesRepository _uploadFilesRepository;

        public UploadFilesService(IUploadFilesRepository uploadFilesRepository)
        {
            _uploadFilesRepository = uploadFilesRepository;
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
                        table.Rows[table.Rows.Count - 1][count] = FileRec;
                        count++;
                    }
                }
            }
            _uploadFilesRepository.InsertRecords(table);
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

                table.Rows[table.Rows.Count - 1][count] = node.Attributes[0].InnerText;
                count++;
                foreach (XmlNode child in node.ChildNodes)
                {
                    switch (child.Name)
                    {
                        case "TransactionDate":
                            table.Rows[table.Rows.Count - 1][count] = Convert.ToDateTime(child.InnerText);
                            count++;
                            break;
                        case "PaymentDetails":
                            for (int i = 0; i < child.ChildNodes.Count; i++)
                            {
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

            _uploadFilesRepository.InsertRecords(table);
        }
    }
}
