using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HW3.Models;
using System.Web.Mvc;
using PagedList;

namespace HW3.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        private LibraryEntities db = new LibraryEntities();
        public ActionResult CombinedIndex()
        {
            var viewModel = new Combined
            {
                books = db.books.ToList(),
                authors = db.authors.ToList(),
                students = db.students.ToList()
                
            };
           
            return View(viewModel);
        }


        public ActionResult CombinedIndex2()
        {
            var viewModel = new Combined
            {
                borrows = db.borrows.ToList(),
                authors = db.authors.ToList(),
                types = db.types.ToList()

            };

            return View(viewModel);
        }
        //public ActionResult CombinedIndex(int? page)
        //{
        //    int pageSize = 10; // Set the page size as per your requirement

        //    var books = db.books.ToList();
        //    var students = db.students.ToList();

        //    var booksPaged = books.ToPagedList(page ?? 1, pageSize);
        //    var studentsPaged = students.ToPagedList(page ?? 1, pageSize);

        //    var combinedModel = new Combined
        //    {
        //        books = booksPaged,
        //        students = studentsPaged
        //    };

        //    return View(combinedModel);
        //}
    }
}