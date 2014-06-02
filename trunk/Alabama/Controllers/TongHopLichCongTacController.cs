using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;
using System.Globalization;

namespace Alabama.Controllers
{
    public class TongHopLichCongTacController : Controller
    {
        int pageSize = 20;
        //
        // GET: /Owner/



        [HttpGet]
        public ActionResult NewOrEdit(string fromDate, string endDate)
        {
            if (fromDate == null)
            {
                return View(new List<TongHopDetail>());
            }
            //DateTime start = DateTime.ParseExact(fromDate + " 12:00:00 AM", "MM/dd/yyyy HH:mm:ss tt", CultureInfo.InvariantCulture);
            string[] arrFrom = fromDate.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            DateTime start = new DateTime(int.Parse(arrFrom[2]), int.Parse(arrFrom[1]), int.Parse(arrFrom[0]), 0, 0, 0);
            DateTime end = start.AddDays(7).AddSeconds(-1);
            string codedate = "" + start.Day + start.Month + start.Year;
            var db = DB.Entities;
            if (db.TongHop.FirstOrDefault(m => m.Code == codedate) == null)
            {
                var listJob = db.JobRegister.Where(m => m.DateFrom >= start && m.DateFrom <= end).ToList();
                for (int i = 0; i < 7; i++)
                {
                    var tonghop = new TongHop() { FromDate = start, DayOfWeek = i, Code = codedate };
                    db.TongHop.AddObject(tonghop);
                    db.SaveChanges();
                    foreach (var item in listJob)
                    {
                        if (((int)item.DateFrom.DayOfWeek + 6) % 7 != i) continue;
                        var tonghopdetail = new TongHopDetail();
                        tonghopdetail.Time = item.DateFrom.TimeOfDay;
                        tonghopdetail.NoiDung = item.Content;
                        tonghopdetail.NguoiThucHien = item.NguoiThucHien;
                        tonghopdetail.TongHopID = tonghop.ID;
                        tonghopdetail.Code = codedate;
                        db.TongHopDetail.AddObject(tonghopdetail);

                        item.Added = true;
                        db.ObjectStateManager.ChangeObjectState(item, System.Data.EntityState.Modified);
                    }
                }

                db.SaveChanges();

                var listDetail = db.TongHopDetail.ToList();
                return View(listDetail);
            }
            else
            {
                var listJob = db.JobRegister.Where(m => !m.Added && m.DateFrom >= start && m.DateFrom <= end).ToList();
                if (listJob.Count > 0)
                {
                    var lsttonghop = db.TongHop.Where(m => m.Code == codedate);
                    foreach (var item in lsttonghop)
                    {
                        foreach (var job in listJob)
                        {
                            if (((int)job.DateFrom.DayOfWeek + 6) % 7 != item.DayOfWeek) continue;
                            var tonghopdetail = new TongHopDetail();
                            tonghopdetail.Time = job.DateFrom.TimeOfDay;
                            tonghopdetail.NoiDung = job.Content;
                            tonghopdetail.NguoiThucHien = job.NguoiThucHien;
                            tonghopdetail.TongHopID = item.ID;
                            tonghopdetail.Code = codedate;
                            db.TongHopDetail.AddObject(tonghopdetail);

                            job.Added = true;
                            db.ObjectStateManager.ChangeObjectState(job, System.Data.EntityState.Modified);
                        }
                    }
                    db.SaveChanges();

                }
                var listDetail = db.TongHopDetail.Where(m => m.Code == codedate).ToList();
                return View(listDetail);
            }
        }

        [HttpGet]
        public ActionResult EditDetail(int id, string url)
        {
            ViewBag.Url = url;
            return PartialView("_EditDetail", DB.Entities.TongHopDetail.FirstOrDefault(m => m.ID == id));
        }

        public ActionResult EditDetail(TongHopDetail model, string url)
        {
            var db = DB.Entities;
            // Add new      
            db.AttachTo("TongHopDetail", model);
            db.ObjectStateManager.ChangeObjectState(model, System.Data.EntityState.Modified);
            db.SaveChanges();
            return Redirect(url);
        }
        public bool ChangeNguoiTruc(int id,int idNguoiTruc)
        {
            var db = DB.Entities;
            var tongHop = db.TongHop.FirstOrDefault(m=>m.ID==id);
            if (tongHop!=null)
            {
                if (db.NguoiTruc.FirstOrDefault(m=>m.ID==idNguoiTruc)!=null)
                {
                    tongHop.NguoiTrucID = idNguoiTruc;
                }
                else
                {
                    tongHop.NguoiTrucID = null;
                }                
                db.ObjectStateManager.ChangeObjectState(tongHop, System.Data.EntityState.Modified);
                db.SaveChanges();
                return true;
            }            
            return false;
        }
    }
}
