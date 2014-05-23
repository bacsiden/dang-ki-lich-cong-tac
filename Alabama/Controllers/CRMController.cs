using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.IO;

namespace Alabama.Controllers
{
    public class CRMController
    {
        public string GetPackages(string productCode)
        {
            string base64String = new VPSS.Server.Secure().Base64Sign(productCode);
            try
            {
                CRM.PackageInfo[] t = new CRM.PublicServiceSoapClient().GetPackages(base64String, productCode);
                if (t == null)
                    return XMLUtilities.GetXml(new ErrorInfo(3), typeof(ErrorInfo)).OuterXml;

                return XMLUtilities.GetXml(t, typeof(CRM.PackageInfo[])).OuterXml;
            }
            catch
            {
                return XMLUtilities.GetXml(new ErrorInfo(2), typeof(ErrorInfo)).OuterXml;
            }
        }

        public int GetPrice(int packageId)
        {
            string base64Sign = new VPSS.Server.Secure().Base64Sign(packageId);
            return (int)new CRM.PublicServiceSoapClient().GetPackage(base64Sign, packageId).PackagePrice;
        }

        public CRM.KeyPairInfo GetKeyPair(int packageId, string machineCode, string version)
        {
            string base64Sign = new VPSS.Server.Secure().Base64Sign(packageId, machineCode, version);

            return new CRM.PublicServiceSoapClient().GetKeyPair(base64Sign, packageId, machineCode, version);
        }

        public string test1(int packageId, string machineCode, string version)
        {
            string base64Sign = new VPSS.Server.Secure().Base64Sign(packageId, machineCode, version);
            return base64Sign;
            //return new CRM.PublicServiceSoapClient().GetKeyPair(base64Sign, packageId, machineCode, version);
        }

        public string GetActivateKeyByRegKey(int keyPairId, string registKey)
        {
            string base64Sign = new VPSS.Server.Secure().Base64Sign(keyPairId, registKey);

            CRM.KeyPairInfo t = new CRM.PublicServiceSoapClient().GetActivateKeyByRegKey(base64Sign, keyPairId, registKey);
            if (t == null)
                return XMLUtilities.GetXml(new ErrorInfo(3), typeof(ErrorInfo)).OuterXml;

            return XMLUtilities.GetXml(t, typeof(CRM.KeyPairInfo)).OuterXml;
        }

        public string GetActivateKeyByMachineCode(string productCode, string machineCode)
        {
            string base64Sign = new VPSS.Server.Secure().Base64Sign(productCode, machineCode);

            CRM.KeyPairInfo t = new CRM.PublicServiceSoapClient().GetActivateKeyByMachineCode(base64Sign, productCode, machineCode);
            if (t == null)
                return XMLUtilities.GetXml(new ErrorInfo(3), typeof(ErrorInfo)).OuterXml;

            return XMLUtilities.GetXml(t, typeof(CRM.KeyPairInfo)).OuterXml;
        }

        public string SetMachineCode(string serialID, string randCode, string machineCode)
        {
            string base64Sign = new VPSS.Server.Secure().Base64Sign(serialID, randCode, machineCode);

            bool? t = new CRM.PublicServiceSoapClient().SetMachineCode(base64Sign, serialID, randCode, machineCode);
            if (!t.HasValue)
                return XMLUtilities.GetXml(new ErrorInfo(3), typeof(ErrorInfo)).OuterXml;

            return XMLUtilities.GetXml(t, typeof(bool?)).OuterXml;
        }
    }
}