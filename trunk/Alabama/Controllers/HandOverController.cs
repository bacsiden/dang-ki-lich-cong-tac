using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Alabama.Controllers
{
    public class HandOverController : Controller
    {
        //
        // GET: /HandOver/

        public ActionResult Index()
        {
            return View(new VPSS1Entities().HandOverCard);
        }

    }
}
