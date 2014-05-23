using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VPSS.Data;
using System.IO;
using System.Data;

namespace Alabama.Controllers
{
    public class CreatedCardController : Controller
    {
        //
        // GET: /CreatedCard/

        public ActionResult Index()
        {
            return View(new VPSS1Entities().CreatedCard.OrderByDescending(m => m.ID).ToList());
        }

        //
        // GET: /CreatedCard/Details/5

        public ActionResult Details(int id)
        {
            return View(new VPSS1Entities().CreatedCard.FirstOrDefault(m => m.ID == id));
        }

        //
        // GET: /CreatedCard/Create

        public ActionResult Create()
        {
            string s = "";
            foreach (var item in new VPSS1Entities().PriceLevel)
            {
                s += "<option value='" + item.Amount + "'>" + item.Amount.ToString("0,0") + "</option>";
            }
            ViewBag.PriceLevel = s;
            ViewBag.Error = "";
            return View();
        }

        //
        // POST: /CreatedCard/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                if (string.IsNullOrEmpty(collection["txtNote"]))
                    throw new Exception("Phải nhập mô tả mỗi lần tạo danh sách thẻ");

                int amount = int.Parse(collection["cboAmount"]);
                int priceLevel = int.Parse(collection["cboPrice"]);
                new SmashCardDAL().CreateCard(2, amount,
                   priceLevel, DateTime.Parse(collection["dtRelease"]), "txtSecret.Text", collection["txtNote"]);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                //return RedirectToAction("Create");
                string s = "";
                foreach (var item in new VPSS1Entities().PriceLevel)
                {
                    s += "<option value='" + item.Amount + "'>" + item.Amount.ToString("0,0") + "</option>";
                }
                ViewBag.PriceLevel = s;
                return View();
            }
        }

        //
        // GET: /CreatedCard/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /CreatedCard/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public string Download(int id)
        {
            var x = from p in DB.Entities.Card
                    where p.CreatedCardID == id
                    select p;
            DataTable dt = new DataTable("CardTable");
            dt.Columns.Add("No");
            dt.Columns.Add("CardChar");
            dt.Columns.Add("Serial");
            int i = 0;
            foreach (var item in x)
            {
                i++;
                DataRow dr = dt.NewRow();
                dr["No"] = i;
                dr["Serial"] = item.Serial;
                dr["CardChar"] = item.CardChar;
                dt.Rows.Add(dr);
            }

            MemoryStream stream = new MemoryStream();
            //StreamWriter writer = new StreamWriter(stream);
            dt.WriteXml(stream);

            long bytestoread = stream.Length;
            Response.ContentType = "application/octet-stream";

            Response.AddHeader("content-disposition", "attachment; filename=Viegrid_Cards.xml");
            stream.Position = 0;
            while (bytestoread > 0)
            {
                if (Response.IsClientConnected)
                {
                    byte[] buffer = new byte[10000];
                    int length = stream.Read(buffer, 0, 10000);
                    Response.OutputStream.Write(buffer, 0, length);
                    Response.Flush();
                    bytestoread = bytestoread - length;
                }
                else
                {
                    bytestoread = -1;
                }
            }
            return null;
        }

        public ActionResult Release(int id)
        {
            try
            {
                var db = new VPSS1Entities();
                string s = "";
                foreach (var item in db.User)
                {
                    s += "<option value='" + item.ID + "'>" + item.UserName + "</option>";
                }
                ViewBag.Agent = s;

                string actioncode = Common.CardAction.Release.ToString();
                int statusID = db.CardAction.FirstOrDefault(m => m.Code == actioncode).BeforeStatusID.Value;
                int sumCard = db.Card.Count(m => m.CreatedCardID == id && m.CardStatus.ID == statusID);
                if (sumCard == 0)
                {
                    throw new Exception("Không còn thẻ để bàn giao.</br></br>");
                }
                Card card = db.Card.FirstOrDefault(m => m.CreatedCardID == id && m.CardStatus.ID == statusID);

                ViewBag.AmountLeft = sumCard;
                ViewBag.Amount = sumCard;
                ViewBag.PriceLevel = card.CreatedCard.PriceLevel;
                ViewBag.FirstSerial = card.Serial;
                ViewBag.ReleaseTime = DateTime.Now.ToString("dd/MM/yyyy");
                return RedirectToAction("Index", "HandOver");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorDetails", "Home", new { message = ex.ToString() });
            }
        }

        [HttpPost]
        public ActionResult Release(FormCollection collection)
        {
            int amount = int.Parse(collection["txtAmount"]);
            int sumCard = int.Parse(collection["txtAmountLeft"]);
            string serial = collection["txtFirstSerial"];
            int agentID = int.Parse(collection["cboAgent"]);
            if (amount < 1 || amount > sumCard)
                throw new Exception("Dữ liệu không hợp lệ");

            new CardDAL().MultiCardAction(Common.CardAction.Release.ToString(), serial, amount, agentID, collection["txtNote"], DateTime.Parse(collection["dtReleaseTime"]));
            
            return View();
        }
        //
        // GET: /CreatedCard/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /CreatedCard/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
