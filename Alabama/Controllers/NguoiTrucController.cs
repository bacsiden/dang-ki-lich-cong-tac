using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace Alabama.Controllers
{
    [Authorize]
    public class NguoiTrucController : BaseController
    {
        int pageSize = 20;
        //
        // GET: /Owner/

        [Authorize]
        [ValidationFunction("/JobRegister/index", ActionName.ViewCategory)]
        public ActionResult Index(int? page)
        {
            return View(DB.Entities.NguoiTruc.OrderByDescending(m => m.ID).ToPagedList(!page.HasValue ? 0 : page.Value, pageSize));
        }
        //
        // GET: /Owner/Edit/5

        [Authorize]
        [ValidationFunction("/JobRegister/index", ActionName.NewOrEditCategory)]
        public ActionResult NewOrEdit(int? id = 0)
        {
            var obj = DB.Entities.NguoiTruc.FirstOrDefault(m => m.ID == id);
            return View(obj);
        }

        //
        // POST: /Owner/Edit/5

        [HttpPost]
        [Authorize]
        [ValidationFunction("/JobRegister/index", ActionName.NewOrEditCategory)]
        public ActionResult NewOrEdit(NguoiTruc model)
        {
            try
            {
                var db = DB.Entities;

                if (model.ID == 0)
                {
                    // Edit                    
                    db.NguoiTruc.AddObject(model);
                }
                else
                {
                    // Add new      
                    db.AttachTo("NguoiTruc", model);
                    db.ObjectStateManager.ChangeObjectState(model, System.Data.EntityState.Modified);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Owner/Delete/5

        [Authorize]
        [ValidationFunction("/JobRegister/index", ActionName.DeleteCategory)]
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
                        var obj = db.NguoiTruc.FirstOrDefault(m => m.ID == tmpID);
                        db.NguoiTruc.DeleteObject(obj);
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
