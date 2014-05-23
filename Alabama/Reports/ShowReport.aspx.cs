using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Microsoft.Reporting.WebForms;

namespace DKLCT.Reports
{
    public partial class ShowReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string reportName = Request.QueryString["ReportName"];
                ReportViewer1.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("/Reports/" + reportName + ".rdlc");
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportParameter[] parameters = null;
                DataTable dt = LoadReportData(reportName, out parameters);
                ReportViewer1.LocalReport.DataSources.Add(
                new Microsoft.Reporting.WebForms.ReportDataSource("DataSet1", dt));
                if(parameters!=null)
                    this.ReportViewer1.LocalReport.SetParameters(parameters);

                ReportViewer1.LocalReport.Refresh();
            }
        }

        private DataTable LoadReportData(string reportName, out ReportParameter[] rparam)
        {
            rparam = null;
            switch (reportName)
            {
                case "TripInfoOutstanding":
                    return TripInfoOutstanding();
                case "ScheduleOfInvoices":
                    return ScheduleOfInvoices(out rparam);
                case "DispatcherTrip":
                    return DispatcherTrip();
                case "Invoice":
                    return Invoice();
                default:
                    return null;
            }
        }

        private DataTable TripInfoOutstanding()
        {
            var db = DB.Entities;
            DateTime fromdate = DateTime.Parse(Request.QueryString["startdate"]);
            DateTime todate = DateTime.Parse(Request.QueryString["enddate"]);
            List<int?> lstDriver = db.Trip_Info.Where(m => m.Order_date >= fromdate && m.Order_date <= todate).Select(m => m.Driver).Distinct().ToList();
            var lst = db.Trip_Info.Take(100).ToList();
            DataTable dt = new TripInfoOutstanding().DataTable1;
            foreach (int? driverID in lstDriver)
            {
                if (driverID != null)
                {
                    int totalCharges = 0;
                    List<DataRow> lstDR = new List<DataRow>();
                    foreach (var item in lst.Where(m => m.Driver == driverID))
                    {
                        DataRow dr = dt.NewRow();
                        dr["DriverID"] = driverID;
                        dr["DriverName"] = item.Driver_Info != null ? item.Driver_Info.First_name + item.Driver_Info.First_name : "";
                        dr["OrderDate"] = String.Format("{0:MM/dd/yyyy}", item.Order_date);
                        dr["PickupDate"] = String.Format("{0:MM/dd/yyyy}", item.Pickup_date);
                        dr["DeliveryDate"] = String.Format("{0:MM/dd/yyyy}", item.Delivery_date);
                        dr["CustomerName"] = item.Customer_Info != null ? item.Customer_Info.Customer_Name : "";
                        dr["DeliveryLocation"] = item.Delivery_location;
                        dr["ComfirmedRate1"] = item.Comfirmed_Rate;
                        dr["LumperExtra1"] = "LumperExtra";
                        dr["DetentionPay1"] = item.Detention_pay;
                        dr["ChargesBack1"] = "ChargesBack1";
                        dr["TotalCharges1"] = item.Total_charges;

                        totalCharges += (int)item.Total_charges;
                        lstDR.Add(dr);
                    }
                    foreach (var item in lstDR)
                    {
                        item["TotalCharges"] = totalCharges;
                        dt.Rows.Add(item);
                    }
                }
            }


            return dt;
        }

        private DataTable ScheduleOfInvoices(out ReportParameter[] rparam)
        {
            var db = DB.Entities;
            DateTime fromdate = DateTime.Parse(Request.QueryString["startdate"]);
            DateTime todate = DateTime.Parse(Request.QueryString["enddate"]);
            var lst = db.Trip_Info.Where(m =>m.Picked && m.Deliverd && m.Customer_Invoiced &&
                m.Order_date >= fromdate && m.Order_date <= todate).ToList();
            DataTable dt = new ScheduleOfInvoices().DataTable1;
            long totals = 0;
            foreach (var item in lst)
            {
                DataRow dr = dt.NewRow();
                dr["Name"] = item.Customer_Info != null ? item.Customer_Info.Customer_Name : "";
                dr["Date"] = String.Format("{0:MM/dd/yyyy}", item.Order_date);
                //Lấy 2 kí tự đầu của first name, last name dirver info
                string invoice = item.Invoice + "";
                if (item.Driver_Info != null)
                {
                    string char1 = string.IsNullOrEmpty(item.Driver_Info.First_name) ? null : item.Driver_Info.First_name[0] + "";
                    string char2 = string.IsNullOrEmpty(item.Driver_Info.Last_name) ? null : item.Driver_Info.Last_name[0] + "";
                    invoice = char1 + char2 + invoice;
                }
                dr["Invoice"] = invoice;
                dr["PO"] = item.PO_;
                dr["Amount"] = (int)item.Total_charges + ".00";
                int total = 0;
                if(item.Total_charges.HasValue)
                    total = (int)item.Total_charges;
                totals += total;
                //totalCharges += (int)item.Total_charges;
                dt.Rows.Add(dr);
            }
            rparam = new ReportParameter[3];
            rparam[0] = new ReportParameter("DateOfAssignment", string.Format("{0:MM/dd/yyyy}", DateTime.Now));
            rparam[1] = new ReportParameter("Count", lst.Count.ToString());
            rparam[2] = new ReportParameter("Total", totals.ToString());
            return dt;
        }

        private DataTable DispatcherTrip()
        {
            var db = DB.Entities;
            List<int?> lstDis = db.Trip_Info.Take(100).Select(m => m.Dispatcher).Distinct().ToList();
            var lst = db.Trip_Info.Take(100).ToList();
            DataTable dt = new DispatcherTrip().DataTable1;
            foreach (int? dispatherID in lstDis)
            {
                if (dispatherID != null)
                {
                    int totalCharges = 0;
                    List<DataRow> lstDR = new List<DataRow>();
                    foreach (var item in lst.Where(m => m.Driver == dispatherID))
                    {
                        DataRow dr = dt.NewRow();
                        dr["DriverID"] = dispatherID;
                        dr["DriverName"] = item.Driver_Info != null ? item.Driver_Info.First_name + item.Driver_Info.First_name : "";
                        dr["OrderDate"] = String.Format("{0:MM/dd/yyyy}", item.Order_date);
                        dr["PickupDate"] = String.Format("{0:MM/dd/yyyy}", item.Pickup_date);
                        dr["DeliveryDate"] = String.Format("{0:MM/dd/yyyy}", item.Delivery_date);
                        dr["CustomerName"] = item.Customer_Info != null ? item.Customer_Info.Customer_Name : "";
                        dr["DeliveryLocation"] = item.Delivery_location;
                        dr["ComfirmedRate1"] = item.Comfirmed_Rate;
                        dr["LumperExtra1"] = "LumperExtra";
                        dr["DetentionPay1"] = item.Detention_pay;
                        dr["ChargesBack1"] = "ChargesBack1";
                        dr["TotalCharges1"] = item.Total_charges;

                        totalCharges += (int)item.Total_charges;
                        lstDR.Add(dr);
                    }
                    foreach (var item in lstDR)
                    {
                        item["TotalCharges"] = totalCharges;
                        dt.Rows.Add(item);
                    }
                }
            }


            return dt;
        }

        private DataTable Invoice()
        {
            DataTable dt = new Invoice().DataTable1;
            var db = DB.Entities;
            int id = int.Parse(Request.QueryString["tripID"]);
            var item = db.Trip_Info.FirstOrDefault(m => m.Trip_ID == id);
            DataRow dr = dt.NewRow();
            dr["Date"] = String.Format("{0:MM/dd/yyyy}", item.Order_date);
            dr["CustomerName"] = item.Customer_Info != null ? item.Customer_Info.Customer_Name : "";
            dr["CustomerAddress"] = item.Customer_Info != null ? item.Customer_Info.City + ", " +
                item.Customer_Info.State + " " + item.Customer_Info.ZIP_Code : "";
            dr["Street"] = item.Customer_Info != null ? item.Customer_Info.Street : "";
            //Lấy 2 kí tự đầu của first name, last name dirver info
            string invoice = item.Invoice + "";
            if (item.Driver_Info != null)
            {
                string char1 = string.IsNullOrEmpty(item.Driver_Info.First_name) ? null : item.Driver_Info.First_name[0] + "";
                string char2 = string.IsNullOrEmpty(item.Driver_Info.Last_name) ? null : item.Driver_Info.Last_name[0] + "";
                invoice = char1 + char2 + invoice;
            }
            dr["Invoice"] = invoice;
            dr["Load_"] = item.Loaded_miles;
            dr["PO_"] = item.PO_;
            dr["PickupLocation"] = item.Pick_up_location;
            dr["DeliveryLocation"] = item.Delivery_location;
            dr["ComfirmedRate"] = (int)item.Comfirmed_Rate + ".00";
            dr["Lumber_ExtraCharges"] = (int)item.Extra_charges + ".00";
            dr["DetentionPay"] = (int)item.Detention_pay + ".00";
            dr["TotalCharges"] = (int)item.Total_charges + ".00";
            dr["ExtraStops"] = item.Extra_stops;
            dr["DispatcherName"] = item.Dispatchers != null ? item.Dispatchers.Last_name + " " + item.Dispatchers.Last_name : "";
            dt.Rows.Add(dr);

            return dt;
        }
    }
}