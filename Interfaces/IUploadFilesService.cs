using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UploadService.Interfaces
{
    public interface IUploadFilesService
    {
        void ProcessCSVFile(string customerName);

        void ProcessXMLFile(string customerName);

        string GetAllTranactionsByCurrency(string currency);

        string GetAllTranactionsByDateRange(string startDate, string enddate);

        string GetAllTranactionsByStatus(string status);
    }
}
