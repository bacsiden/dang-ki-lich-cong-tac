using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Alabama.Models
{
    public class PassWordModels
    {
        [Required(ErrorMessage = "Bạn phải nhập vào mật khẩu mới")]
        //[StringLength(100, ErrorMessage = "{0} phải có ít nhất {2} ký tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu mới")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Nhắc lại mật khẩu")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu chưa khớp.")]
        public string ConfirmPassword { get; set; }
    }
    public class ForgotPasswordModel
    {
        [Required(ErrorMessage = "Bạn phải nhập vào tên đăng nhập")]
        [Display(Name = "Mã số")]
        public string UserName { get; set; }
    }

    public class ResetPasswordModel : PassWordModels
    {
        [Required(ErrorMessage = "Bạn phải nhập vào tên đăng nhập")]
        [Display(Name = "Mã số")]
        public string UserName { get; set; }
    }

    public class ConfirmForgotPasswordModel : ResetPasswordModel
    {
        public string Code { get; set; }
    }
}