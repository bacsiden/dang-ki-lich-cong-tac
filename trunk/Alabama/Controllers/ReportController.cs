using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DKLCT.Controllers
{
    public class ReportController : Controller
    {
        //
        // GET: /Report/

        public ActionResult Index()
        {
            SelectOption();
            return View();
        }
        void SelectOption()
        {
            #region SELECT OPTION
            string dataDriver_Info = "<option value='' >--Select Driver_Info--</option>";
            foreach (var item in DKLCT.DB.Entities.Driver_Info)
            {
                dataDriver_Info += string.Format("<option value='{0}'>{1} {2}</option>", item.ID, item.Last_name, item.First_name);
            }
            ViewBag.dataDriver = dataDriver_Info;

            string dataDispatchers = "<option value='' >--Select Dispatchers--</option>";
            foreach (var item in DKLCT.DB.Entities.Dispatchers)
            {
                dataDispatchers += string.Format("<option value='{0}'>{1} {2}</option>", item.ID, item.Last_name, item.First_name);
            }
            ViewBag.dataDispatchers = dataDispatchers;
            #endregion
        }

    }
}
