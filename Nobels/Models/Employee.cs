using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nobels.Models
{
    public class Employee:IdentityUser<int>
    {
        public Employee()
        {
            QoutationUpdates = new HashSet<QoutationUpdate>();
        }
        [Display(Name = "المعرف")]
        //[Required(ErrorMessage = "هذا الحقل اجباري")]
        public int EmployeeId { get; set; }
        [Display(Name = "رقم الموظف")]
        [Required(ErrorMessage = "هذا الحقل اجباري")]
        public string? EmployeeNumber { get; set; }
        [Display(Name = "الفرع")]
        //[Required(ErrorMessage = "هذا الحقل اجباري")]
        public int? BranchId { get; set; }
        [Display(Name = "صلاحيات")]
        //[Required(ErrorMessage = "هذا الحقل اجباري")]
        public int? RoleId { get; set; }
        [Display(Name = "الاسم الأول")]
        //[Required(ErrorMessage = "هذا الحقل اجباري")]
        public string? FirstName { get; set; }
        [Display(Name = "الكنية")]
        //[Required(ErrorMessage = "هذا الحقل اجباري")]
        public string? LastName { get; set; }
        [Display(Name = "رقم الهاتف")]
        //[Required(ErrorMessage = "هذا الحقل اجباري")]
       // [RegularExpression(@"^(966)[0-9]{8}$", ErrorMessage = "يرجى ادخل رقم بصيغة966xxxxxxxxx")]
        public string? Phone { get; set; }
        [Display(Name = " الايميل")]
        //[Required(ErrorMessage = "هذا الحقل اجباري")]
        public string? Email { get; set; }
        [Display(Name = "تاريخ التسجيل")]
        //[Required(ErrorMessage = "هذا الحقل اجباري")]
        public DateTime? RegisterDate { get; set; }
        [Display(Name = "اسم المستخدم")]
        //[Required(ErrorMessage = "هذا الحقل اجباري")]
        public string UserName { get; set; } = null!;
        [Display(Name = "كلمة السر")]
        [Required(ErrorMessage = "هذا الحقل اجباري")]
        public string Password { get; set; } = null!;

        public virtual Branch? Branch { get; set; }
        public virtual EmployeeRole? Role { get; set; }
        public virtual ICollection<QoutationUpdate> QoutationUpdates { get; set; }
    }
}
