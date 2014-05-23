using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Alabama.Controllers
{
    public class CuocPhiController : BaseController
    {
        //
        // GET: /CuocPhi/
        [Authorize]
        [ValidationFunction("/Home/index", ActionName.VIEWLISTCUOCPHI)]
        public ActionResult Index()
        {
            return View(DB.Entities.CuocPhi);
        }

        //
        // GET: /CuocPhi/Details/5

        //
        // GET: /CuocPhi/Create
        [Authorize]
        [ValidationFunction("/CuocPhi/index", ActionName.ADDNEWCUOCPHI)]
        public ActionResult Create()
        {
            return View(new CuocPhi());
        } 

        //
        // POST: /CuocPhi/Create
        [Authorize]
        [HttpPost]
        [ValidationFunction("/CuocPhi/index", ActionName.ADDNEWCUOCPHI)]
        public ActionResult Create(CuocPhi model)
        {
            try
            {
                // TODO: Add insert logic here
                var db = DB.Entities;
                db.CuocPhi.AddObject(model);
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
         [ValidationFunction("/CuocPhi/index", ActionName.EDITCUOCPHI)]
        public ActionResult Edit(int id)
        {
            var obj = DB.Entities.CuocPhi.FirstOrDefault(m => m.ID == id);
            return View(obj);
        }

        //
        // POST: /CuocPhi/Edit/5
        [Authorize]
        [HttpPost]
        [ValidationFunction("/CuocPhi/index", ActionName.EDITCUOCPHI)]
        public ActionResult DoEdit(CuocPhi model)
        {
            try
            {
                // TODO: Add update logic here
                var db = DB.Entities;
                var obj = db.CuocPhi.FirstOrDefault(m => m.ID == model.ID);
                obj.Title = model.Title;
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
        [ValidationFunction("/CuocPhi/index", ActionName.DELETECUOCPHI)]
        public ActionResult Delete(int id)
        {
            try
            {
                var db = DB.Entities;
                var obj = db.CuocPhi.FirstOrDefault(m => m.ID == id);
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
