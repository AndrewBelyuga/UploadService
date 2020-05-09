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

        void GetAllTranactionsByCurrency(string currency);

        void GetAllTranactionsByDateRange(string startDate, string enddate);

        void GetAllTranactionsByStatus(string status);
    }
}
