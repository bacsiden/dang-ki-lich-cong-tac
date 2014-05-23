using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Alabama.Controllers
{
    public class DanhMucController : BaseController
    {
        //
        // GET: /DanhMuc/
        #region Danh Mục Loại Hàng
        [Authorize]
        [ValidationFunction("/Home/index", ActionName.VIEWLISTLOAIHANG)]
        public ActionResult LoaiHangIndex()
        {
            return View(DB.Entities.LoaiHang);
        }

        //
        // GET: /CuocPhi/Details/5

        //
        // GET: /CuocPhi/Create

        [Authorize]
        [ValidationFunction("/DanhMuc/LoaiHangIndex", ActionName.ADDNEWLOAIHANG)]
        public ActionResult LoaiHangCreate()
        {
            return View(new LoaiHang());
        }

        //
        // POST: /CuocPhi/Create

        [HttpPost]
        [Authorize]
        [ValidationFunction("/DanhMuc/LoaiHangIndex", ActionName.ADDNEWLOAIHANG)]
        public ActionResult LoaiHangCreate(LoaiHang model)
        {
            try
            {
                // TODO: Add insert logic here
                var db = DB.Entities;
                db.LoaiHang.AddObject(model);
                db.SaveChanges();
                return RedirectToAction("LoaiHangIndex");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /CuocPhi/Edit/5

        [Authorize]
        [ValidationFunction("/DanhMuc/LoaiHangIndex", ActionName.EDITLOAIHANG)]
        public ActionResult LoaiHangEdit(int id)
        {
            var obj = DB.Entities.LoaiHang.FirstOrDefault(m => m.ID == id);
            return View(obj);
        }

        //
        // POST: /CuocPhi/Edit/5

        [HttpPost]
        [Authorize]
        [ValidationFunction("/DanhMuc/LoaiHangIndex", ActionName.EDITLOAIHANG)]
        public ActionResult LoaiHangEdit(LoaiHang model)
        {
            try
            {
                // TODO: Add update logic here
                var db = DB.Entities;
                var obj = db.LoaiHang.FirstOrDefault(m => m.ID == model.ID);
                obj.Title = model.Title;
                db.ObjectStateManager.ChangeObjectState(obj, System.Data.EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("LoaiHangIndex");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /CuocPhi/Delete/5



        [Authorize]
        [ValidationFunction("/DanhMuc/LoaiHangIndex", ActionName.DELETELOAIHANG)]
        public ActionResult LoaiHangDelete(int id)
        {
            try
            {
                var db = DB.Entities;
                var obj = db.LoaiHang.FirstOrDefault(m => m.ID == id);
                db.DeleteObject(obj);
                db.SaveChanges();
                return RedirectToAction("LoaiHangIndex");
            }
            catch
            {
                return View();
            }
        }
        #endregion

        #region Danh Mục Bưu cục
        [Authorize]
        [ValidationFunction("/Home/index", ActionName.VIEWLISTBUUCUC)]
        public ActionResult BuuCucIndex()
        {
            return View(DB.Entities.BuuCuc);
        }

        //
        // GET: /CuocPhi/Details/5

        //
        // GET: /CuocPhi/Create

        [Authorize]
        [ValidationFunction("/DanhMuc/BuuCucIndex", ActionName.ADDNEWBUUCUC)]
        public ActionResult BuuCucCreate()
        {
            return View(new BuuCuc());
        }

        //
        // POST: /CuocPhi/Create

        [HttpPost]
        [Authorize]
        [ValidationFunction("/DanhMuc/BuuCucIndex", ActionName.ADDNEWBUUCUC)]
        public ActionResult BuuCucCreate(BuuCuc model)
        {
            try
            {
                // TODO: Add insert logic here
                var db = DB.Entities;
                db.BuuCuc.AddObject(model);
                db.SaveChanges();
                return RedirectToAction("BuuCucIndex");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /CuocPhi/Edit/5

        [Authorize]
        [ValidationFunction("/DanhMuc/BuuCucIndex", ActionName.EDITBUUCUC)]
        public ActionResult BuuCucEdit(int id)
        {
            var obj = DB.Entities.BuuCuc.FirstOrDefault(m => m.ID == id);
            return View(obj);
        }

        //
        // POST: /CuocPhi/Edit/5

        [HttpPost]
        [Authorize]
        [ValidationFunction("/DanhMuc/BuuCucIndex", ActionName.EDITBUUCUC)]
        public ActionResult BuuCucEdit(BuuCuc model)
        {
            try
            {
                // TODO: Add update logic here
                var db = DB.Entities;
                var obj = db.BuuCuc.FirstOrDefault(m => m.ID == model.ID);
                obj.TenBuuCuc = model.TenBuuCuc;
                obj.Phone = model.Phone;
                obj.DiaChi = model.DiaChi;
                db.ObjectStateManager.ChangeObjectState(obj, System.Data.EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("BuuCucIndex");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /CuocPhi/Delete/5



        [Authorize]
        [ValidationFunction("/DanhMuc/BuuCucIndex", ActionName.DELETEBUUCUC)]
        public ActionResult BuuCucDelete(int id)
        {
            try
            {
                var db = DB.Entities;
                var obj = db.BuuCuc.FirstOrDefault(m => m.ID == id);
                db.DeleteObject(obj);
                db.SaveChanges();
                return RedirectToAction("BuuCucIndex");
            }
            catch
            {
                return View();
            }
        }
        #endregion

    }
}
