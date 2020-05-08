using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using UploadService.Interfaces;

namespace UploadService.Infrastructure
{
    public class UploadFilesRepository: IUploadFilesRepository
    {
        private readonly IConfiguration _configuration;

        public UploadFilesRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void InsertRecords(DataTable table)
        {
            using (SqlBulkCopy objbulk = new SqlBulkCopy(_configuration.GetConnectionString("DefaultConnectionString"), SqlBulkCopyOptions.KeepIdentity))
            {
                objbulk.DestinationTableName = "UploadedInfo";

                objbulk.ColumnMappings.Add("Transaction Id", "TransactionId");
                objbulk.ColumnMappings.Add("Amount", "Amount");
                objbulk.ColumnMappings.Add("Currency Code", "CurrencyCode");
                objbulk.ColumnMappings.Add("Transaction Date", "TransactionDate");
                objbulk.ColumnMappings.Add("Status", "StatusCode");

                objbulk.WriteToServer(table);
            }
        }
    }
}
