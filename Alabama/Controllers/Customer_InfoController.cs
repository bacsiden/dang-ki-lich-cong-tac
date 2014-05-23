using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;


namespace Alabama.Controllers
{
    public class Customer_InfoController : Controller
    {
        int pageSize = 20;
        //
        // GET: /Customer_Info/

        public ActionResult Index(int? page)
        {            
            return View(DB.Entities.Customer_Info.OrderByDescending(m=>m.Customer_ID).ToPagedList(!page.HasValue ? 0 : page.Value,pageSize));
        }

        //
        // GET: /Customer_Info/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Customer_Info/Create

        public ActionResult Create()
        {
            return View(new Customer_Info());
        } 

        //
        // POST: /Customer_Info/Create

        [HttpPost]
        public ActionResult Create(Customer_Info model)
        {
            try
            {
                // TODO: Add insert logic here
                var db = DB.Entities;
                var obj = new Customer_Info();
                obj = model;
                db.Customer_Info.AddObject(obj);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        
        //
        // GET: /Customer_Info/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View(DB.Entities.Customer_Info.FirstOrDefault(m=>m.Customer_ID==id));
        }

        //
        // POST: /Customer_Info/Edit/5

        [HttpPost]
        public ActionResult Edit(Customer_Info model)
        {
            try
            {
                // TODO: Add update logic here
                var db = DB.Entities;
                var obj = db.Customer_Info.FirstOrDefault(m=>m.Customer_ID==model.Customer_ID);
                obj.City = model.City;
                obj.Comments = model.Comments;
                obj.Contact = model.Contact;
                obj.Customer_Name = model.Customer_Name;
                obj.Days_to_pay = model.Days_to_pay;
                obj.Fax = model.Fax;
                obj.Phone = model.Phone;
                obj.Street = model.Street;
                obj.ZIP_Code = model.ZIP_Code;
                db.ObjectStateManager.ChangeObjectState(obj, System.Data.EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

       
        
        public ActionResult Delete(int id)
        {
            try
            {
                // TODO: Add delete logic here
                var db = DB.Entities;
                var obj = db.Customer_Info.FirstOrDefault(m=>m.Customer_ID==id);
                db.Customer_Info.DeleteObject(obj);
                db.SaveChanges();                
            }
            catch
            {
                
            }
            return RedirectToAction("Index");
        }
    }
}
