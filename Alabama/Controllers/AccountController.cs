using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Alabama.Models;
using System.Data.SqlClient;
using System.Data;

namespace Alabama.Controllers
{
    public class UserDAL : DB.BaseClass<User>
    {
        public const string ADMIN = "huyenvv";
        public List<string> GetListFunctionCodeByUsername(string username)
        {
            List<string> listCode = new List<string>();
            try
            {
                //var db = DB.Entities;                
                //var list = (from p in db.V_Function
                //            join
                //            q in db.V_FunctionInRole
                //            on p.ID equals q.FunctionID
                //            where q.V_Role.V_UserInRole.FirstOrDefault(m => m.V_User.UserName == username) != null
                //            select p).ToList();

                //if (list != null && list.Count() > 0)
                //{
                //    foreach (var item in list)
                //    {
                //        listCode.Add(item.Code);
                //    }
                //}
                var db = DB.Entities;
                var list = db.Function.Where(m => m.Role1.FirstOrDefault(n => n.Group.FirstOrDefault(x => x.User.FirstOrDefault(y => y.UserName == username) != null) != null) != null);
                if (list != null && list.Count() > 0)
                {
                    foreach (var item in list)
                    {
                        listCode.Add(item.Code);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return listCode;
        }
        public List<int> GetMenuIDByUsername(string username)
        {
            List<int> listID = new List<int>();
            try
            {
                var db = DB.Entities;
                var list = db.Menu.Where(n => n.Group.FirstOrDefault(x => x.User.FirstOrDefault(y => y.UserName == username) != null) != null).ToList();
                if (list != null && list.Count() > 0)
                {
                    foreach (var item in list)
                    {
                        listID.Add(item.ID);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return listID;
        }
        public User GetUserByUserName(string userName)
        {
            var context = DB.Entities;
            return context.User.FirstOrDefault(m => m.UserName.ToLower() == userName.ToLower());
        }
        public User GetUserByID(int id)
        {
            return GetByID(id);
        }
        public User GetCurrentUser
        {
            get
            {
                MembershipUser mbsUser = Membership.GetUser();
                if (mbsUser != null)
                {
                    Guid id = (Guid)mbsUser.ProviderUserKey;
                    return DB.Entities.User.FirstOrDefault(m => m.AspnetUserID == id);
                }
                return null;
            }
        }
        public void LockUserByID(int id)
        {
            var user = GetUserByID(id);
            user.Locked = true;
            Update(user);
        }
    }
    public class AccountController : BaseController
    {

        public IFormsAuthenticationService FormsService { get; set; }
        public IMembershipService MembershipService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(requestContext);
        }

        // **************************************
        // URL: /Account/LogOn
        // **************************************


        public ActionResult Index(int id = 0)
        {
            try
            {
                if (id == 0)
                {
                    @ViewBag.GroupName = "Danh sách người dùng";
                    return View(DB.Entities.User);
                }
                else
                {
                    var db = DB.Entities;
                    var g = db.Group.First(m => m.ID == id);
                    //@ViewBag.GroupName = "Tên nhóm: <a href='" + HttpContext.Request.Url + "'>" + g.Title + "</a>";
                    @ViewBag.GroupName = g.Title;
                    return View(db.User.Where(m => m.Group.FirstOrDefault(x => x.ID == id) != null));
                }
            }
            catch (Exception ex)
            {
                return View(new List<User>());
            }
        }

        public ActionResult NewOrEdit(int? id = 0)
        {
            var obj = DB.Entities.User.FirstOrDefault(m => m.ID == id);
            ViewBag.Title = "Sửa tài khoản người dùng";
            if (obj == null)
            {
                ViewBag.Title = "Thêm mới tài khoản người dùng";
                obj = new User();
            }
            return View(obj);
        }

        [HttpPost]
        public ActionResult NewOrEdit(User model)
        {
            try
            {
                var db = DB.Entities;
                if (model.ID == 0)
                {
                    // Add new                 
                    db.User.AddObject(model);
                }
                else
                {
                    // Edit    
                    db.AttachTo("User", model);
                    db.ObjectStateManager.ChangeObjectState(model, System.Data.EntityState.Modified);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult DeleteByListID(string arrayID = "")
        {
            try
            {
                // TODO: Add delete logic here
                var lstID = arrayID.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var db = DB.Entities;
                if (lstID.Length > 0)
                {
                    foreach (var item in lstID)
                    {
                        int tmpID = int.Parse(item);
                        var obj = db.User.FirstOrDefault(m => m.ID == tmpID);
                        db.User.DeleteObject(obj);
                    }
                    db.SaveChanges();
                }
            }
            catch
            {

            }
            return RedirectToAction("Index");
        }

        public ActionResult LockByListID(string arrayID = "")
        {
            try
            {
                // TODO: Add delete logic here
                var lstID = arrayID.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var db = DB.Entities;
                var userDAL = new UserDAL();
                if (lstID.Length > 0)
                {
                    foreach (var item in lstID)
                    {
                        // Thực hiện khóa tài khoản người dùng
                        userDAL.LockUserByID(int.Parse(item));
                    }
                }
            }
            catch
            {

            }
            return RedirectToAction("Index");
        }


        public ActionResult LogOn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = new UserDAL().GetUserByUserName(model.UserName);
                if (user!=null && !user.Locked)
                {
                    if (MembershipService.ValidateUser(model.UserName, model.Password))
                    {
                        FormsService.SignIn(model.UserName, model.RememberMe);
                        ListFunctionCode = new UserDAL().GetListFunctionCodeByUsername(model.UserName);
                        if (Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không đúng.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Tài khoản đã bị khóa. Vui lòng liên hệ quản trị để được giải đáp.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // **************************************
        // URL: /Account/LogOff
        // **************************************

        public ActionResult LogOff()
        {
            FormsService.SignOut();

            return RedirectToAction("Index", "Home");
        }

        // **************************************
        // URL: /Account/Register
        // **************************************

        public ActionResult Register()
        {
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            // DropDown Bưu cục
            var db = DB.Entities;
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var db = DB.Entities;
                // Attempt to register the user
                if (db.User.FirstOrDefault(m => m.UserName.Equals(model.UserName)) == null)
                {


                    MembershipUser aspnetUser = Membership.CreateUser(model.UserName, model.Password, model.Email);
                    Guid userCreated = (Guid)aspnetUser.ProviderUserKey;
                    if (userCreated != null)
                    {

                        db.User.AddObject(new User() { UserName = model.UserName, Email = model.Email, PhoneNumber = model.Phone, Address = model.Address, AspnetUserID = userCreated, Name = model.Name });
                        db.SaveChanges();
                        FormsService.SignIn(model.UserName, false /* createPersistentCookie */);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Đăng ký không thành công!");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Tài khoản đã tồn tại!");
                }
            }

            // If we got this far, something failed, redisplay form
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View(model);
        }

        // **************************************
        // URL: /Account/ChangePassword
        // **************************************

        [Authorize]
        public ActionResult ChangePassword()
        {
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View(model);
        }

        // **************************************
        // URL: /Account/ChangePasswordSuccess
        // **************************************

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        public Models.Account Get(int id)
        {
            try
            {
                DataTable dt = DataUtilities.GetTable("Account_GetByID", CommandType.StoredProcedure, "@ID", id);
                return new Models.Account(dt);
            }
            catch
            {
                return null;
            }
        }

        public Models.Account Get(string user, string pass)
        {
            try
            {
                DataTable dt = DataUtilities.GetTable("Account_Select_User_Pass",
                CommandType.StoredProcedure, "@username", user, "@pass", pass);
                return new Models.Account(dt);
            }
            catch
            {
                return null;
            }
        }

        public Models.Account Get(string user)
        {
            try
            {
                DataTable dt = DataUtilities.GetTable("Account_Select_UserName",
                CommandType.StoredProcedure, "@username", user);
                return new Models.Account(dt);
            }
            catch
            {
                return null;
            }
        }

        public DataTable Gets(string name) // Gat table of Account has the similar name
        {
            try
            {
                return DataUtilities.GetTable("Account_Select_Name",
                    CommandType.StoredProcedure, "@username", name);
            }
            catch
            {
                return null;
            }
        }

        public int GetGroup(string user, string pass)
        {
            try
            {
                Models.Account a = Get(user, pass);
                return a.GroupRole;
            }
            catch
            {
                return 10;
            }
        }

        public object ChangeBalance(string userName, int changeType, int money, int transactionID)
        {
            SqlParameter[] pars = new SqlParameter[4];
            pars[0] = new SqlParameter("@username", userName);
            pars[1] = new SqlParameter("@changetypeid", changeType);
            pars[2] = new SqlParameter("@money", money);
            pars[3] = new SqlParameter("@transactionid", transactionID);
            return DataUtilities.ExcuteScalar("Account_ChangeBalance",
                CommandType.StoredProcedure, pars);
        }

        public void Insert(Models.Account ac)
        {
            SqlParameter[] pars = new SqlParameter[13];
            pars[0] = new SqlParameter("@UserName", ac.UserName);
            pars[1] = new SqlParameter("@Pass", ac.Pass);
            pars[2] = new SqlParameter("@Name", ac.Name);
            pars[3] = new SqlParameter("@Email", ac.Email);
            pars[4] = new SqlParameter("@SexID", ac.SexID);
            pars[5] = new SqlParameter("@Birthday", ac.Birthday);
            pars[6] = new SqlParameter("@Address", ac.Address);
            pars[7] = new SqlParameter("@Balance", ac.Balance);
            pars[8] = new SqlParameter("@VBalance", ac.VBalance);
            pars[9] = new SqlParameter("@Point", ac.Point);
            pars[10] = new SqlParameter("@FailCard", ac.FailCard);
            pars[11] = new SqlParameter("@GroupRole", ac.GroupRole);
            pars[12] = new SqlParameter("@Status", ac.Status);

            DataUtilities.ExcuteNonQuery("Account_Insert",
                CommandType.StoredProcedure, pars);
        }

        public void Update(Models.Account ac)
        {
            SqlParameter[] pars = new SqlParameter[14];
            pars[0] = new SqlParameter("@UserName", ac.UserName);
            pars[1] = new SqlParameter("@Pass", ac.Pass);
            pars[2] = new SqlParameter("@Name", ac.Name);
            pars[3] = new SqlParameter("@Email", ac.Email);
            pars[4] = new SqlParameter("@SexID", ac.SexID);
            pars[5] = new SqlParameter("@Birthday", ac.Birthday);
            pars[6] = new SqlParameter("@Address", ac.Address);
            pars[7] = new SqlParameter("@Balance", ac.Balance);
            pars[8] = new SqlParameter("@VBalance", ac.VBalance);
            pars[9] = new SqlParameter("@Point", ac.Point);
            pars[10] = new SqlParameter("@FailCard", ac.FailCard);
            pars[11] = new SqlParameter("@GroupRole", ac.GroupRole);
            pars[12] = new SqlParameter("@Status", ac.Status);
            pars[13] = new SqlParameter("@ID", ac.ID);

            DataUtilities.ExcuteNonQuery("Account_Update",
                CommandType.StoredProcedure, pars);
        }

        public int Del(int id)
        {
            return DataUtilities.ExcuteNonQuery("Account_Delete", CommandType.StoredProcedure, "@ID", id);
        }

    }
}
