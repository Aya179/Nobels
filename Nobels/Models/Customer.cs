using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Nobels.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Quotations = new HashSet<Quotation>();
        }
        [Display(Name = "المعرف")]
        public int Id { get; set; }
        [Display(Name = "الاسم")]
        //[Required(ErrorMessage = "هذا الحقل اجباري")]

        public string ArabicName { get; set; } = null!;
        [Display(Name = "الاسم بالانكليزية")]
        //[Required(ErrorMessage = "هذا الحقل اجباري")]
        public string? EnglishName { get; set; }
        [Display(Name = "الايميل")]
        //[Required(ErrorMessage = "هذا الحقل اجباري")]
        public string? Email { get; set; }
        [Display(Name = "رقم الهاتف")]
        //[RegularExpression(@"^(9639)[0-9]{8}$", ErrorMessage = "يرجى ادخل رقم بصيغة 9639xxxxxxxx")]
        //[Required(ErrorMessage = "هذا الحقل اجباري")]
        public string Phone { get; set; } = null!;


        [Display(Name = "رقم هاتف ثان")]
        //[RegularExpression(@"^(9639)[0-9]{8}$", ErrorMessage = "يرجى ادخل رقم بصيغة 9639xxxxxxxx")]
        //[Required(ErrorMessage = "هذا الحقل اجباري")]
        public string? PhonNumber { get; set; } = null!;
        [Display(Name = "العنوان")]
        //[Required(ErrorMessage = "هذا الحقل اجباري")]
        public string Address { get; set; } = null!;
        [Display(Name = "عنوان ثان")]
        //[Required(ErrorMessage = "هذا الحقل اجباري")]
        public string? SecondAddress { get; set; } = null!;

        [Display(Name = "المدينة")]
        //[Required(ErrorMessage = "هذا الحقل اجباري")]
        public int CityId { get; set; }
        [Display(Name = "اللقب")]
        //[Required(ErrorMessage = "هذا الحقل اجباري")]
        public string? Gender { get; set; }
        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }
        [Display(Name = "الفرع")]
        public int branchId { get; set; }
        [Display(Name = "البائع")]
        public int EmployeeId { get; set; }
        public virtual City? City { get; set; }
       // public virtual Address? AddressSpecification { get; set; }
        public int adressId { set; get; }



        public virtual ICollection<Quotation> Quotations { get; set; }
    }
}
