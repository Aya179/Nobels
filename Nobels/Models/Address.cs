using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Nobels.Models
{
    public partial class Address



    {
        [Display(Name = "المعرف")]
        public int id { get; set; }
        [Display(Name = "المدينة")]
        public int cityid { get; set; }
        [Display(Name = "الحي")]
        public string neighborhood { get; set; }
        [Display(Name = "الشارع")]
        public string street { get; set; }
        [Display(Name = "رقم ا لبيت")]
        public string houseNumber { get; set; }


    }
}
