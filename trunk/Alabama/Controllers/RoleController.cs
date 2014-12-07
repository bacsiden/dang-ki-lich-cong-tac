using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Alabama.Controllers
{
    [Authorize]
    public class RoleController : BaseController
    {
        //
        // GET: /Role/

        [Authorize]
        [ValidationFunction("/JobRegister/index", ActionName.VIEWLISTROLE)]
        public ActionResult Index()
        {
            return View(DB.Entities.Role1);
        }

        [Authorize]
        [ValidationFunction("/Role/index", ActionName.ADDNEWROLE)]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidationFunction("/Role/index", ActionName.ADDNEWROLE)]
        public ActionResult Create(Role1 model)
        {
            try
            {
                // TODO: Add insert logic here
                var db = DB.Entities;
                var role = new Role1() { Name = model.Name };
                db.Role1.AddObject(role);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [Authorize]
        [ValidationFunction("/Role/index", ActionName.EDITROLE)]
        public ActionResult Edit(int id = 0)
        {
            var obj = DB.Entities.Role1.First(m => m.ID == id);
            if (obj == null)
                return RedirectToAction("Index");
            return View(obj);
        }

        [HttpPost]
        [Authorize]
        [ValidationFunction("/Role/index", ActionName.EDITROLE)]
        public ActionResult Edit(Role1 model)
        {
            try
            {
                // TODO: Add update logic here
                var db = DB.Entities;
                var obj = db.Role1.First(m => m.ID == model.ID);
                obj.Name = model.Name;
                db.ObjectStateManager.ChangeObjectState(obj, System.Data.EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [Authorize]
        [ValidationFunction("/Role/index", ActionName.DELETEROLE)]
        public ActionResult Delete(int id)
        {
            try
            {
                var db = DB.Entities;
                var obj = db.Role1.First(m => m.ID == id);
                db.Role1.DeleteObject(obj);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }

        [Authorize]
        [ValidationFunction("/Role/index", ActionName.ADDFUNCTIONFORROLE)]
        public ActionResult Function(int id)
        {
            try
            {
                var db = DB.Entities;
                var lst = db.Function.Where(m => m.Role1.FirstOrDefault(n => n.ID == id) != null);
                string s = "";
                foreach (var item in db.Function)
                {
                    string check = "";
                    if (lst.FirstOrDefault(m => m.ID == item.ID) != null)
                    {
                        check = "checked='checked'";
                    }
                    s += "<label class='checkbox'><input type='checkbox' " + check + " value='" + item.ID + "' />" + item.Title + "</label>";
                }
                ViewBag.listFuntion = s;
                return View(db.Role1.FirstOrDefault(m => m.ID == id));

            }
            catch (Exception)
            {
                return View();
            }
        }

        [Authorize]
        [ValidationFunction("/Role/index", ActionName.ADDFUNCTIONFORROLE)]
        public ActionResult DoFunction(int roleID, string listCheck)
        {
            try
            {
                var db = DB.Entities;
                var role = db.Role1.FirstOrDefault(m => m.ID == roleID);
                string[] listChecked = listCheck.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in db.Function)
                {
                    if (listChecked.Contains(item.ID.ToString()))
                    {
                        if (role.Function.FirstOrDefault(m => m.ID == item.ID) == null)
                        {
                            role.Function.Add(item);
                        }
                    }
                    else
                        if (role.Function.FirstOrDefault(m => m.ID == item.ID) != null)
                        {
                            role.Function.Remove(item);
                        }
                }

                db.SaveChanges();
                return RedirectToAction("index");
            }
            catch (Exception)
            {
                return View();
            }
        }
    }


}

