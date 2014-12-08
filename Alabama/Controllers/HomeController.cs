using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using System.Globalization;
using System.Data;
using System.Data.SqlClient;

namespace Alabama.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        [Authorize]
        public ActionResult Index()
        {
            ViewBag.Message = "CHÀO MỪNG ĐẾN VỚI PHẦN MỀM ĐĂNG KÝ LỊCH CÔNG TÁC";
            return View();
        }

        public static string temp1 = "<li class='{3}'><a href=\"{1}\" class='menu-item-a {5}'>{2}<span>&nbsp;{0}</span>{4} </a>{6}</li>";
        public static string temp2 = "<ul class=\"submenu\">{0}</ul>";
        public static string BuildMenu()
        {
            var user = new UserDAL().GetCurrentUser;
            string s = "";
            string cls = "dropdown-toggle";
            string icondrop = "<b class=\"arrow icon-angle-right\"></b>";
            var lst = user.UserName==UserDAL.ADMIN? DB.Entities.Menu.OrderBy(m=>m.Oder):
                DB.Entities.Menu.Where(x=>x.IsActive.Value).OrderBy(m=>m.Oder);
            foreach (var item in lst)
            {
                if (item.ParentID == null || item.ParentID.Value == 0)
                {
                    string tmp = "";
                    if (item.IsActive != true || !(new HomeController().IsMenuInGroup(item.ID)))
                    {
                        tmp = "hiddenField";
                    }
                    var listChild = lst.Where(m => m.ParentID == item.ID).OrderBy(m=>m.Oder).ToList();
                    if (listChild.Count > 0)
                    {
                        string subLI = "";
                        foreach (var itemSub in listChild)
                        {
                            subLI += string.Format("<li><a href=\"{1}\" class='menu-item-a'>{0}</a></li>", itemSub.Title, itemSub.Url);
                        }
                        string subMenu = string.Format(temp2,subLI);
                        s += string.Format(temp1, item.Title,"#", item.Icon, tmp, icondrop, cls, subMenu);
                    }
                    else
                    {
                        s += string.Format(temp1, item.Title, item.Url,"", tmp, "", "", "");
                    }

                }
            }
            return s;
        }
        public bool IsMenuInGroup(int id)
        {
            var menuIDList = new List<int>();
            if (System.Web.HttpContext.Current.Session["ListMenuID"] != null)
            {
                menuIDList = (List<int>)System.Web.HttpContext.Current.Session["ListMenuID"];
            }
            else
            {
                menuIDList = new UserDAL().GetMenuIDByUsername(System.Web.HttpContext.Current.User.Identity.Name);
            }

            if (menuIDList != null)
            {
                if (menuIDList.Contains(id))
                {
                    return true;
                }
            }
            return false;
        }

        [Authorize]
        public ActionResult ErrorDetails()
        {
            ViewBag.Message = Request.QueryString["message"];
            return View();
        }
        [Authorize]
        public ActionResult SystemConfig()
        {
            return View(DB.Entities.Config);
        }
        [Authorize]
        public ActionResult Edit(int id)
        {
            var db = DB.Entities.Config;
            var obj = db.FirstOrDefault(m => m.ID == id);
            return View(obj);
        }
        [Authorize]
        public ActionResult DoEdit(Config model)
        {
            var db = DB.Entities;
            var obj = db.Config.FirstOrDefault(m => m.ID == model.ID);
            obj.Title = model.Title;
            obj.Multiline = model.Multiline;
            db.ObjectStateManager.ChangeObjectState(obj, System.Data.EntityState.Modified);
            db.SaveChanges();
            return View("SystemConfig", db.Config);
        }
        [Authorize]
        public ActionResult Create()
        {
            return View(new Config());
        }
        [Authorize]
        public ActionResult Create(Config model)
        {
            var db = DB.Entities;
            db.Config.AddObject(model);
            db.SaveChanges();
            return View("SystemConfig");
        }


    }
}
