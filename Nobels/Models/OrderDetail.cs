using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nobels.Models;

namespace Nobels.Models
{
    public partial class OrderDetail
    {
        [Display(Name = "رقم العرضية")]
        [Key]
        public int DetailId { get; set; }
        [Display(Name ="العنصر")]
        public int? ItemId { get; set; }
        [Display(Name = "رقم العرض")]
        public int QoutationId { get; set; }
        [Display(Name = "الكمية")]
        public float Quantity { get; set; }
        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }
        [Display(Name = "السعر")]
        public float Price { get; set; }
        [Display(Name = "الخصم")]
        public float Discount { get; set; }
        [Display(Name = "الإجمالي")]
        public float TotalValue { get; set; }
        [Display(Name = "نوع الخصم")]
        public string DiscountType { get; set; }
        [Display(Name = "نوع العرض")]
        public int roomId { get; set; }

        [Display(Name ="اللون رقم1")]
        public string colorname { get; set; }
        [Display(Name ="اللون رقم2")]
        public string colorname1 { get; set; }
        [Display(Name ="اللون رقم3")]
        public string colorname2 { get; set; }

        [Display(Name = "المادة الأساسية")]
        public int? MainItemId { get; set; }
        public virtual MainItem? MainItem { get; set; }


        public virtual Item? Item { get; set; }
        public virtual Quotation? Qoutation { get; set; }
    }
}
