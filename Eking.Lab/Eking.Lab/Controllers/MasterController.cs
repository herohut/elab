using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Eking.Lab.Controllers
{
    public class MasterController : Controller
    {
        //
        // GET: /Master/

        public ActionResult Chat()
        {
            return View();
        }


        public ActionResult Gomoku()
        {
            return View();
        }
    }
}
