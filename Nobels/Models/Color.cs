using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Nobels.Models
{
    public partial class Color
    {
        public Color()
        {
            ItemColors = new HashSet<ItemColor>();
            RoeMaterialColors = new HashSet<RoeMaterialColor>();
        }

        [Display(Name = "المعرف")]
        public int ColorId { get; set; }
        [Display(Name = "اسم اللون")]
        public string? ColorName { get; set; } = null!;
        [Display(Name = "الكود")]
        public string ColorCode { get; set; } = null!;
        [Display(Name = "درجات اللون")]
        public string? ColorTemplatePhoto { get; set; }
        [Display(Name = "ملاحظات")]
        public string? Note { get; set; }
        [Display(Name = "نوع المنتج")]
        public int itemTypeId1 { get; set; }
        [Display(Name = "نوع المنتج")]
        public virtual ItemType? ItemTypeId { set; get; }
        public virtual ICollection<ItemColor> ItemColors { get; set; }
    

    public virtual ICollection<RoeMaterialColor> RoeMaterialColors { get; set; }
    }
}
