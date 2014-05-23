using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Alabama.Controllers;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Alabama.PublicServices
{
    /// <summary>
    /// Summary description for Clients
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Clients : System.Web.Services.WebService
    {
        [WebMethod]
        public string GetFrequency(string md5String)
        {
            return "5000";
        }

        [WebMethod]
        public string GetQuestion(string md5String, string userName)
        {
            try
            {
                var db = DB.Entities;
                int count = db.CauHoiDieuTra.Where(m => m.Activated).Count();
                int index = new Random().Next(count);
                CauHoiDieuTra obj = null;
                if (index == 0) obj = db.CauHoiDieuTra.FirstOrDefault();
                else obj = db.CauHoiDieuTra.Where(m => m.Activated).OrderBy(m => m.ID).Skip(index).Take(1).FirstOrDefault();

                string fileName = Server.MapPath(Constant.PromoData).Replace("\\publicservices", null) + "\\" + obj.ID + obj.FileExtention;
                Image image = Image.FromFile(fileName);

                return HttpContext.Current.Request.Url.ToString().ToLower().Replace("publicservices/clients.asmx", "Promo/Details/" + obj.ID + string.Format("?w={0}&h={1}", image.Width + 20, image.Height + 100));
            }
            catch (Exception ex)
            {
                return "http://viegrid.com"; 
            }
        }

        #region Đăng ký sử dụng sản phẩm

        //Lấy thông tin các gói dịch vụ cho người dùng chọn lựa
        [WebMethod]
        public string GetPackages(string md5String, string productCode)
        {
            //return DataUtilities.strConnection;
            //if (new VPSS.Server.Secure().GetMD5String(false, CommonVariable.clientKey, productCode) != md5String)
            //    return XMLUtilities.GetXml(new ErrorInfo(4), typeof(ErrorInfo)).OuterXml;

            return new CRMController().GetPackages(productCode);
        }

        [WebMethod]
        public string GetKeyPair1()
        {
            CRM.KeyPairInfo k = new CRMController().GetKeyPair(1, "", "");
            if (k == null) return "nu;;";
            return k.ToString();
        }
        //Lấy cặp khóa và trừ tiền. Đăng ký lần đầu
        [WebMethod]
        public string GetKeyPair(string md5String, int packageId, string machineCode, string version, string userName, string pass)
        {
            try
            {
                string md = new VPSS.Server.Secure().GetMD5String(false, CommonVariable.clientKey, packageId, machineCode, version, userName, pass);
                if (md != md5String)
                    return XMLUtilities.GetXml(new ErrorInfo(4), typeof(ErrorInfo)).OuterXml;
                if (new AccountController().Get(userName, pass) == null)
                    return XMLUtilities.GetXml(new ErrorInfo("11", "Sai tên tài khoản và mật khẩu!"), typeof(ErrorInfo)).OuterXml;

                //neu ko du so du tai khoan chinh
                int money = new CRMController().GetPrice(packageId);
                object o = new AccountController().ChangeBalance(userName, 1, money * -1, 0);
                if (o == null)
                    return XMLUtilities.GetXml(new ErrorInfo(11), typeof(ErrorInfo)).OuterXml;
                if (long.Parse(new AccountController().ChangeBalance(userName, 1, money * -1, 0).ToString()) <= -1)
                    return XMLUtilities.GetXml(new ErrorInfo("13", "Not enought balance"), typeof(ErrorInfo)).OuterXml;
                //Da thanh toan, tra ve cap khoa
                CRM.KeyPairInfo k = new CRMController().GetKeyPair(packageId, machineCode, version);
                if (k == null)
                {
                    //Rollback here
                    //if ((long)new AccountController().ChangeBalance(userName, 1, money, 0) <= -1)
                    return XMLUtilities.GetXml(new ErrorInfo(3), typeof(ErrorInfo)).OuterXml;
                }
                return XMLUtilities.GetXml(k, typeof(CRM.KeyPairInfo)).OuterXml;
            }
            catch
            {
                return XMLUtilities.GetXml(new ErrorInfo(2), typeof(ErrorInfo)).OuterXml;
            }
        }

        //Sau khi đã trừ tiền, lấy activate key
        [WebMethod]
        public string GetActivateKeyByRegKey(string md5String, int keyPairId, string registKey)
        {
            if (new VPSS.Server.Secure().GetMD5String(false, CommonVariable.clientKey, keyPairId, registKey) != md5String)
                return XMLUtilities.GetXml(new ErrorInfo(4), typeof(ErrorInfo)).OuterXml;

            return new CRMController().GetActivateKeyByRegKey(keyPairId, registKey);
        }

        //Lấy Activate Key khi đã có, đăng ký lại
        [WebMethod]
        public string GetActivateKeyByMachineCode(string md5String, string productCode, string machineCode)
        {
            if (new VPSS.Server.Secure().GetMD5String(false, CommonVariable.clientKey, productCode, machineCode) != md5String)
                return XMLUtilities.GetXml(new ErrorInfo(4), typeof(ErrorInfo)).OuterXml;

            return new CRMController().GetActivateKeyByMachineCode(productCode, machineCode);
        }

        [WebMethod]
        public string SetMachineCode(string md5String, string serialID, string randCode, string machineCode)
        {
            if (new VPSS.Server.Secure().GetMD5String(false, CommonVariable.clientKey, serialID, randCode, machineCode) != md5String)
                return XMLUtilities.GetXml(new ErrorInfo(4), typeof(ErrorInfo)).OuterXml;
            string t = new CRMController().SetMachineCode(serialID, randCode, machineCode);

            return new CRMController().SetMachineCode(serialID, randCode, machineCode);
        }
        #endregion
    }
}
