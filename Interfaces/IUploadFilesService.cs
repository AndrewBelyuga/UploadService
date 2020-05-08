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
    }
}
