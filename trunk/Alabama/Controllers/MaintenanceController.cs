using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace DKLCT.Controllers
{
    public class MaintenanceController : Controller
    {
        int pageSize = 20;
        //
        // GET: /Owner/

        public ActionResult Index(int? page)
        {
            return View(DB.Entities.Maintenance.OrderByDescending(m => m.Id).ToPagedList(!page.HasValue ? 0 : page.Value, pageSize));
        }
        //
        // GET: /Owner/Edit/5

        public ActionResult NewOrEdit(int? id = 0)
        {
            var obj = DB.Entities.Maintenance.FirstOrDefault(m => m.Id == id);
            string dataEquipment = "<option >--Select Equiment--</option>";
            foreach (var item in DKLCT.DB.Entities.Equipment)
            {
                if (obj != null && obj.Track_Trailer_Number == item.ID)
                {
                    dataEquipment += string.Format("<option value='{0}' selected='selected'>{1} - {2}</option>", item.ID, item.Equipment_number, item.Equipment_Type);
                }
                else
                {
                    dataEquipment += string.Format("<option value='{0}'>{1} - {2}</option>", item.ID, item.Equipment_number, item.Equipment_Type);
                }
            }
            ViewBag.dataEquipment = dataEquipment;
            return View(obj);
        }

        //
        // POST: /Owner/Edit/5

        [HttpPost]
        public ActionResult NewOrEdit(Maintenance model)
        {
            try
            {
                var db = DB.Entities;

                if (model.Id == 0)
                {
                    // Edit                    
                    db.Maintenance.AddObject(model);
                }
                else
                {
                    // Add new      
                    db.AttachTo("Maintenance", model);
                    db.ObjectStateManager.ChangeObjectState(model, System.Data.EntityState.Modified);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                string dataEquipment = "<option >--Select Equiment--</option>";
                foreach (var item in DKLCT.DB.Entities.Equipment)
                {
                    if (model != null && model.Track_Trailer_Number == item.ID)
                    {
                        dataEquipment += string.Format("<option value='{0}' selected='selected'>{1} - {2}</option>", item.ID, item.Equipment_number, item.Equipment_Type);
                    }
                    else
                    {
                        dataEquipment += string.Format("<option value='{0}'>{1} - {2}</option>", item.ID, item.Equipment_number, item.Equipment_Type);
                    }
                }
                ViewBag.dataEquipment = dataEquipment;
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
                        var obj = db.Maintenance.FirstOrDefault(m => m.Id == tmpID);
                        db.Maintenance.DeleteObject(obj);
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
