using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;
using System.Globalization;
using Microsoft.Reporting.WebForms;
using System.Data;

namespace Alabama.Controllers
{
    [Authorize]
    public class TongHopLichCongTacController : Controller
    {
        int pageSize = 20;
        //
        // GET: /Owner/

        [Authorize]
        public ActionResult Index(int? page)
        {
            ViewBag.STT = page.HasValue ? page.Value * pageSize - pageSize : 1;
            var list = DB.Entities.TieuDe.OrderByDescending(m => m.ID).ToPagedList(page.HasValue ? page.Value : 1, pageSize);
            return View(list);
        }

        [HttpGet]

        [Authorize]
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
            ViewBag.CodeDate = codedate;
            var db = DB.Entities;
            if (db.TongHop.FirstOrDefault(m => m.Code == codedate) == null)
            {
                var tieude = new TieuDe() { Title = "LỊCH CÔNG TÁC TUẦN TỪ " + string.Format("{0:d/M}", start) + " ĐẾN " + string.Format("{0:d/M/yyyy}", end) };
                db.TieuDe.AddObject(tieude);
                db.SaveChanges();
                var listJob = db.JobRegister.Where(m => m.DateFrom >= start && m.DateFrom <= end).ToList();
                for (int i = 0; i < 7; i++)
                {
                    var tonghop = new TongHop() { FromDate = start, DayOfWeek = i, Code = codedate, TieuDeID = tieude.ID };
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
                        tonghopdetail.Location = item.Location;
                        tonghopdetail.Code = codedate;
                        db.TongHopDetail.AddObject(tonghopdetail);

                        item.Added = true;
                        db.ObjectStateManager.ChangeObjectState(item, System.Data.EntityState.Modified);
                    }
                }

                db.SaveChanges();

                ViewBag.ListTongHop = db.TongHop.Where(m => m.Code == codedate).ToList();
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
                            tonghopdetail.Location = job.Location;
                            tonghopdetail.Code = codedate;
                            db.TongHopDetail.AddObject(tonghopdetail);

                            job.Added = true;
                            db.ObjectStateManager.ChangeObjectState(job, System.Data.EntityState.Modified);
                        }
                    }
                    db.SaveChanges();

                }
                ViewBag.ListTongHop = db.TongHop.Where(m => m.Code == codedate).ToList();
                var listDetail = db.TongHopDetail.Where(m => m.Code == codedate).ToList();
                ViewBag.IsEdit = true;
                return View(listDetail);
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult EditDetail(int id, string url)
        {
            ViewBag.Url = url;
            return PartialView("_EditDetail", DB.Entities.TongHopDetail.FirstOrDefault(m => m.ID == id));
        }

        [Authorize]
        public ActionResult EditDetail(TongHopDetail model, string url)
        {
            var db = DB.Entities;
            // Add new      
            db.AttachTo("TongHopDetail", model);
            db.ObjectStateManager.ChangeObjectState(model, System.Data.EntityState.Modified);
            db.SaveChanges();
            return Redirect(url);
        }

        [HttpGet]
        [Authorize]
        public ActionResult EditTieuDe(int id, string url)
        {
            ViewBag.Url = url;
            return PartialView("_EditTieuDe", DB.Entities.TieuDe.FirstOrDefault(m => m.ID == id));
        }
        [Authorize]
        public ActionResult EditTieuDe(TieuDe model, string url)
        {
            var db = DB.Entities;
            // Add new      
            db.AttachTo("TieuDe", model);
            db.ObjectStateManager.ChangeObjectState(model, System.Data.EntityState.Modified);
            db.SaveChanges();
            return Redirect(url);
        }


        [Authorize]
        public FileContentResult DisplayReport(string codedate)
        {
            if (string.IsNullOrEmpty(codedate))
            {
                return null;
            }
            var listTongHop = DB.Entities.TongHop.Where(m => m.Code == codedate).ToList();            
            DataTable dt = new BaoCao().LichCongTac;
            foreach (var item in listTongHop)
            {
                if (item.TongHopDetail.Count > 0)
                {
                    foreach (var detail in item.TongHopDetail)
                    {
                        DataRow dr = dt.NewRow();
                        dr["Thu"] = GetThuByDayOfWeek(item.DayOfWeek,item.FromDate);
                        dr["ThoiGian"] = detail.Time.ToString("hh'h'mm");
                        dr["DiaDiem"] = detail.Location;
                        dr["NoiDung"] = "- "+detail.NoiDung;
                        dr["NguoiThucHien"] = "- " + detail.NguoiThucHien;
                        dr["TrucLanhDao"] = item.NguoiTrucID.HasValue ? item.NguoiTruc.Title : "";
                        dt.Rows.Add(dr);
                    }
                }
                else
                {
                    DataRow dr = dt.NewRow();
                    dr["Thu"] = GetThuByDayOfWeek(item.DayOfWeek, item.FromDate);
                    dr["ThoiGian"] = "";
                    dr["DiaDiem"] = "";
                    dr["NoiDung"] = "";
                    dr["NguoiThucHien"] = "";
                    dr["TrucLanhDao"] = "";
                    dt.Rows.Add(dr);
                }
            }

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Views/TongHopLichCongTac/BaoCao.rdlc");
            string title = listTongHop.Count>0?(listTongHop.First().TieuDe.Title):"";
            ReportParameter param0 = new ReportParameter("Title", title);
            ReportDataSource reportDataSource = new ReportDataSource();
            localReport.SetParameters(param0);
            reportDataSource.Name = "DataSet1";
            reportDataSource.Value = dt;
            localReport.DataSources.Add(reportDataSource);
            Byte[] mybytes = localReport.Render("pdf");
            return File(mybytes, "pdf");
        }

        public string GetThuByDayOfWeek(int dayofweek, DateTime fromdate)
        {
            string thu = "";
            switch (dayofweek)
            {
                case 0:
                    thu = "Thứ 2\n"+fromdate.ToString("dd/MM");
                    break;
                case 1:
                    thu = "Thứ 3\n" + fromdate.AddDays(1).ToString("dd/MM");
                    break;
                case 2:
                    thu = "Thứ 4\n" + fromdate.AddDays(2).ToString("dd/MM");
                    break;
                case 3:
                    thu = "Thứ 5\n" + fromdate.AddDays(3).ToString("dd/MM");
                    break;
                case 4:
                    thu = "Thứ 6\n" + fromdate.AddDays(4).ToString("dd/MM");
                    break;
                case 5:
                    thu = "Thứ 7\n" + fromdate.AddDays(5).ToString("dd/MM");
                    break;
                case 6:
                    thu = "CN\n" + fromdate.AddDays(6).ToString("dd/MM");
                    break;
            }
            return thu;
        }

        public bool ChangeNguoiTruc(int id, int idNguoiTruc)
        {
            var db = DB.Entities;
            var tongHop = db.TongHop.FirstOrDefault(m => m.ID == id);
            if (tongHop != null)
            {
                if (db.NguoiTruc.FirstOrDefault(m => m.ID == idNguoiTruc) != null)
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

        public class LichCongTac
        {
            public string Thu { get; set; }
            public string ThoiGian { get; set; }
            public string NoiDung { get; set; }
            public string NguoiThucHien { get; set; }
            public string DiaDiem { get; set; }
            public string TrucLanhDao { get; set; }
        }

    }
}
