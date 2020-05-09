using System.Data;

namespace UploadService.Interfaces
{
    public interface IUploadFilesRepository
    {
        void InsertRecords(DataTable table);

        public string GetByCurrency(string currency);

        public string GetByStatus(string status);

        public string GetByDateRange(string startDate, string endDate);
    }
}
