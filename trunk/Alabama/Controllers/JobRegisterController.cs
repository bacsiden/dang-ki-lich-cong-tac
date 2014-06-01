using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace Alabama.Controllers
{
    public class JobRegisterController : Controller
    {
        int pageSize = 20;
        //
        // GET: /Owner/

        public ActionResult Index(int? page)
        {
            return View(DB.Entities.JobRegister.OrderByDescending(m => m.ID).ToPagedList(!page.HasValue ? 1 : page.Value, pageSize));
        }
        //
        // GET: /Owner/Edit/5

        public ActionResult NewOrEdit(int? id)
        {
            var currentUser = new UserDAL().GetCurrentUser;
            var obj = DB.Entities.JobRegister.FirstOrDefault(m => m.ID == id);
            if (obj == null)
            {
                obj = new JobRegister() { UserID=currentUser.ID,DateFrom=DateTime.Now,Created=DateTime.Now};
                ViewBag.DateFrom = obj.DateFrom.ToString("08:00 AM");                 
            }
            else
            {
                ViewBag.DateFrom = obj.DateFrom.ToString("hh:mm tt");
            }                      
            ViewBag.UserName = currentUser.Name;
            SelectOption(obj.LocationID.HasValue ? obj.LocationID.Value : 0);
            return View(obj);
        }

        void SelectOption(int locationID)
        {
            #region SELECT OPTION            

            string dataLocation = "<option >--Chọn địa điểm--</option>";
            foreach (var item in Alabama.DB.Entities.Location)
            {
                if (locationID != 0 && item.ID == locationID)
                {
                    dataLocation += string.Format("<option value='{0}' selected='selected'>{1}</option>", item.ID, item.Title);
                }
                else
                {
                    dataLocation += string.Format("<option value='{0}'>{1}</option>", item.ID, item.Title);
                }
            }
            ViewBag.dataLocation = dataLocation;
            #endregion
        }
        //
        // POST: /Owner/Edit/5

        [HttpPost]
        public ActionResult NewOrEdit(JobRegister model,FormCollection frm)
        {
            try
            {
                var created = DateTime.Parse(frm["Created"]);
                var db = DB.Entities;
                if (model.ID == 0)
                {
                    // Edit                    
                    db.JobRegister.AddObject(model);
                }
                else
                {
                    // Add new      
                    db.AttachTo("JobRegister", model);
                    db.ObjectStateManager.ChangeObjectState(model, System.Data.EntityState.Modified);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.UserName = new UserDAL().GetCurrentUser.Name;
                ViewBag.DateFrom = model.DateFrom.ToString("hh:mm tt");
                ViewBag.DateCreated = model.Created.ToString("hh:mm tt");    
                SelectOption(model.LocationID.HasValue ? model.LocationID.Value : 0);
                return View(model);
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
                        var obj = db.JobRegister.FirstOrDefault(m => m.ID == tmpID);
                        db.JobRegister.DeleteObject(obj);
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
