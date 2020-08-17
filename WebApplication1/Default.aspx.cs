using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication1
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string FirstName = Request.Form["Getinput"];
        }

        public string Post(string Getinput,string Getinput2) 
        {
            return string.Empty;
        }
    }
}