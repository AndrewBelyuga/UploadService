using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using UploadService.Interfaces;

namespace UploadService.Infrastructure
{
    public class UploadFilesRepository : IUploadFilesRepository
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

        public string GetByCurrency(string currency)
        {
            SqlConnection conn = null;
            SqlDataReader reader = null;

            var result = "";

            try
            {
                conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnectionString"));
                conn.Open();
                SqlCommand cmd = new SqlCommand("select * from UploadedInfo where CurrencyCode = @CurrencyCode", conn);

                SqlParameter param = new SqlParameter();
                param.ParameterName = "@CurrencyCode";
                param.Value = currency;
                cmd.Parameters.Add(param);

                reader = cmd.ExecuteReader();

                result = toJSON(reader);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (conn != null)
                    conn.Close();
            }

            return result;
        }

        public string GetByStatus(string status)
        {
            SqlConnection conn = null;
            SqlDataReader reader = null;

            var result = "";

            try
            {
                conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnectionString"));
                conn.Open();
                SqlCommand cmd = new SqlCommand("select * from UploadedInfo where StatusCode = @StatusCode", conn);

                SqlParameter param = new SqlParameter();
                param.ParameterName = "@StatusCode";
                param.Value = status;
                cmd.Parameters.Add(param);

                reader = cmd.ExecuteReader();

                result = toJSON(reader);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (conn != null)
                    conn.Close();
            }

            return result;
        }
        public string GetByDateRange(string startDate, string endDate)
        {
            SqlConnection conn = null;
            SqlDataReader reader = null;

            var result = "";

            try
            {
                conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnectionString"));
                conn.Open();
                SqlCommand cmd = new SqlCommand("select * from UploadedInfo where TransactionDate BETWEEN @StartDate AND @EndDate", conn);

                SqlParameter param1 = new SqlParameter();
                param1.ParameterName = "@StartDate";
                param1.Value = startDate;
                SqlParameter param2 = new SqlParameter();
                param2.ParameterName = "@EndDate";
                param2.Value = endDate;
                cmd.Parameters.Add(param1);
                cmd.Parameters.Add(param2);

                reader = cmd.ExecuteReader();

                result = toJSON(reader);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (conn != null)
                    conn.Close();
            }

            return result;
        }

        private string toJSON(SqlDataReader o)
        {
            StringBuilder s = new StringBuilder();
            var status = "";
            s.Append("[");
            if (o.HasRows)
            {
                while (o.Read())
                {
                    switch (o["StatusCode"])
                    {
                        case "Approved":
                            status = "A";
                            break;
                        case "Failed":
                        case "Rejected":
                            status = "R";
                            break;
                        case "Finished":
                        case "Done":
                            status = "D";
                            break;
                        default:
                            break;
                    }

                    s.Append("{" + '"' + "Id" + '"' + ":" + o["TransactionId"] + ", "
                    + '"' + "Payment" + '"' + ":" + o["Amount"] + " " + o["CurrencyCode"] + ", "
                    + '"' + "Status" + '"' + ":" + status + "}, ");

                    s.Remove(s.Length - 2, 2);
                }
                o.Close();
            }
            s.Append("]");
            return s.ToString();
        }
    }
}
