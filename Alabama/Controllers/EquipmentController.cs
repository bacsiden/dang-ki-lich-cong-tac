using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace DKLCT.Controllers
{
    public class EquipmentController : Controller
    {
        int pageSize = 20;
        //
        // GET: /Owner/

        public ActionResult Index(int? page)
        {
            return View(DB.Entities.Equipment.OrderByDescending(m => m.ID).ToPagedList(!page.HasValue ? 0 : page.Value, pageSize));
        }
        //
        // GET: /Owner/Edit/5

        public ActionResult NewOrEdit(int? id = 0)
        {
            var obj = DB.Entities.Equipment.FirstOrDefault(m => m.ID == id);
            if (obj == null) obj = new Equipment();


            string dataDriver = "<option >--Select Driver--</option>";
            foreach (var item in DKLCT.DB.Entities.Driver_Info)
            {
                if (obj != null && obj.Driver == item.ID)
                {
                    dataDriver += string.Format("<option value='{0}' selected='selected'>{1} {2}</option>", item.ID, item.Last_name, item.First_name);
                }
                else
                {
                    dataDriver += string.Format("<option value='{0}'>{1} {2}</option>", item.ID, item.Last_name, item.First_name);
                }
            }
            ViewBag.dataDriver = dataDriver;

            return View(obj);
        }

        //
        // POST: /Owner/Edit/5

        [HttpPost]
        public ActionResult NewOrEdit(Equipment model)
        {
            try
            {
                var db = DB.Entities;

                if (model.ID == 0)
                {
                    // Edit                    
                    db.Equipment.AddObject(model);
                }
                else
                {
                    // Add new      
                    db.AttachTo("Equipment", model);
                    db.ObjectStateManager.ChangeObjectState(model, System.Data.EntityState.Modified);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                string dataDriver = "<option >--Select Driver--</option>";
                foreach (var item in DKLCT.DB.Entities.Driver_Info)
                {
                    if (model != null && model.Driver == item.ID)
                    {
                        dataDriver += string.Format("<option value='{0}' selected='selected'>{1} {2}</option>", item.ID, item.Last_name, item.First_name);
                    }
                    else
                    {
                        dataDriver += string.Format("<option value='{0}'>{1} {2}</option>", item.ID, item.Last_name, item.First_name);
                    }
                }
                ViewBag.dataDriver = dataDriver;
                return View();
            }
        }

        //
        // GET: /Owner/Delete/5

        public ActionResult Delete(string arrayID = "")
        {
            try
            {
                // TODO: Add delete logic here
                var lstID = arrayID.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var db = DB.Entities;
                if (lstID.Length > 0)
                {
                    foreach (var item in lstID)
                    {
                        int tmpID = int.Parse(item);
                        var obj = db.Equipment.FirstOrDefault(m => m.ID == tmpID);
                        db.Equipment.DeleteObject(obj);
                    }
                    db.SaveChanges();
                }
            }
            catch
            {

            }
            return RedirectToAction("Index");
        }

    }
}
