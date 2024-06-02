using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nobels.Models
{
    public partial class ItemColor
    {
        [Display(Name = "المعرف")]
        public int ItemColorId { get; set; }
        [Display(Name = " اللون")]
        public int? ColorId { get; set; }
        [Display(Name = "العنصر")]
        public int? ItemId { get; set; }
        [Display(Name = "السعر")]
        [Required(ErrorMessage = "الرجاء إخال السعر")]
        public double  SpecialPrice { get; set; }
        public bool isEnabled { get; set; }
        public virtual Color? Color { get; set; }
        public virtual Item? Item { get; set; }
    }
}
