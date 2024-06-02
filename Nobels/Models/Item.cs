using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Nobels.Models
{
    public partial class Item
    {
        public Item()
        {
            ItemColors = new HashSet<ItemColor>();
            OrderDetails = new HashSet<OrderDetail>();
            RowMaterialItems = new HashSet<RowMaterialItem>();
        }

        [Display(Name = "المعرف")]

        public int ItemId { get; set; }
        [Display(Name = "اسم المنتج بالعربية")]

        public string? ItemArName { get; set; }
        [Display(Name = "اسم المنتج بالانكليزية")]

        public string? ItemEnName { get; set; }
        [Display(Name = "كود المنتج")]

        public string? ItemCode { get; set; }
        [Display(Name = "السعر الحالي")]

        public double? ItemCurrentPrice { get; set; }
        [Display(Name = "التصنيف")]

        public int? ItemSubTypeId { get; set; }
        [Display(Name = "الصورة")]

        public string? ItemPhotoPath { get; set; }
        [Display(Name = "الملاحظات")]

        public string? Notes { get; set; }
        public double? RMCost { get; set; }
        public double? FCost { get; set; }
        public double? ICost { get; set; }
        public double? COG { get; set; }
        public string? UOM { get; set; }

        public virtual SubType? ItemSubType { get; set; }
        public virtual ICollection<ItemColor> ItemColors { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<RowMaterialItem> RowMaterialItems { get; set; }
    }
}
