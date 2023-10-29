using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace HW3.Models
{
    public class Combined
    {
        public IEnumerable<books> books{ get; set; } 
        public IEnumerable<borrows> borrows { get; set; }
        public IEnumerable<authors> authors { get; set; }
        public IEnumerable<students> students { get; set; }
        public IEnumerable<types>types { get; set; }
        public List<string> Bookstatus { get; set; } = new List<string>();
    }
}