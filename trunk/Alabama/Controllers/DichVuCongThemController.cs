using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Alabama.Controllers
{
    public class DichVuCongThemController : BaseController
    {
        //
        // GET: /DichVuCongThem/

        [Authorize]
        [ValidationFunction("/Home/index", ActionName.VIEWLISTDICHVUCONGTHEM)]
        public ActionResult Index()
        {
            return View(DB.Entities.DichVuCongThem);
        }

        //
        // GET: /CuocPhi/Details/5

        //
        // GET: /CuocPhi/Create

        [Authorize]
        [ValidationFunction("/DichVuCongThem/index", ActionName.ADDNEWDICHVUCONGTHEM)]
        public ActionResult Create()
        {
            return View(new DichVuCongThem());
        }

        //
        // POST: /CuocPhi/Create

        [HttpPost]
        [Authorize]
        [ValidationFunction("/DichVuCongThem/index", ActionName.ADDNEWDICHVUCONGTHEM)]
        public ActionResult Create(DichVuCongThem model)
        {
            try
            {
                // TODO: Add insert logic here
                var db = DB.Entities;
                db.DichVuCongThem.AddObject(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /CuocPhi/Edit/5

        [Authorize]
        [ValidationFunction("/DichVuCongThem/index", ActionName.EDITDICHVUCONGTHEM)]
        public ActionResult Edit(int id)
        {
            var obj = DB.Entities.DichVuCongThem.FirstOrDefault(m => m.ID == id);
            return View(obj);
        }

        //
        // POST: /CuocPhi/Edit/5

        [HttpPost]
        [Authorize]
        [ValidationFunction("/DichVuCongThem/index", ActionName.EDITDICHVUCONGTHEM)]
        public ActionResult DoEdit(DichVuCongThem model)
        {
            try
            {
                // TODO: Add update logic here
                var db = DB.Entities;
                var obj = db.DichVuCongThem.FirstOrDefault(m => m.ID == model.ID);
                obj.Name = model.Name;
                obj.Cost = model.Cost;
                db.ObjectStateManager.ChangeObjectState(obj, System.Data.EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /CuocPhi/Delete/5



        [Authorize]
        [ValidationFunction("/DichVuCongThem/index", ActionName.DELETEDICHVUCONGTHEM)]
        public ActionResult Delete(int id)
        {
            try
            {
                var db = DB.Entities;
                var obj = db.DichVuCongThem.FirstOrDefault(m => m.ID == id);
                db.DeleteObject(obj);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
