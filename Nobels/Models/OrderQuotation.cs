using System.ComponentModel.DataAnnotations;

namespace Nobels.Models
{
    public class OrderQuotation
    {
        [Display(Name = "اسم العميل")]
        public string cusName { set; get; }
        [Display(Name = "الكمية")]
        public int quantity { set; get; }
        [Display(Name = "اسم المنتج")]
        public string ItemName { set; get; }
        [Display(Name = "السعر")]
        public float price { set; get; }
        [Display(Name = "الخصم")]
        public float discount { set; get; }
        [Display(Name = "نوع الخصم")]
        public string DiscountType  { set; get; }
        [Display(Name = "الإجمالي")]
        public float Total { set; get; }
        [Display(Name = "اللون")]
        public string colorName  { set; get; }


    }
}
