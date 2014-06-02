using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
namespace Alabama.Report
{
    public partial class RegisJob : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ReportViewer1.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("/Report/RegisJob.rdlc");
                ReportViewer1.LocalReport.DataSources.Clear();
                var parameters = new List<ReportParameter>();
                var db = DB.Entities;
                int id = int.Parse(Request.QueryString["id"]);
                JobRegister item = db.JobRegister.FirstOrDefault(m => m.ID == id);
                parameters.Add(new ReportParameter("NoiDung", item.Content));
                parameters.Add(new ReportParameter("TenDonVi", item.User.DonVi1.Title));
                parameters.Add(new ReportParameter("NguoiThucHien", item.NguoiThucHien));
                parameters.Add(new ReportParameter("ThoiGian", string.Format("{0:hh:mm}", item.DateFrom)
                    + " ngày " + string.Format("{0:dd/MM/yyyy}", item.DateFrom)));
                parameters.Add(new ReportParameter("DiaDiem", item.Location.Title));
                int adddays = -((int)item.DateFrom.DayOfWeek + 6) % 7;
                parameters.Add(new ReportParameter("TuanTu", string.Format("{0:dd/MM/yyyy}", item.DateFrom.AddDays(adddays))
                    + " ĐẾN " + string.Format("{0:dd/MM/yyyy}", item.DateFrom.AddDays(6))));

                this.ReportViewer1.LocalReport.SetParameters(parameters);
                ReportViewer1.LocalReport.Refresh();
            }
        }
    }
}