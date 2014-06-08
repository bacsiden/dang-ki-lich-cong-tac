using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Alabama.Models
{

    #region Models

    public class ChangePasswordModel : PassWordModels
    {
        [Required(ErrorMessage = "Bạn phải nhập vào mật khẩu cũ")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu hiện tại")]
        public string OldPassword { get; set; }
    }

    public class LogOnModel
    {
        [Required(ErrorMessage="Nhập tên tài khoản")]
        [Display(Name = "Tên tài khoản")]
        public string UserName { get; set; }

        [Required(ErrorMessage="Nhập mật khẩu")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }


    public class RegisterModel
    {
        [Required(ErrorMessage = "Nhập tên tài khoản")]
        [Display(Name = "Tên tài khoản")]
        public string UserName { get; set; }


        [Required(ErrorMessage = "Nhập họ tên")]
        [Display(Name = "Họ và tên")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Nhập email")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Địa chỉ email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Nhập mật khẩu")]
        [ValidatePasswordLength]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        [Compare("Password", ErrorMessage = "Xác nhận mật khẩu không khớp.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        [Display(Name = "Số điện thoại")]
        public string Phone { get; set; }
    }
    #endregion

    #region Services
    // The FormsAuthentication type is sealed and contains static members, so it is difficult to
    // unit test code that calls its members. The interface and helper class below demonstrate
    // how to create an abstract wrapper around such a type in order to make the AccountController
    // code unit testable.

    public interface IMembershipService
    {
        int MinPasswordLength { get; }

        bool ValidateUser(string userName, string password);
        MembershipCreateStatus CreateUser(string userName, string password, string email);
        bool ChangePassword(string userName, string oldPassword, string newPassword);
    }

    public class AccountMembershipService : IMembershipService
    {
        private readonly MembershipProvider _provider;

        public AccountMembershipService()
            : this(null)
        {
        }

        public AccountMembershipService(MembershipProvider provider)
        {
            _provider = provider ?? Membership.Provider;
        }

        public int MinPasswordLength
        {
            get
            {
                return _provider.MinRequiredPasswordLength;
            }
        }

        public bool ValidateUser(string userName, string password)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Giá trị không được để trống.", "userName");
            if (String.IsNullOrEmpty(password)) throw new ArgumentException("Giá trị không được để trống.", "password");

            return _provider.ValidateUser(userName, password);
        }

        public MembershipCreateStatus CreateUser(string userName, string password, string email)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Giá trị không được để trống.", "userName");
            if (String.IsNullOrEmpty(password)) throw new ArgumentException("Giá trị không được để trống.", "password");
            if (String.IsNullOrEmpty(email)) throw new ArgumentException("Giá trị không được để trống.", "email");

            MembershipCreateStatus status;
            _provider.CreateUser(userName, password, email, null, null, true, null, out status);
            return status;
        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Giá trị không được để trống.", "userName");
            if (String.IsNullOrEmpty(oldPassword)) throw new ArgumentException("Giá trị không được để trống.", "oldPassword");
            if (String.IsNullOrEmpty(newPassword)) throw new ArgumentException("Giá trị không được để trống.", "newPassword");

            // The underlying ChangePassword() will throw an exception rather
            // than return false in certain failure scenarios.
            try
            {
                MembershipUser currentUser = _provider.GetUser(userName, true /* userIsOnline */);
                return currentUser.ChangePassword(oldPassword, newPassword);
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (MembershipPasswordException)
            {
                return false;
            }
        }
    }

    public interface IFormsAuthenticationService
    {
        void SignIn(string userName, bool createPersistentCookie);
        void SignOut();
    }

    public class FormsAuthenticationService : IFormsAuthenticationService
    {
        public void SignIn(string userName, bool createPersistentCookie)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Giá trị không được để trống.", "userName");

            FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
        }
        
        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }
    }
    #endregion

    #region Validation
    public static class AccountValidation
    {
        public static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Tên tài khoản đã tồn tại.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "Email đã tồn tại.";

                case MembershipCreateStatus.InvalidPassword:
                    return "Mật khẩu không hợp lệ.";

                case MembershipCreateStatus.InvalidEmail:
                    return "Email không hợp lệ.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ValidatePasswordLengthAttribute : ValidationAttribute, IClientValidatable
    {
        private const string _defaultErrorMessage = "'{0}' ít nhất phải dài {1} kí tự.";
        private readonly int _minCharacters = Membership.Provider.MinRequiredPasswordLength;

        public ValidatePasswordLengthAttribute()
            : base(_defaultErrorMessage)
        {
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture, ErrorMessageString,
                name, _minCharacters);
        }

        public override bool IsValid(object value)
        {
            string valueAsString = value as string;
            return (valueAsString != null && valueAsString.Length >= _minCharacters);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            return new[]{
                new ModelClientValidationStringLengthRule(FormatErrorMessage(metadata.GetDisplayName()), _minCharacters, int.MaxValue)
            };
        }
    }
    #endregion

    public class Account
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public string Pass { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int SexID { get; set; }
        public DateTime Birthday { get; set; }
        public string Address { get; set; }
        public long Balance { get; set; }
        public long VBalance { get; set; }
        public int Point { get; set; }
        public int FailCard { get; set; }
        public int GroupRole { get; set; }
        public int Status { get; set; }

        public Account()
        {
            UserName = "";
            Pass = new VPSS.Server.Secure().GetMD5String(false, "123456");
            Name = "khanh";
            Email = "";
            SexID = 1;
            Birthday = DateTime.Now;
            Address = "";
            Balance = 0;
            VBalance = 0;
            Point = 0;
            FailCard = 0;
            GroupRole = 4;
            Status = 1;
        }
        public Account(string userName)
        {
            UserName = userName;
            Pass = new VPSS.Server.Secure().GetMD5String(false, "123456");
            Name = "khanh";
            Email = "";
            SexID = 1;
            Birthday = DateTime.Now;
            Address = "";
            Balance = 0;
            VBalance = 0;
            Point = 0;
            FailCard = 0;
            GroupRole = 4;
            Status = 1;
        }

        public Account(System.Data.DataTable dt)
        {
            ID = (int)dt.Rows[0]["ID"]; ;
            UserName = dt.Rows[0]["UserName"].ToString();
            Pass = dt.Rows[0]["Pass"].ToString();
            Name = dt.Rows[0]["Name"].ToString();
            Email = dt.Rows[0]["Email"].ToString();
            SexID = (int)dt.Rows[0]["SexID"];
            FailCard = (int)dt.Rows[0]["FailCard"];
            Balance = (long)dt.Rows[0]["Balance"];
            VBalance = (long)dt.Rows[0]["VBalance"];
            Point = (int)dt.Rows[0]["Point"];
            Address = dt.Rows[0]["Address"].ToString();
            try
            {
                Birthday = (DateTime)dt.Rows[0]["Birthday"];
            }
            catch { Birthday = Birthday.AddYears(2000); }
            GroupRole = (int)dt.Rows[0]["GroupRole"];
            Status = (int)dt.Rows[0]["Status"];
        }
    }
}
