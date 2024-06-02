using System.ComponentModel.DataAnnotations;

namespace Nobels.Models
{
    public class BillModel
    {
        [Display(Name = "الكود")]
        public string code { get; set; }
        [Display(Name = "اسم المنتج")]
        public string itemname { get; set; }
        [Display(Name = "السعر")]
        public string unit { get; set; }
        [Display(Name = "الكمية")]
        public float quantity { get; set; }
        [Display(Name = "السعر الإفرادي")]
        public double Unitprice { get; set; }
        [Display(Name = "نوع الخصم")]
        public string percentType { get; set; }
        [Display(Name = "قيمة الخصم")]
        public double discountValue { get; set; }
        [Display(Name = "الإجمالي")]
        public double totalValue { get; set; }
        [Display(Name = "رقم العرض")]

        public int quationId { get; set; }
        [Display(Name = "اسم الزبون")]
        public  string Customer { get; set; }
        [Display(Name = "الفرع")]
        public string branch { get; set; }
        [Display(Name = "رقم الهاتف")]
        public string phone { get; set; }
        [Display(Name = "التاريخ")]
        public DateTime? BillDate { get; set; }

        [Display(Name = "المدينة")]
        public  string CityName { get; set; }
        public int orderId   { get; set; }
        public int roomId { get; set; }

        [Display(Name = "الغرفة")]
        public string roomName { get; set; }
        [Display(Name = "العنصر الأساسي")]
        public string MainItemName { get; set; }
        [Display(Name = "الموظف")]
        public string employeeName { get; set; }
        public string gender { get; set; }

        public int Colorid { get; set; }
        public string Colorname { get; set; }
        public string Colorname1 { get; set; }
        public string Colorname2 { get; set; }
        public string notes { get; set; }
        [Display(Name = "الحالة")]

        public string status { get; set; }
        public bool isEnabel { get; set; }
        [Display(Name = "الاسم")]
        //[Required(ErrorMessage = "هذا الحقل اجباري")]

        public string ArabicName { get; set; } = null!;
        [Display(Name = "الاسم بالانكليزية")]
        public string? EnglishName { get; set; }
      
        [Display(Name = "الحي")]
        public string neighborhood { get; set; }
        [Display(Name = "الشارع")]
        public string street { get; set; }
        [Display(Name = "رقم ا لبيت")]
        public string houseNumber { get; set; }
        [Display(Name = "رقم الهاتف")]
        public string Phone1 { get; set; }
        public int customerid { get; set; }
        public int EmployeeId { get; set; }
        public string? UOM { get; set; }


    }
}
