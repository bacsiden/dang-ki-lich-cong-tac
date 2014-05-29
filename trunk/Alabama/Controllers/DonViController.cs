using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace Alabama.Controllers
{
    public class DonViController : Controller
    {
        int pageSize = 20;
        //
        // GET: /Owner/

        public ActionResult Index(int? page)
        {
            return View(DB.Entities.DonVi.OrderByDescending(m => m.ID).ToPagedList(!page.HasValue ? 1 : page.Value, pageSize));
        }
        //
        // GET: /Owner/Edit/5

        public ActionResult NewOrEdit(int? id)
        {
            var obj = DB.Entities.DonVi.FirstOrDefault(m => m.ID == id);
            if (obj==null)
            {
                obj = new DonVi();
            }
            SelectOption(obj.LanhDaoID.HasValue?obj.LanhDaoID.Value:0);
            return View(obj);
        }

        void SelectOption(int id)
        {
            #region SELECT OPTION
            string dataUser = "<option >--Chọn lãnh đạo--</option>";
            foreach (var item in Alabama.DB.Entities.User)
            {
                if (id!=0 && item.ID==id)
                {
                    dataUser += string.Format("<option value='{0}' selected='selected'>{1}</option>", item.ID, item.Name);
                }
                else
                {
                    dataUser += string.Format("<option value='{0}'>{1}</option>", item.ID, item.Name);
                }
            }
            ViewBag.dataUser = dataUser;
            #endregion
        }
        //
        // POST: /Owner/Edit/5

        [HttpPost]
        public ActionResult NewOrEdit(DonVi model)
        {
            try
            {
                var db = DB.Entities;

                if (model.ID == 0)
                {
                    // Edit                    
                    db.DonVi.AddObject(model);
                }
                else
                {
                    // Add new      
                    db.AttachTo("DonVi", model);
                    db.ObjectStateManager.ChangeObjectState(model, System.Data.EntityState.Modified);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                SelectOption(model.LanhDaoID.HasValue ? model.LanhDaoID.Value : 0);
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
                        var obj = db.DonVi.FirstOrDefault(m => m.ID == tmpID);
                        db.DonVi.DeleteObject(obj);
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
