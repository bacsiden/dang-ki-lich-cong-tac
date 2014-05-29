using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Alabama.Controllers
{
    public class MenuController : BaseController
    {
        [Authorize]
        [ValidationFunction("/Home/index", ActionName.VIEWLISTMENU)]
        public ActionResult Index()
        {
            return View(DB.Entities.Menu);
        }

        [Authorize]
        [ValidationFunction("/Menu/index", ActionName.ADDNEWMENU)]
        public ActionResult Create()
        {
            var db = new List<Menu>();
            var obj = new Menu() { ID = 0, Title = "None" };
            db.Add(obj);
            db.AddRange(DB.Entities.Menu);
            SelectList select = new SelectList(db.ToList(), "ID", "Title");
            ViewBag.dropDown = select;
            return View(new Menu());
        }

        //
        // POST: /CuocPhi/Create

        [HttpPost]
        [Authorize]
        [ValidationFunction("/Menu/index", ActionName.ADDNEWMENU)]
        public ActionResult Create(Menu model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // TODO: Add insert logic here
                    var db = DB.Entities;
                    if (model.ParentID == 0)
                    {
                        model.ParentID = null;
                    }
                    model.IsActive = true;
                    db.Menu.AddObject(model);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    var lst = new List<Menu>();
                    var obj = new Menu() { ID = 0, Title = "None" };
                    lst.Add(obj);
                    lst.AddRange(DB.Entities.Menu);
                    SelectList select = new SelectList(lst.ToList(), "ID", "Title");
                    ViewBag.dropDown = select;
                    return View();
                }
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /CuocPhi/Edit/5

        [Authorize]
        [ValidationFunction("/Menu/index", ActionName.DELETEMENU)]
        public ActionResult Edit(int id)
        {
            var obj = DB.Entities.Menu.FirstOrDefault(m => m.ID == id);
            var db = new List<Menu>();
            var dfobj = new Menu() { ID = 0, Title = "None" };
            db.Add(dfobj);
            db.AddRange(DB.Entities.Menu);
            SelectList select = new SelectList(db.ToList(), "ID", "Title", DB.Entities.Menu.FirstOrDefault(m => m.ID == obj.ParentID));
            ViewBag.dropDown = select;

            return View(obj);
        }

        //
        // POST: /CuocPhi/Edit/5

        [HttpPost]
        [Authorize]
        [ValidationFunction("/Menu/index", ActionName.EDITMENU)]
        public ActionResult Edit(Menu model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // TODO: Add update logic here
                    var db = DB.Entities;
                    var obj = db.Menu.FirstOrDefault(m => m.ID == model.ID);
                    obj.Title = model.Title;
                    obj.Oder = model.Oder;
                    if (model.ParentID == 0)
                    {
                        model.ParentID = null;
                    }
                    obj.ParentID = model.ParentID;
                    obj.Url = model.Url;
                    obj.Icon = model.Icon;
                    db.ObjectStateManager.ChangeObjectState(obj, System.Data.EntityState.Modified);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    var obj = DB.Entities.Menu.FirstOrDefault(m => m.ID == model.ID);
                    var db = new List<Menu>();
                    var dfobj = new Menu() { ID = 0, Title = "None" };
                    db.Add(dfobj);
                    db.AddRange(DB.Entities.Menu);
                    SelectList select = new SelectList(db.ToList(), "ID", "Title", DB.Entities.Menu.FirstOrDefault(m => m.ID == obj.ParentID));
                    ViewBag.dropDown = select;
                    return View(model);
                }

            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /CuocPhi/Delete/5



        [Authorize]
        [ValidationFunction("/Menu/index", ActionName.EDITMENU)]
        public ActionResult Delete(int id)
        {
            try
            {
                var db = DB.Entities;
                var obj = db.Menu.FirstOrDefault(m => m.ID == id);
                db.DeleteObject(obj);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        [ValidationFunction("/Menu/index", ActionName.VIEWLISTMENU)]
        public string DoActive(int id)
        {
            try
            {
                var db = DB.Entities;
                var obj = db.Menu.FirstOrDefault(m => m.ID == id);
                obj.IsActive = !obj.IsActive;
                db.ObjectStateManager.ChangeObjectState(obj, System.Data.EntityState.Modified);
                db.SaveChanges();
                if (obj.IsActive == true)
                {
                    return "Đã bật";
                }
                return "<span class='validation-summary-errors'>Đã tắt</span>";
            }
            catch (Exception ex)
            {

                return "";
            }

        }


    }
}
