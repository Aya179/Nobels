using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using Nobels.Models;

namespace Nobels.Models
{
    public partial class ItemType
    {
        public ItemType()
        {
            SubTypes = new HashSet<SubType>();
        }



        [Display(Name = "المعرف")]
        public int TypeId { get; set; }
        [Display(Name = "نوع المنتج بالعربي")] 

        public string? TypeArName { get; set; }
        [Display(Name = "نوع المنتج بالانكليزية")]

        public string? TypeEnName { get; set; }
        [Display(Name = "الصورة")]

        public string? Photopath { get; set; }
        [Display(Name = "المادة الأساسية")]
        //public int? MainItemId { get; set; }
        
        public virtual ICollection<SubType> SubTypes { get; set; }
        public virtual ICollection<Color> Colors { get; set; }
        //public virtual MainItem? MainItem { get; set; }
        
    }
}
