using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HW3.Models
{
    public class SavedReport
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string ReportId { get; set; }
        public string FileType { get; set; }
        public string DateSaved { get; set; }
    }
}
