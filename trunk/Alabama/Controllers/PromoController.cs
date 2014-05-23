using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Drawing;
namespace Alabama.Controllers
{
    public class PromoController : Controller
    {
        public ActionResult Index()
        {
            return View(DB.Entities.CauHoiDieuTra);
        }

        [HttpGet]
        public ActionResult Details(int id = 0)
        {
            try
            {
                var db = DB.Entities;
                CauHoiDieuTra obj = null;
                if (id == 0)
                    obj = db.CauHoiDieuTra.Where(m => m.Activated).FirstOrDefault();
                else
                    obj = db.CauHoiDieuTra.FirstOrDefault(m => m.ID == id);

                if (obj == null) return View();
                ViewBag.Content = obj.Content;
                ViewBag.ImageUrl = "/PromoData/" + obj.ID + obj.FileExtention;
                string fileName = Server.MapPath(Constant.PromoData).Replace("Promo\\Details\\", null) + "\\" + obj.ID + obj.FileExtention;
                Image image = Image.FromFile(fileName);
                ViewBag.Width = image.Width;
                ViewBag.Height = image.Height;
                ViewBag.Url = obj.Link;
                ViewBag.Url = obj.Link;

                return View(db.TraLoiCauHoiDieuTra.Where(m => m.CauHoiDieuTraID == obj.ID));
            }
            catch
            {
                return View(new List<TraLoiCauHoiDieuTra>());
            }
        }
        public ActionResult Details(FormCollection frm)
        {
            try
            {
                var db = DB.Entities;
                int id = int.Parse(frm["Id"]);
                int idAns = int.Parse(frm["Rbt"]);
                var ans = db.TraLoiCauHoiDieuTra.FirstOrDefault(m => m.ID == idAns);
                ans.SoLanChon += 1;
                //db.TraLoiCauHoiDieuTra.Attach(ans);
                db.SaveChanges();

                return RedirectToAction("Thanks", new { id = id });
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorDetails", "Home", new { message = ex.ToString() });
            }
        }
        public ActionResult Thanks(int id = 0)
        {
            try
            {
                var db = DB.Entities;
                CauHoiDieuTra obj = null;
                if (id == 0)
                    obj = db.CauHoiDieuTra.Where(m => m.Activated).FirstOrDefault();
                else
                    obj = db.CauHoiDieuTra.FirstOrDefault(m => m.ID == id);

                if (obj == null) return View();
                ViewBag.Content = obj.Content;
                ViewBag.ImageUrl = "/PromoData/" + obj.ID + obj.FileExtention;
                string fileName = Server.MapPath(Constant.PromoData).Replace("Promo\\Thanks\\", null) + "\\" + obj.ID + obj.FileExtention;
                Image image = Image.FromFile(fileName);
                ViewBag.Width = image.Width;
                ViewBag.Height = image.Height;
                ViewBag.Url = obj.Link;
                ViewBag.Thanks = "Cảm ơn bạn đã bình chọn!";
                return View();
            }
            catch
            {
                return View();
            }
        }
        //[HttpPost]
        //public ActionResult Details(FormCollection frm)
        //{
        //    ViewBag.Thanks = "Câu trả lời của bạn là: " + frm["Rbt"] + "Cảm ơn bạn đã bình chọn";
        //    try
        //    {
        //        var db = DB.Entities;
        //        CauHoiDieuTra obj = null;
        //        int id = int.Parse(frm["Id"]);
        //        if (string.IsNullOrEmpty(Request.QueryString["trueID"]))
        //            obj = db.CauHoiDieuTra.Where(m => m.Activated).OrderBy(m => m.ID).Skip(id).Take(1).First();
        //        else
        //        {
        //            int trueID = int.Parse(Request.QueryString["trueID"]);
        //            obj = db.CauHoiDieuTra.FirstOrDefault(m => m.ID == trueID);
        //        }
        //        if (obj != null)
        //            ViewBag.Content = obj.Content;
        //        ViewBag.ImageUrl = "/PromoData/" + obj.ID + obj.FileExtention;
        //        ViewBag.Width = 300;
        //        ViewBag.Height = 200;
        //        ViewBag.Url = obj.Link;

        //        return View(db.TraLoiCauHoiDieuTra.Where(m => m.CauHoiDieuTraID == obj.ID));
        //    }
        //    catch
        //    {
        //        return View(new List<TraLoiCauHoiDieuTra>());
        //    }
        //}

        //
        // GET: /Promo/Create

        public ActionResult Create()
        {
            return View();
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Create(FormCollection frm, HttpPostedFileBase file)
        {
            try
            {
                // TODO: Add insert logic here
                CauHoiDieuTra obj = new CauHoiDieuTra();
                obj.Title = frm["txtCompany"];
                obj.Content = frm["txtNoiDung"];
                obj.Company = frm["txtCompany"];
                obj.Link = frm["txtLink"];
                obj.CreatedDate = DateTime.Now;
                obj.FileExtention = ".jpg";
                obj.Activated = false;
                obj.IsMultiChoice = false;
                var db = DB.Entities;
                db.CauHoiDieuTra.AddObject(obj);
                db.SaveChanges();
                string ans = frm["txtAnswer"].Trim();
                if (!string.IsNullOrEmpty(ans))
                {
                    foreach (var item in ans.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        var objAns = new TraLoiCauHoiDieuTra();
                        objAns.CauHoiDieuTraID = obj.ID;
                        objAns.SoLanChon = 0;
                        objAns.NoiDung = item;
                        db.TraLoiCauHoiDieuTra.AddObject(objAns);
                        db.SaveChanges();
                        db.TraLoiCauHoiDieuTra.Detach(objAns);
                    }
                }
                if (file != null)
                {
                    byte[] imageSize = new byte[file.ContentLength];
                    file.InputStream.Read(imageSize, 0, (int)file.ContentLength);
                    FileStream fs = new FileStream(Server.MapPath(Constant.PromoData).Replace("\\Promo\\", "\\") + "\\" + obj.ID + obj.FileExtention, FileMode.Create);
                    fs.Write(imageSize, 0, file.ContentLength);
                    fs.Flush();
                    fs.Dispose();
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorDetails", "Home", new { message = ex.ToString() });
            }
        }

        public ActionResult Edit(int id)
        {
            var db = DB.Entities;
            var obj = db.CauHoiDieuTra.FirstOrDefault(m => m.ID == id);
            return View(obj);
        }

        [HttpPost]
        public ActionResult Edit(CauHoiDieuTra model)
        {
            return View();
        }
        //[HttpPost]
        //public ActionResult Edit(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add update logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        public ActionResult Delete(int id)
        {
            var db = DB.Entities;
            var obj = db.CauHoiDieuTra.FirstOrDefault(m => m.ID == id);
            db.DeleteObject(obj);
            db.SaveChanges();
            string fileName = Server.MapPath(Constant.PromoData).Replace("Promo\\Delete\\", null) + "\\" + obj.ID + obj.FileExtention;
            System.IO.File.Delete(fileName);
            return RedirectToAction("Index");
        }

        public ActionResult Activate(int id, bool activate)
        {
            var db = DB.Entities;
            var obj = db.CauHoiDieuTra.FirstOrDefault(m => m.ID == id);
            obj.Activated = activate;
            db.ObjectStateManager.ChangeObjectState(obj, System.Data.EntityState.Modified);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
