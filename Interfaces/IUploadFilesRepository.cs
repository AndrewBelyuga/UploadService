using System.Data;

namespace UploadService.Interfaces
{
    public interface IUploadFilesRepository
    {
        void InsertRecords(DataTable table);
    }
}
