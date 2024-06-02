using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nobels.Models
{
    public partial class SubType
    {
        public SubType()
        {
            Items = new HashSet<Item>();
        }

        [Display(Name = "المعرف")]
        public int SubTypeId { get; set; }
        [Display(Name = "النوع")]
        public int? TypeId { get; set; }
        [Display(Name = "اسم النوع الفرعي بالانكليزي")]
        public string? SubTypeEnName { get; set; }
        [Display(Name = "اسم النوع الفرعي بالعربي")]
        public string? SubTypeArName { get; set; }
        [Display(Name = "الصورة")]
        public string? Photopath { get; set; }
        [Display(Name = "الملاحظات")]
        public string? Notes { get; set; }

        public virtual ItemType? Type { get; set; }
        public virtual ICollection<Item> Items { get; set; }
    }
}
