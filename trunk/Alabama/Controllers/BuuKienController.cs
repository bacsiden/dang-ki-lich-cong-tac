using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Alabama.Controllers
{
    public class BuuKienController : BaseController
    {
        //
        // GET: /BuuKien/
        [Authorize]
        [ValidationFunction("/Home/index", ActionName.VIEWLISTBUUKIEN)]
        public ActionResult Index()
        {
            var db = DB.Entities;
            if (CurrentUser != null)
            {
                var model = db.BuuKien.Where(m => m.BuuCucInBuuKien.Any(x => x.BuuCucID == CurrentUser.BuuCucID)).ToList();
                return View(model);
            }
            return View();

            //.Include(u => u.Businesses)
            //           .Where(u => u.Businesses.Any(x => x.Id == currentBusinessId))
            //           .ToList();


        }

        [Authorize]
        [ValidationFunction("/BuuKien/index", ActionName.ADDNEWBUUKIEN)]
        public ActionResult Create()
        {
            ViewBag.MaBuuKien = "";
            var db = DB.Entities;
            BuuKien objBuuKien = DB.Entities.BuuKien.FirstOrDefault();

            if (objBuuKien == null)
            {
                ViewBag.MaBuuKien = "BK-001";
            }
            else
            {
                ViewBag.MaBuuKien = "BK-00" + (db.BuuKien.Max(m => m.ID) + 1);
            }

            // Dropdown khách hàng

            var objKhachHang = new KhachHang() { ID = 0, Name = "- None -" };
            List<KhachHang> lst = new List<KhachHang>();
            lst.Add(objKhachHang);
            lst.AddRange(db.KhachHang);
            var selectListKH = new SelectList(lst, "ID", "Name");
            ViewBag.SelectListKhachHang = selectListKH;

            // DropDown loại hàng

            var objLoaiHang = new LoaiHang() { ID = 0, Title = "- None -" };
            List<LoaiHang> lstLoaiHang = new List<LoaiHang>();
            lstLoaiHang.Add(objLoaiHang);
            lstLoaiHang.AddRange(db.LoaiHang);
            var selectListLoaiHang = new SelectList(lstLoaiHang, "ID", "Title");
            ViewBag.SelectListLoaiHang = selectListLoaiHang;

            // DropDown đường gửi
            var objCuocPhi = new CuocPhi() { ID = 0, Title = "- None -" };
            List<CuocPhi> lstCuocPhi = new List<CuocPhi>();
            lstCuocPhi.Add(objCuocPhi);
            lstCuocPhi.AddRange(db.CuocPhi);
            var selectListCuocPhi = new SelectList(lstCuocPhi, "ID", "Title");
            ViewBag.SelectListBuuCuc = selectListCuocPhi;

            // DropDown Dịch vụ c

            var objDichVuCongThem = new DichVuCongThem() { ID = 0, Name = "-----None----" };
            List<DichVuCongThem> lstDichVuCongThem = new List<DichVuCongThem>();
            lstDichVuCongThem.Add(objDichVuCongThem);
            lstDichVuCongThem.AddRange(db.DichVuCongThem);
            var selectListDichVuCongThem = new SelectList(lstDichVuCongThem, "ID", "Name");
            ViewBag.SelectListDichVuCongThem = selectListDichVuCongThem;
            return View(new BuuKien() { NgayGui = DateTime.Now });
        }

        [HttpPost]
        [Authorize]
        [ValidationFunction("/BuuKien/index", ActionName.ADDNEWBUUKIEN)]
        public ActionResult Create(BuuKien model)
        {
            if (model!=null)
            {
                var db = DB.Entities;
                model.NgayNhan = DateTime.Now;
                model.TrangThai = TrangThai.DaTiepNhan;
                model.NguoiNhanID = CurrentUser.ID;
                model.BuuCucNhanGuiID = CurrentUser.BuuCucID;
                model.BuuCucHienTaiID = CurrentUser.BuuCucID;
                
                db.BuuKien.AddObject(model);
                var bbb = new BuuCucInBuuKien();
                bbb.BuuCucID = model.BuuCucNhanGuiID.Value;
                bbb.BuuKienID = model.ID;
                bbb.KieuChuyen = 0;
                db.BuuCucInBuuKien.AddObject(bbb);
                db.SaveChanges();

                return RedirectToAction("Details/" + model.ID);
            }
            else
            {
                ViewBag.MaBuuKien = "";
                var db = DB.Entities;                
                BuuKien objBuuKien = DB.Entities.BuuKien.FirstOrDefault();

                if (objBuuKien == null)
                {
                    ViewBag.MaBuuKien = "BK-001";
                }
                else
                {
                    ViewBag.MaBuuKien = "BK-00" + (db.BuuKien.Max(m => m.ID) + 1);
                }

                // Dropdown khách hàng

                var objKhachHang = new KhachHang() { ID = 0, Name = "- None -" };
                List<KhachHang> lst = new List<KhachHang>();
                lst.Add(objKhachHang);
                lst.AddRange(db.KhachHang);
                var selectListKH = new SelectList(lst, "ID", "Name");
                ViewBag.SelectListKhachHang = selectListKH;

                // DropDown loại hàng

                var objLoaiHang = new LoaiHang() { ID = 0, Title = "- None -" };
                List<LoaiHang> lstLoaiHang = new List<LoaiHang>();
                lstLoaiHang.Add(objLoaiHang);
                lstLoaiHang.AddRange(db.LoaiHang);
                var selectListLoaiHang = new SelectList(lstLoaiHang, "ID", "Title");
                ViewBag.SelectListLoaiHang = selectListLoaiHang;

                // DropDown đường gửi
                var objCuocPhi = new CuocPhi() { ID = 0, Title = "- None -" };
                List<CuocPhi> lstCuocPhi = new List<CuocPhi>();
                lstCuocPhi.Add(objCuocPhi);
                lstCuocPhi.AddRange(db.CuocPhi);
                var selectListCuocPhi = new SelectList(lstCuocPhi, "ID", "Title");
                ViewBag.SelectListBuuCuc = selectListCuocPhi;
                // DropDown Bưu cục

                //var objBuuCuc = new BuuCuc() { ID = 0, TenBuuCuc = "- None -" };
                //List<BuuCuc> lstBuuCuc = new List<BuuCuc>();
                //lstBuuCuc.Add(objBuuCuc);
                //lstBuuCuc.AddRange(db.BuuCuc);
                //var selectListBuuCuc = new SelectList(lstBuuCuc, "ID", "TenBuuCuc");
                //ViewBag.SelectListBuuCuc = selectListBuuCuc;
                var objDichVuCongThem = new DichVuCongThem() { ID = 0, Name = "-----None----" };
                List<DichVuCongThem> lstDichVuCongThem = new List<DichVuCongThem>();
                lstDichVuCongThem.Add(objDichVuCongThem);
                lstDichVuCongThem.AddRange(db.DichVuCongThem);
                var selectListDichVuCongThem = new SelectList(lstDichVuCongThem, "ID", "Name");
                ViewBag.SelectListDichVuCongThem = selectListDichVuCongThem;
                return View();
            }

        }

        [Authorize]
        [ValidationFunction("/BuuKien/index", ActionName.EDITBUUKIEN)]
        public ActionResult Edit(int id)
        {
            var db = DB.Entities;
            BuuKien objBuuKien = DB.Entities.BuuKien.FirstOrDefault(m => m.ID == id);
            ViewBag.MaBuuKien = objBuuKien.MaBuuKien;

            // Dropdown khách hàng

            var objKhachHang = new KhachHang() { ID = 0, Name = "- None -" };
            List<KhachHang> lst = new List<KhachHang>();
            lst.Add(objKhachHang);
            lst.AddRange(db.KhachHang);
            var selectListKH = new SelectList(lst, "ID", "Name", objBuuKien.KhachHang);
            ViewBag.SelectListKhachHang = selectListKH;

            // DropDown loại hàng

            var objLoaiHang = new LoaiHang() { ID = 0, Title = "- None -" };
            List<LoaiHang> lstLoaiHang = new List<LoaiHang>();
            lstLoaiHang.Add(objLoaiHang);
            lstLoaiHang.AddRange(db.LoaiHang);
            var selectListLoaiHang = new SelectList(lstLoaiHang, "ID", "Title", objBuuKien.LoaiHang);
            ViewBag.SelectListLoaiHang = selectListLoaiHang;

            // DropDown đường gửi
            var objCuocPhi = new CuocPhi() { ID = 0, Title = "- None -" };
            List<CuocPhi> lstCuocPhi = new List<CuocPhi>();
            lstCuocPhi.Add(objCuocPhi);
            lstCuocPhi.AddRange(db.CuocPhi);
            var selectListCuocPhi = new SelectList(lstCuocPhi, "ID", "Title", objBuuKien.CuocPhi);
            ViewBag.SelectListBuuCuc = selectListCuocPhi;
            
            // DropDown Dịch vụ cộng thêm
            var objDichVuCongThem = new DichVuCongThem() { ID = 0, Name = "-----None----" };
            List<DichVuCongThem> lstDichVuCongThem = new List<DichVuCongThem>();
            lstDichVuCongThem.Add(objDichVuCongThem);
            lstDichVuCongThem.AddRange(db.DichVuCongThem);
            var selectListDichVuCongThem = new SelectList(lstDichVuCongThem, "ID", "Name");
            ViewBag.SelectListDichVuCongThem = selectListDichVuCongThem;
            // DropDown Bưu cục

            //var objBuuCuc = new BuuCuc() { ID = 0, TenBuuCuc = "-----None----" };
            //List<BuuCuc> lstBuuCuc = new List<BuuCuc>();
            //lstBuuCuc.Add(objBuuCuc);
            //lstBuuCuc.AddRange(db.BuuCuc);
            //var selectListBuuCuc = new SelectList(lstBuuCuc, "ID", "TenBuuCuc");
            //ViewBag.SelectListBuuCuc = selectListBuuCuc;
            return View(objBuuKien);
        }

        [Authorize]
        [HttpPost]
        [ValidationFunction("/BuuKien/index", ActionName.EDITBUUKIEN)]
        public ActionResult Edit(BuuKien model)
        {
            var db = DB.Entities;
            if (ModelState.IsValid)
            {
                var obj = db.BuuKien.FirstOrDefault(m => m.ID == model.ID);
                obj.TenBuuKien = model.TenBuuKien;
                obj.KhachHangID = model.KhachHangID;
                obj.NguoiGui = model.NguoiGui;
                obj.DiaChiNguoiGui = model.DiaChiNguoiGui;
                obj.NguoiNhan = model.NguoiNhan;
                obj.DiaChiNguoiNhan = model.DiaChiNguoiNhan;
                obj.LoaiHangID = model.LoaiHangID;
                obj.CuocPhiID = model.CuocPhiID;
                obj.KhoiLuong = model.KhoiLuong;
                obj.GhiChu = model.GhiChu;
                db.ObjectStateManager.ChangeObjectState(obj, System.Data.EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = model.ID });
            }
            else
            {
                ViewBag.MaBuuKien = model.MaBuuKien;

                // Dropdown khách hàng

                var objKhachHang = new KhachHang() { ID = 0, Name = "- None -" };
                List<KhachHang> lst = new List<KhachHang>();
                lst.Add(objKhachHang);
                lst.AddRange(db.KhachHang);
                var selectListKH = new SelectList(lst, "ID", "Name", model.KhachHang);
                ViewBag.SelectListKhachHang = selectListKH;

                // DropDown loại hàng

                var objLoaiHang = new LoaiHang() { ID = 0, Title = "- None -" };
                List<LoaiHang> lstLoaiHang = new List<LoaiHang>();
                lstLoaiHang.Add(objLoaiHang);
                lstLoaiHang.AddRange(db.LoaiHang);
                var selectListLoaiHang = new SelectList(lstLoaiHang, "ID", "Title", model.LoaiHang);
                ViewBag.SelectListLoaiHang = selectListLoaiHang;

                // DropDown đường gửi
                var objCuocPhi = new CuocPhi() { ID = 0, Title = "- None -" };
                List<CuocPhi> lstCuocPhi = new List<CuocPhi>();
                lstCuocPhi.Add(objCuocPhi);
                lstCuocPhi.AddRange(db.CuocPhi);
                var selectListCuocPhi = new SelectList(lstCuocPhi, "ID", "Title", model.CuocPhi);
                ViewBag.SelectListBuuCuc = selectListCuocPhi;
                
                // Dropdown dịch vụ cộng thêm
                var objDichVuCongThem = new DichVuCongThem() { ID = 0, Name = "-----None----" };
                List<DichVuCongThem> lstDichVuCongThem = new List<DichVuCongThem>();
                lstDichVuCongThem.Add(objDichVuCongThem);
                lstDichVuCongThem.AddRange(db.DichVuCongThem);
                var selectListDichVuCongThem = new SelectList(lstDichVuCongThem, "ID", "Name");
                ViewBag.SelectListDichVuCongThem = selectListDichVuCongThem;
                return View(model);
            }
        }
        [Authorize]
        [ValidationFunction("/BuuKien/index", ActionName.CHUYENBUUKIEN)]
        public ActionResult ChuyenBuuKien(int id)
        {
            var model = DB.Entities.BuuKien.FirstOrDefault(m=>m.ID==id);
            // DropDown Bưu cục
            
            List<BuuCuc> lstBuuCuc = new List<BuuCuc>();
            lstBuuCuc.AddRange(DB.Entities.BuuCuc.Where(m=>m.ID!=model.BuuCucNhanGuiID));
            var selectListBuuCuc = new SelectList(lstBuuCuc, "ID", "TenBuuCuc");
            ViewBag.SelectListBuuCuc = selectListBuuCuc;

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidationFunction("/BuuKien/index", ActionName.CHUYENBUUKIEN)]
        public ActionResult ChuyenBuuKien(int id, int SelectListBuuCuc)
        {
            var db = DB.Entities;
            BuuKien objBuuKien = db.BuuKien.FirstOrDefault(m => m.ID == id);
            objBuuKien.BuuCucTraID = SelectListBuuCuc;
            objBuuKien.BuuCucHienTaiID = SelectListBuuCuc;
            objBuuKien.TrangThai = TrangThai.DangGui;
            objBuuKien.NgayGui = DateTime.Now;
            db.ObjectStateManager.ChangeObjectState(objBuuKien, System.Data.EntityState.Modified);
            db.SaveChanges();

            var bbb = new BuuCucInBuuKien();
            bbb.BuuCucID = objBuuKien.BuuCucNhanGuiID.Value;
            bbb.BuuKienID = objBuuKien.ID;
            bbb.KieuChuyen = 1;
            db.BuuCucInBuuKien.AddObject(bbb);
            return RedirectToAction("Details", new { id = id });
        }


        [Authorize]
        [HttpPost]
        [ValidationFunction("/BuuKien/index", ActionName.TRABUUKIEN)]
        public ActionResult TraBuuKien(int id, int SelectListBuuCuc)
        {
            var db = DB.Entities;
            BuuKien objBuuKien = db.BuuKien.FirstOrDefault(m => m.ID == id);
            objBuuKien.BuuCucTraID = SelectListBuuCuc;
            objBuuKien.BuuCucHienTaiID = SelectListBuuCuc;
            objBuuKien.TrangThai = TrangThai.GuiThanhCong;
            objBuuKien.NgayGui = DateTime.Now;
            db.ObjectStateManager.ChangeObjectState(objBuuKien, System.Data.EntityState.Modified);
            db.SaveChanges();

            var bbb = new BuuCucInBuuKien();
            bbb.BuuCucID = objBuuKien.BuuCucNhanGuiID.Value;
            bbb.BuuKienID = objBuuKien.ID;
            bbb.KieuChuyen = 2;
            db.BuuCucInBuuKien.AddObject(bbb);
            return RedirectToAction("Details", new { id = id });
        }

        [ValidationFunction("/BuuKien/index", ActionName.DETAILS)]
        [Authorize]
        public ActionResult Details(int id)
        {
            var model = DB.Entities.BuuKien.FirstOrDefault(m => m.ID == id);
            ViewBag.CuocPhi = model.KhoiLuong * model.LoaiHang.HeSo * model.CuocPhi.Cost + (model.DichVuCongThem!=null?model.DichVuCongThem.Cost:0);
            
            // DropDown Bưu cục
            
            List<BuuCuc> lstBuuCuc = new List<BuuCuc>();
            lstBuuCuc.AddRange(DB.Entities.BuuCuc);
            var selectListBuuCuc = new SelectList(lstBuuCuc, "ID", "TenBuuCuc");
            ViewBag.SelectListBuuCuc = selectListBuuCuc;

            return View(model);
        }

        [ValidationFunction("/BuuKien/index", ActionName.DETAILS)]
        [Authorize]
        public ActionResult Delete(int id)
        {
            try
            {
                var db = DB.Entities;
                var obj = db.BuuKien.FirstOrDefault(m => m.ID == id);
                db.BuuKien.DeleteObject(obj);
                db.SaveChanges();
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {

                return View();
            }
            
            
        }

        public JsonResult GetKhachHang(int id)
        {
            var obj = DB.Entities.KhachHang.FirstOrDefault(m => m.ID == id);
            if (obj != null)
            {
                return Json(new { name = obj.Name, phone = obj.Phone, address = obj.Address });
            }
            return Json(new { name = "", phone = "", address = "" });
        }

    }
}
