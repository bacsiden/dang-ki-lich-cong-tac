using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace Alabama.Controllers
{
    public class CardController : Controller
    {
        //
        // GET: /Card/

        //public ActionResult Index()
        //{
        //    return View();
        //}
        //[HttpGet]
        public ActionResult Index(int createdCardID = 0)
        {
            int a = 0;
            int.TryParse(Request.QueryString["page"], out a);
            if (a == 0) a = 1;
            int page = a;
            if (createdCardID == 0)
            {
                var lst = DB.Entities.Card;
                return View(lst.OrderBy(m=>m.ID).ToPagedList(page, ConfigInfo.PageSize));
            }
            return View(new VPSS1Entities().Card.Where(m => m.CreatedCardID == createdCardID).OrderBy(m => m.ID).ToPagedList(page, ConfigInfo.PageSize));
        }
        //
        // GET: /Card/Details/5

        public ActionResult Details(int id)
        {
            var obj = DB.Entities.Card.FirstOrDefault(m => m.ID == id);
            return View(obj);
        }

        //
        // GET: /Card/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Card/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Card/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Card/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Card/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Card/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
