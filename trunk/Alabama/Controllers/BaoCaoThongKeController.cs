using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;

namespace Alabama.Controllers
{
    public class BaoCaoThongKeController : BaseController
    {
        //
        // GET: /BaoCaoThongKe/

        public ActionResult Index()
        {
            var db = DB.Entities;
            ViewBag.NgayBatDau = "01/"+(DateTime.Now.Month<10?"0":"")+DateTime.Now.Month+"/"+DateTime.Now.Year;
            // DropDown Bưu cục
            
            List<BuuCuc> lstBuuCuc = new List<BuuCuc>();
            lstBuuCuc.AddRange(db.BuuCuc);
            var selectListBuuCuc = new SelectList(lstBuuCuc, "ID", "TenBuuCuc");
            ViewBag.SelectListBuuCuc = selectListBuuCuc;

            return View();
        }
        [Authorize]
        //[ValidationFunction(ActionName.EXPORTREGISTER)]
        public ActionResult ThongKe(FormCollection frm)
        {
            var startDate = frm["StartDate"];
            var endDate = frm["EndDate"];
            DateTime StartDate = DateTime.Now;
            DateTime EndDate = DateTime.Now;
            
            var loaiBaoCao = frm["LoaiBaoCao"];
            var model = DB.Entities;
            if (startDate != null && startDate != "")
            {
                if (!startDate.Equals("") && !endDate.Equals(""))
                {
                    var startDate2 = startDate + " 00:00:00";
                    var endDate2 = endDate + " 23:59:59";
                    StartDate = DateTime.ParseExact(startDate2, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    EndDate = DateTime.ParseExact(endDate2, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    ViewBag.startDate = startDate;
                    ViewBag.endDate = endDate;
                }
            }
            else
            {
                //return error here;
            }
            if (loaiBaoCao.Equals("TongHop"))
            {
                var lst = DB.Entities.BuuKien.Where(m => m.NgayNhan >= StartDate && m.NgayNhan <= EndDate);
                return View("BaoCaoTongHop", lst);
            }
            if (loaiBaoCao.Equals("TongHopTheoBC"))//Cac buu kien da nhan theo buu cuc
            {
                var buuCuc = frm["SelectListBuuCuc"];
                if (string.IsNullOrEmpty(buuCuc))
                {
                    //bao loi o day"Phai chon buu cuc"
                }
                int buucucID = int.Parse(buuCuc);
                var lst = DB.Entities.BuuKien.Where(m => m.NgayNhan >= StartDate && m.NgayNhan <= EndDate && m.BuuCucNhanGuiID == buucucID);
                return View("BaoCaoTheoBuuCuc", lst);
            }
            if (loaiBaoCao.Equals("TrangThai"))//Theo ten nguoi nhan
            {
                var trangThai = frm["TrangThai"];
                if (string.IsNullOrEmpty(trangThai))
                {
                    //bao loi o day"Phai chon trang thai"
                }
                var lst = DB.Entities.BuuKien.Where(m => m.NgayNhan >= StartDate && m.NgayNhan <= EndDate && m.TrangThai.Trim().Equals(trangThai.Trim()));
                return View("BaoCaoTheoTrangThai", lst);
            }
            return View("index");
        }
        public ActionResult BaoCaoTongHop(List<BuuKien> model)
        {
            return View(model);
        }
        
        public ActionResult BaoCaoTheoBuuCuc(List<BuuKien> model)
        {
            return View(model);
        }
        public ActionResult BaoCaoTheoTrangThai(List<BuuKien> model)
        {
            return View(model);
        }
        //[Authorize]
        //[ValidationFunction(ActionName.EXPORTREGISTER)]
        //public ActionResult ThongKeReport(FormCollection frm)
        //{
        //    ViewBag.NavBar = new List<string>() { "/ExportAndPDF/ExportRegistered", "Báo cáo", "#", "Danh sách thành viên đã đăng ký" };
        //    try
        //    {
        //        string userType = "Cán bộ, công dân";
        //        string startDate = frm["startDate1"].ToString();
        //        string endDate = frm["endDate1"].ToString();
        //        int type = Convert.ToInt32(frm["GetDSUser1"]);
        //        var db = DB.CreateEntities();
        //        var model = db.V_User.Where(m => (m.UserName != V_UserDAL.ADMIN));
        //        if (startDate != null && startDate != "")
        //        {
        //            if (!startDate.Equals("") && !endDate.Equals(""))
        //            {
        //                var startDate2 = startDate + " 00:00:00";
        //                var endDate2 = endDate + " 23:59:59";
        //                DateTime StartDate = DateTime.ParseExact(startDate2, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        //                DateTime EndDate = DateTime.ParseExact(endDate2, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        //                model = model.Where(m => (m.aspnet_Users.aspnet_Membership.CreateDate.CompareTo(StartDate) >= 0) && (m.aspnet_Users.aspnet_Membership.CreateDate.CompareTo(EndDate) <= 0));
        //                ViewBag.startDate = startDate;
        //                ViewBag.endDate = endDate;
        //            }
        //        }
        //        // Khi người dùng chọn loại người dùng cần hiển thị
        //        if (type == 2)
        //        {
        //            model = model.Where(m => m.OrgID == null);
        //            userType = "Công dân";
        //        }
        //        if (type == 3)
        //        {
        //            model = model.Where(m => m.OrgID != null);
        //            userType = "Cán bộ";
        //        }
        //        model = model.OrderByDescending(m => m.aspnet_Users.aspnet_Membership.CreateDate);

        //        //Xuất dự liệu ra pdf
        //        Warning[] warnings;
        //        string[] streamIds;
        //        string mimeType = string.Empty;
        //        string encoding = string.Empty;
        //        string extension = string.Empty;
        //        string reportPath = "Views/ExportAndPDF/UsserRegistered.rdlc";

        //        // Setup the report viewer object and get the array of bytes
        //        ReportViewer viewer = new ReportViewer();
        //        viewer.ProcessingMode = ProcessingMode.Local;
        //        viewer.LocalReport.ReportPath = reportPath;


        //        //Data
        //        List<VReportParameter> listParameters = new List<VReportParameter>();
        //        listParameters.Add(new VReportParameter("UserType", userType));
        //        listParameters.Add(new VReportParameter("StartDate", frm["startDate1"].ToString()));
        //        listParameters.Add(new VReportParameter("EndDate", frm["endDate1"].ToString()));
        //        listParameters.Add(new VReportParameter("UserCreateReport", CurrentUser.UserName));
        //        DataTable dt = new DataSet2().Tables["UserRegisted"];
        //        foreach (var item in model)
        //        {
        //            // Lấy ra tên vai trò của từng người dùng.
        //            var VaiTro = item.V_UserInRole.ToList();
        //            string DSVaiTro = "";
        //            if (VaiTro != null)
        //            {
        //                foreach (var item1 in VaiTro)
        //                {
        //                    DSVaiTro += item1.V_Role.Name + "\n";
        //                }

        //            }
        //            if (string.IsNullOrEmpty(DSVaiTro))
        //            {
        //                if (item.OrgID == null)
        //                {
        //                    DSVaiTro = "Công dân ";
        //                }
        //                else
        //                {
        //                    DSVaiTro = "Cán bộ";
        //                }
        //            }
        //            string trangThai = item.IsActive ? "Đã chứng thực" : "Chưa chứng thực";
        //            dt.Rows.Add(item.UserName, item.FullName, item.Address, item.PhoneNumber, item.aspnet_Users.aspnet_Membership.CreateDate.ToShortDateString(), DSVaiTro, trangThai);
        //        }

        //        //viewer.Reset();
        //        //viewer.LocalReport.ReportPath = reportPath;

        //        foreach (var parameter in listParameters)
        //        {
        //            viewer.LocalReport.SetParameters(new Microsoft.Reporting.WebForms.ReportParameter(parameter.Name, parameter.Value));
        //        }
        //        ReportDataSource newDS = new ReportDataSource(dt.TableName, dt);
        //        viewer.LocalReport.DataSources.Clear();
        //        viewer.LocalReport.DataSources.Add(newDS);
        //        viewer.LocalReport.Refresh();


        //        byte[] bytes = viewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

        //        string fileName = "Bao_Cao_Thanh_Vien_Da_Dang_ky.pdf";
        //        return File(bytes, mimeType, fileName);
        //    }
        //    catch (Exception ex)
        //    {
        //        SaveErrLog(ex);
        //        return RedirectToAction("Index", "Home");
        //    }
        //}
    }
}
