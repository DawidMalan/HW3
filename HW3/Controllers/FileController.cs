using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HW3.Models;

namespace HW3.Controllers
{
    public class FileController : Controller
    {
        private IFileSystem _fileSystem;
        private string fileDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files");

        public FileController()
        {
            _fileSystem = new FileSystem();
        }

        public ActionResult Index()
        {
            List<SavedReport> files = new List<SavedReport>();

            if (_fileSystem.Directory.Exists(fileDirectory))
            {
                foreach (string filePath in _fileSystem.Directory.GetFiles(fileDirectory))
                {
                    var fileInfo = _fileSystem.FileInfo.FromFileName(filePath);
                    files.Add(new SavedReport
                    {
                        FileName = fileInfo.Name,
                        FilePath = fileInfo.FullName
                    });
                }
            }

            return View(files);
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                string filePath = Path.Combine(fileDirectory, Path.GetFileName(file.FileName));
                file.SaveAs(filePath);
            }

            return RedirectToAction("Index");
        }

        public FileResult Download(string fileName)
        {
            string filePath = Path.Combine(fileDirectory, fileName);
            byte[] fileBytes = _fileSystem.File.ReadAllBytes(filePath);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        public ActionResult Delete(string fileName)
        {
            string filePath = Path.Combine(fileDirectory, fileName);
            if (_fileSystem.File.Exists(filePath))
            {
                _fileSystem.File.Delete(filePath);
            }

            return RedirectToAction("Index");
        }
    }
}