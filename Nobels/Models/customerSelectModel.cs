using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Nobels.Models
{
    public class customerSelectModel
    {




        [Display(Name = "المعرف")]

        public int id { set; get; }
        [Display(Name = "اسم العميل")]

        public string CustomerName { set; get; }
        [Display(Name = "رقم الهاتف")]

        public string CustomerPhone { set; get; }
        [Display(Name = "تاريخ الإنشاء")]

        public DateTime CreatedDate { set; get; }
        [Display(Name = "اسم الموظف")]

        public string EmployeeName { get; set; }
        [Display(Name = "الكود")]

        public string code { set; get; }

        [Display(Name = "التصنيف")]

        public string mainTypeName { set; get; }
        public int maintypeiD { set; get; }
        [Display(Name = "تصنيف فرعي")]

        public string SubtypeName { set; get; }

        public int subId { set; get; }
        [Display(Name = "الصنف")]

        public string itemArName { set; get; }
        public string rawName { set; get; }
        public int itemId { set; get; }

        [Display(Name = "السعر الحالي")]

        public double? ItemCurrentPrice { get; set; }
        public Quotation qu = new Quotation();
        public ICollection<Color> Colors { get; set; }
        public ICollection<RawMaterial>  rawMaterials{ get; set; }
        public ICollection<RoeMaterialColor>  roeMaterialColors{ get; set; }
        

    }
}
