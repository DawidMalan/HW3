using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HW3.Models
{
    public class TopBorrowedBooks
    {
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public int BorrowCount { get; set; }


        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
    }
}