using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.IO;
using System.Web.Security;
using System.EnterpriseServices;
using Alabama.Controllers;
using System.ComponentModel;

namespace Alabama
{
    public class BaseController : Controller
    {
        //protected override ViewResult View(IView view, object model)
        //{
        //    ViewBag.LanguageCode = CurrentLanguageCode;
        //    ViewBag.LayoutUrl = _currentLayoutURL;
        //    return base.View(view, model);
        //}

        //protected override ViewResult View(string viewName, string masterName, object model)
        //{
        //    ViewBag.LanguageCode = CurrentLanguageCode;
        //    ViewBag.LayoutUrl = _currentLayoutURL;
        //    return base.View(viewName, masterName, model);
        //}

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            ViewBag.IsLogin = CurrentUser != null;

            if (CurrentUser != null)
            {
                #region Lich công tác
                bool isAdmin = CurrentUser.UserName.Equals(UserDAL.ADMIN, StringComparison.OrdinalIgnoreCase);
                ViewBag.SuaXoaLichCongTac = isAdmin || IsFunctionInRole(ActionName.TongHopLaiLichCongTac.ToString());
                ViewBag.TongHopLaiLichCongTac = isAdmin || IsFunctionInRole(ActionName.TongHopLaiLichCongTac.ToString());
                ViewBag.TongHopLichCongTac = isAdmin || IsFunctionInRole(ActionName.TongHopLichCongTac.ToString());
                #endregion
            }
        }
        public enum ActionName
        {
            // Khai báo các quyền tương ứng với bảng Function
            [System.ComponentModel.Description("Đăng nhập")]
            LOGIN,
            [System.ComponentModel.Description("Đăng xuất")]
            LOGOUT,

            #region Quản lý menu
            [System.ComponentModel.Description("Tạo mới menu")]
            ADDNEWMENU,
            [System.ComponentModel.Description("Xem danh sách menu")]
            VIEWLISTMENU,
            [System.ComponentModel.Description("Sửa menu")]
            EDITMENU,
            [System.ComponentModel.Description("Xóa menu")]
            DELETEMENU,
            #endregion

            #region Quản lý user and group
            [System.ComponentModel.Description("Tạo mới GROUP")]
            ADDNEWGROUP,
            [System.ComponentModel.Description("Xem danh sách GROUP")]
            VIEWLISTGROUP,
            [System.ComponentModel.Description("Sửa GROUP")]
            EDITGROUP,
            [System.ComponentModel.Description("Thêm user cho GROUP")]
            ADDUSERFORGROUP,
            [System.ComponentModel.Description("Thêm vai trò cho GROUP")]
            ADDROLESFORGROUP,
            [System.ComponentModel.Description("Thêm menu GROUP")]
            ADDMENUFORGROUP,
            [System.ComponentModel.Description("Xóa GROUP")]
            DELETEGROUP,
            #endregion

            #region Quản lý vai trò
            [System.ComponentModel.Description("Tạo mới ROLE")]
            ADDNEWROLE,
            [System.ComponentModel.Description("Xem danh sách ROLE")]
            VIEWLISTROLE,
            [System.ComponentModel.Description("Sửa ROLE")]
            EDITROLE,
            [System.ComponentModel.Description("Thêm funtion cho ROLE")]
            ADDFUNCTIONFORROLE,
            [System.ComponentModel.Description("Xóa ROLE")]
            DELETEROLE,
            #endregion

            #region Quản lý danh mục Đơn vị, Người trực, Địa điểm
            [System.ComponentModel.Description("Xem danh mục")]
            ViewCategory,
            [System.ComponentModel.Description("Thêm sửa danh mục")]
            NewOrEditCategory,
            [System.ComponentModel.Description("Xóa danh mục")]
            DeleteCategory,
            #endregion

            #region Quản lý tổng hợp lịch công tác
            [System.ComponentModel.Description("Tổng hợp lịch công tác")]
            TongHopLichCongTac,
            [System.ComponentModel.Description("Tổng hợp lại lịch công tác")]
            TongHopLaiLichCongTac,
            [System.ComponentModel.Description("Xóa lịch công tác")]
            XoaLichCongTac,
            #endregion

        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            //Tránh trường hợp sau khi logoff người sử dụng nhấn vào nút Back trên trình duyệt sẽ trở về trang cũ
            Response.AddHeader("pragma", "no-cache");
            Response.AddHeader("cache-control", "private");
            Response.CacheControl = "no-cache";
            Response.Cache.SetExpires(DateTime.Now.AddMinutes(-1));
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
        }
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
        public sealed class ValidationFunctionAttribute : ActionFilterAttribute
        {
            /// <summary>
            /// Quyền được phép truy cập
            /// </summary>
            public List<string> AllowFunctionCodes { get; set; }

            public string UrlRedirect { get; set; }


            public ValidationFunctionAttribute(ActionName allowFunctionCode)
            {
                AllowFunctionCodes = new List<string> { allowFunctionCode.ToString() };
            }


            public ValidationFunctionAttribute(string urlRedirect, ActionName allowFunctionCode)
            {
                UrlRedirect = urlRedirect;
                AllowFunctionCodes = new List<string> { allowFunctionCode.ToString() };
            }
            public ValidationFunctionAttribute(string urlRedirect, params ActionName[] allowFunctionCode)
            {
                UrlRedirect = urlRedirect;
                AllowFunctionCodes = new List<string>();
                foreach (var item in allowFunctionCode)
                {
                    AllowFunctionCodes.Add(item.ToString());
                }
            }

            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                bool isValid = true;
                var user = new UserDAL().GetCurrentUser;
                if (user.Locked)
                {
                    new AccountController().LogOff();
                    filterContext.Result = new RedirectResult("/JobRegister/Index");
                }
                else
                {
                    if (!user.UserName.Equals(UserDAL.ADMIN, StringComparison.OrdinalIgnoreCase))
                    {
                        isValid = false;
                        foreach (var item in AllowFunctionCodes)
                        {
                            if (IsFunctionInRole(item))
                            {
                                isValid = true;
                                break;
                            }
                        }
                    }
                    if (!isValid)
                    {
                        if (String.IsNullOrEmpty(UrlRedirect))
                        {
                            filterContext.Result = new RedirectResult("/JobRegister/index");
                        }
                        else
                        {
                            filterContext.Result = new RedirectResult(UrlRedirect);
                        }
                    }
                }

                base.OnActionExecuting(filterContext);
                return;
            }
            public bool IsFunctionInRole(string code)
            {
                var functionCodeList = new List<string>();
                if (System.Web.HttpContext.Current.Session["ListFunctionCode"] != null)
                {
                    functionCodeList = (List<string>)System.Web.HttpContext.Current.Session["ListFunctionCode"];
                }
                else
                {
                    functionCodeList = new UserDAL().GetListFunctionCodeByUsername(System.Web.HttpContext.Current.User.Identity.Name);
                }

                if (functionCodeList != null)
                {
                    if (functionCodeList.Contains(code))
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        public bool IsFunctionInRole(string code)
        {
            if (ListFunctionCode != null)
            {
                if (ListFunctionCode.Contains(code))
                {
                    return true;
                }
            }
            return false;
        }
        public User CurrentUser
        {
            get
            {
                return new UserDAL().GetCurrentUser;
            }
        }
        public ActionResult ViewHome
        {
            get
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public string GetView(string actionName)
        {
            //string site_language = (string)Request.RequestContext.RouteData.Values["site_language"];
            //if (site_language != (string)Session[Constant.SESSION_CURRENT_SITE_LANGUAGE_CODE])
            //{
            //    if (site_language == null)
            //    {
            //        Session[Constant.SESSION_CURRENT_SITE_CODE] = null;
            //        Session[Constant.SESSION_CURRENT_LANGUAGE_CODE] = Constant.DefaultSiteLanguage;
            //        return actionName;
            //    }
            //    else
            //    {
            //        string s = site_language.ToString().Trim().ToLower();
            //        string s1 = s.Replace("admin", null);
            //        if (s != s1) Session[Constant.SESSION_CURRENT_SITE_CODE] = null;
            //        if (!string.IsNullOrEmpty(s1)) Session[Constant.SESSION_CURRENT_LANGUAGE_CODE] = s1.Replace("-", null);
            //    }
            //}
            //if (Session[Constant.SESSION_CURRENT_SITE_CODE] == null)
            //    return actionName;
            return "Admin/" + actionName;
        }

        public int PageSize
        {
            get
            {
                return int.Parse(System.Configuration.ConfigurationManager.AppSettings["PageSize"]);
            }
        }
        public List<string> ListFunctionCode
        {
            get
            {
                if (Session["ListFunctionCode"] == null)
                {
                    Session["ListFunctionCode"] = new UserDAL().GetListFunctionCodeByUsername(CurrentUser.UserName);
                }
                return (List<string>)Session["ListFunctionCode"];
            }
            set
            {
                Session["ListFunctionCode"] = value;
            }
        }
        public List<int> ListMenuID
        {
            get
            {
                if (Session["ListMenuID"] == null)
                {
                    Session["ListMenuID"] = new UserDAL().GetListFunctionCodeByUsername(CurrentUser.UserName);
                }
                return (List<int>)Session["ListMenuID"];
            }
            set
            {
                Session["ListMenuID"] = value;
            }
        }

        #region Render View|PartialView to String ()
        //Chú ý:
        //1. Các hàm này không thể truyền vào cùng lúc cả Model và ViewData do ViewData đã được sử dụng để chứa Model
        //2. Các hàm này không thể truyền vào ViewBag được

        private void InvalidateControllerContext()
        {
            if (ControllerContext == null)
            {
                ControllerContext context = new ControllerContext(System.Web.HttpContext.Current.Request.RequestContext, this);
                ControllerContext = context;
            }
        }

        //1
        public string RenderPartialToString(string partialViewName)
        {
            InvalidateControllerContext();
            IView view = ViewEngines.Engines.FindPartialView(ControllerContext, partialViewName).View;
            string result = RenderViewToString(view);
            return result;
        }

        //2
        public string RenderPartialToString(string partialViewName, object model)
        {
            InvalidateControllerContext();
            IView view = ViewEngines.Engines.FindPartialView(ControllerContext, partialViewName).View;
            string result = RenderViewToString(view, model);
            return result;
        }

        //3
        public string RenderViewToString(string viewName)
        {
            InvalidateControllerContext();
            IView view = ViewEngines.Engines.FindView(ControllerContext, viewName, null).View;
            string result = RenderViewToString(view);
            return result;
        }

        public string RenderViewToString(string viewName, object model)
        {
            InvalidateControllerContext();
            IView view = ViewEngines.Engines.FindView(ControllerContext, viewName, null).View;
            string result = RenderViewToString(view, model);
            return result;
        }

        public string RenderViewToString(IView view)
        {
            InvalidateControllerContext();
            string result = null;
            if (view != null)
            {
                StringBuilder sb = new StringBuilder();
                using (StringWriter writer = new StringWriter(sb))
                {
                    ViewContext viewContext = new ViewContext(ControllerContext, view, this.ViewData, this.TempData, writer);
                    view.Render(viewContext, writer);
                    writer.Flush();
                }
                result = sb.ToString();
            }
            return result;
        }

        public string RenderViewToString(IView view, object model)
        {
            InvalidateControllerContext();
            string result = null;
            if (view != null)
            {
                StringBuilder sb = new StringBuilder();
                using (StringWriter writer = new StringWriter(sb))
                {
                    ViewContext viewContext = new ViewContext(ControllerContext, view, new ViewDataDictionary(model), this.TempData, writer);
                    view.Render(viewContext, writer);
                    writer.Flush();
                }
                result = sb.ToString();
            }
            return result;
        }
        #endregion Render View|PartialView to String
    }
}