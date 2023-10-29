using HW3.Models;
using Rotativa;
using System.IO;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NPOI.XWPF.UserModel;
using System.Text;
using NPOI.HSSF.Model;
using System.Drawing.Drawing2D;
using System.Collections.Generic;



public class ChartController : Controller
{
    private LibraryEntities db = new LibraryEntities(); // Replace with your DbContext
   
    public ActionResult Index()
    {
        var downloads = GetDownloadRecordsFromDirectory(); // Implement this function to retrieve download records
        ViewBag.Downloads = downloads;

        var topBooks = db.borrows
            .GroupBy(b => b.bookId)
            .Select(g => new TopBorrowedBooks
            {
                BookId = (int)g.Key,
                BorrowCount = g.Count()
            })
            .OrderByDescending(b => b.BorrowCount)
            .Take(5)
            .ToList();

        // Retrieve book names for the top books
        foreach (var book in topBooks)
        {
            var bookInfo = db.books.Find(book.BookId);
            if (bookInfo != null)
            {
                book.BookTitle = bookInfo.bookId.ToString();
            }
        }

        return View(topBooks);
    }

    // Action to generate the chart view as a PDF
    public ActionResult DownloadChartAsPdf(string filename)
    {
        var topBooks = db.borrows
            .GroupBy(b => b.bookId)
            .Select(g => new TopBorrowedBooks
            {
                BookId = (int)g.Key,
                BorrowCount = g.Count()
            })
            .OrderByDescending(b => b.BorrowCount)
            .Take(5)
            .ToList();

        // Retrieve book names for the top books
        foreach (var book in topBooks)
        {
            var bookInfo = db.books.Find(book.BookId);
            if (bookInfo != null)
            {
                book.BookTitle = bookInfo.bookId.ToString();
            }
        }

        // Use Rotativa to generate the PDF and set the filename
        var pdf = new ViewAsPdf("Index", topBooks)
        {
            FileName = filename // Set the desired filename
        };

        return pdf;
    }
    public ActionResult DownloadTop5BooksReport(string filename, string fileType)
    {
        var topBooks = db.borrows
        .GroupBy(b => b.bookId)
        .Select(g => new TopBorrowedBooks
        {
            BookId = (int)g.Key,
            BorrowCount = g.Count()
        })
        .OrderByDescending(b => b.BorrowCount)
        .Take(5)
        .ToList();

        // Retrieve book names for the top books
        foreach (var book in topBooks)
        {
            var bookInfo = db.books.Find(book.BookId);
            if (bookInfo != null)
            {
                book.BookTitle = bookInfo.bookId.ToString();
            }
        }

        var customFilename = string.IsNullOrEmpty(filename) ? "Top5BooksReport" : filename;

        byte[] reportContent;
        string contentType = "";

        if (fileType.Equals("pdf", StringComparison.OrdinalIgnoreCase))
        {
            // Use Rotativa to generate the PDF report from the custom partial view
            var pdf = new PartialViewAsPdf("DownloadTop5BooksReport", topBooks);

            // Set custom switches for page margins and media type
            pdf.CustomSwitches = "--print-media-type --margin-top 10 --margin-right 10 --margin-bottom 10 --margin-left 10";

            // Generate the PDF report content
            reportContent = pdf.BuildPdf(ControllerContext);
            contentType = "application/pdf";
        }
        else if (fileType.Equals("txt", StringComparison.OrdinalIgnoreCase))
        {
            // Generate the report content as plain text
            string textReport = GenerateTextReport(topBooks); // Implement this method to generate text content

            // Convert the text content to bytes
            reportContent = Encoding.UTF8.GetBytes(textReport);
            contentType = "text/plain";
        }
       
        else
        {
            // Handle other file types or provide an error message for unsupported types
            // You can customize this part based on your specific requirements
            return HttpNotFound();
        }

        // Save the report to the directory
        SaveReportToDirectory(reportContent, fileType, customFilename);

        // Set the Content-Disposition header to prompt for download with a custom filename
        Response.AppendHeader("Content-Disposition", $"attachment; filename={customFilename}.{fileType}");

        // Set the Content-Type header
        Response.AppendHeader("Content-Type", contentType);

        // Return the report content as a FileResult
        return File(reportContent, contentType);
    }
    private string GenerateTextReport(List<TopBorrowedBooks> topBooks)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("Top Borrowed Books Report");
        sb.AppendLine();
        sb.AppendLine("Book ID\tBook Title\tBorrow Count");

        foreach (var book in topBooks)
        {
            sb.AppendLine($"{book.BookId}\t{book.BookTitle}\t{book.BorrowCount}");
        }

        return sb.ToString();
    }

    private void SaveReportToDirectory(byte[] reportContent, string fileType, string customFilename)
    {
        // Specify the directory where you want to save the report
        string directoryPath = @"C:\Users\user\Dawid2(SSD)\INF 272\HW3\HW3\Files\";

        // Ensure the directory exists
        Directory.CreateDirectory(directoryPath);

        // Generate a unique filename
        string uniqueFilename = Guid.NewGuid().ToString();

        // Combine the directory path, unique filename, and file extension based on the fileType parameter
        string fileExtension = "";

        switch (fileType)
        {
            case "pdf":
                fileExtension = ".pdf";
                break;
            case "txt":
                fileExtension = ".txt";
                break;
            case "doc":
                fileExtension = ".doc";
                break;
            default:
                // Handle unsupported file types
                break;
        }

        if (!string.IsNullOrEmpty(fileExtension))
        {
            string filePath = Path.Combine(directoryPath, uniqueFilename + fileExtension);

            // Save the report content to the specified directory
            System.IO.File.WriteAllBytes(filePath, reportContent);
        }

    }


    // Action to generate and download the report in TXT format
    public ActionResult DownloadTop5BooksReportTxt(string filename)
    {
        var topBooks = db.borrows
            .GroupBy(b => b.bookId)
            .Select(g => new TopBorrowedBooks
            {
                BookId = (int)g.Key,
                BorrowCount = g.Count()
            })
            .OrderByDescending(b => b.BorrowCount)
            .Take(5)
            .ToList();

        // Retrieve book names for the top books
        foreach (var book in topBooks)
        {
            var bookInfo = db.books.Find(book.BookId);
            if (bookInfo != null)
            {
                book.BookTitle = bookInfo.bookId.ToString();
            }
        }

        // Create a StringBuilder to hold the content in plain text format
        var contentBuilder = new StringBuilder();
        contentBuilder.AppendLine("Top 5 Borrowed Books Report");
        contentBuilder.AppendLine("Book ID\tBook Title\tBorrow Count");

        foreach (var book in topBooks)
        {
            contentBuilder.AppendLine($"{book.BookId}\t{book.BookTitle}\t{book.BorrowCount}");
        }

        // Get the plain text content
        string txtContent = contentBuilder.ToString();

        // Set the Content-Disposition header to prompt for download with a custom filename
        if (!string.IsNullOrEmpty(filename))
        {
            Response.AppendHeader("Content-Disposition", $"attachment; filename={filename}.txt");
        }
        else
        {
            Response.AppendHeader("Content-Disposition", "attachment; filename=Top5BooksReport.txt");
        }

        Response.AppendHeader("Content-Type", "text/plain");

        // Return the TXT content as a downloadable file
        return File(Encoding.UTF8.GetBytes(txtContent), "text/plain");
    }
   
    public ActionResult DownloadTop5BooksReportDoc(string filename)
    {
        var topBooks = db.borrows
            .GroupBy(b => b.bookId)
            .Select(g => new TopBorrowedBooks
            {
                BookId = (int)g.Key,
                BorrowCount = g.Count()
            })
            .OrderByDescending(b => b.BorrowCount)
            .Take(5)
            .ToList();

        // Retrieve book names for the top books
        foreach (var book in topBooks)
        {
            var bookInfo = db.books.Find(book.BookId);
            if (bookInfo != null)
            {
                book.BookTitle = bookInfo.bookId.ToString();
            }
        }

        // Create a StringBuilder to hold the content in DOC format
        var contentBuilder = new StringBuilder();
        contentBuilder.AppendLine("Top 5 Borrowed Books Report");
        contentBuilder.AppendLine("Book ID\tBook Title\tBorrow Count");

        foreach (var book in topBooks)
        {
            contentBuilder.AppendLine($"{book.BookId}\t{book.BookTitle}\t{book.BorrowCount}");
        }

        // Get the DOC content
        string docContent = contentBuilder.ToString();

        // Set the Content-Disposition header to prompt for download with a custom filename
        if (!string.IsNullOrEmpty(filename))
        {
            Response.AppendHeader("Content-Disposition", $"attachment; filename={filename}.doc");
        }
        else
        {
            Response.AppendHeader("Content-Disposition", "attachment; filename=Top5BooksReport.doc");
        }

        Response.AppendHeader("Content-Type", "application/msword");

        // Return the DOC content as a downloadable file
        return File(Encoding.UTF8.GetBytes(docContent), "application/msword");
    }





    private ActionResult DownloadFile(string fileName, string fileType)
    {
        string filePath = Path.Combine(Server.MapPath("~/Files"), $"{fileName}.{fileType}");
        if (System.IO.File.Exists(filePath))
        {
            return File(filePath, "application/octet-stream", fileName);
        }
        // Handle the case when the file does not exist
        return HttpNotFound();
    }

    // Function to delete a file
    private void DeleteFile(string fileName, string fileType)
    {
        string filePath = Path.Combine(Server.MapPath("~/Files"), $"{fileName}.{fileType}");
        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }
        // Handle the case when the file does not exist or could not be deleted
        
    }
    public ActionResult Download(string filename, string fileType)
    {
        return DownloadFile(filename, fileType);
    }
    [HttpPost]
    public ActionResult Delete(string filename, string fileType)
    {
        DeleteFile(filename, fileType);
        return RedirectToAction("Index");
    }
    public List<DownloadRecord> GetDownloadRecordsFromDirectory()
    {
        string directoryPath = Server.MapPath("~/Files"); // Path to your "Files" directory
        List<DownloadRecord> downloadRecords = new List<DownloadRecord>();

        // Check if the directory exists
        if (Directory.Exists(directoryPath))
        {
            // Get a list of files in the directory
            var files = Directory.GetFiles(directoryPath);

            // Extract the file name and file type from each file
            foreach (var filePath in files)
            {
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                var fileType = Path.GetExtension(filePath).TrimStart('.').ToLower();

                downloadRecords.Add(new DownloadRecord
                {
                    FileName = fileName,
                    FileType = fileType
                });
            }
        }

        return downloadRecords;
    }



    public ActionResult CopyDownloadedFile(string filename, string fileType)
    {
        // Path to the original downloaded file
        string originalFilePath = Server.MapPath("~/Files") + filename + "." + fileType;

        // Path to the destination directory
        string destinationDirectory = Server.MapPath("~/Files/");

        // Path to the copied file
        string copiedFilePath = Path.Combine(destinationDirectory, filename + "." + fileType);

        // Copy the file
        System.IO.File.Copy(originalFilePath, copiedFilePath);

        // You can return a message or data to indicate the success of the operation
        return Json(new { message = "File copied successfully" });
    }
}

