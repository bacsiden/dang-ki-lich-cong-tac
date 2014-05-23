using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Alabama.Controllers
{
    public class KhachHangController : BaseController
    {
        //
        // GET: /KhachHang/

        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
        [Authorize]
        [ValidationFunction("/Home/index", ActionName.VIEWLISTKHACHHANG)]
        public ActionResult IndexKhachHang(int id = 0)
        {
            try
            {

                @ViewBag.GroupName = "Danh sách khách hàng";
                return View(DB.Entities.KhachHang);
            }
            catch (Exception ex)
            {
                return View(new List<KhachHang>());
            }
        }
        [Authorize]
        [ValidationFunction("/Home/index", ActionName.ADDNEWKHACHHANG)]
        public ActionResult DoCreateKH(KhachHang model)
        {
            try
            {
                var db = DB.Entities;
                db.KhachHang.AddObject(model);
                db.SaveChanges();
                return RedirectToAction("IndexKhachHang");
            }
            catch (Exception ex)
            {
                return View(new List<KhachHang>());
            }
        }
        [Authorize]
        [ValidationFunction("/Home/index", ActionName.ADDNEWKHACHHANG)]
        public ActionResult CreateKH()
        {
            return View(new KhachHang());
        }
        [Authorize]
        [ValidationFunction("/Home/index", ActionName.DELETEKHACHHANG)]
        public ActionResult DeleteKH(int id)
        {
            try
            {
                var db = DB.Entities;
                var obj = db.KhachHang.FirstOrDefault(m => m.ID == id);
                db.DeleteObject(obj);
                db.SaveChanges();
                return RedirectToAction("IndexKhachHang");
            }
            catch (Exception ex)
            {
                return RedirectToAction("IndexKhachHang");
            }
        }
        [Authorize]
        [ValidationFunction("/Home/index", ActionName.EDITKHACHHANG)]
        public ActionResult EditKH(int id = 0)
        {
            var obj = DB.Entities.KhachHang.First(m => m.ID == id);
            if (obj == null)
                return RedirectToAction("IndexKhachHang");
            return View(obj);
        }

        [HttpPost]
        [Authorize]
        [ValidationFunction("/Home/index", ActionName.EDITKHACHHANG)]
        public ActionResult EditKH(KhachHang model)
        {
            try
            {
                // TODO: Add update logic here
                var db = DB.Entities;
                var obj = db.KhachHang.First(m => m.ID == model.ID);
                obj.Name = model.Name;
                obj.Address = model.Address;
                obj.Email = model.Email;
                obj.Phone = model.Phone;
                db.ObjectStateManager.ChangeObjectState(obj, System.Data.EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("IndexKhachHang");
            }
            catch
            {
                return View();
            }
        }
    }
}
