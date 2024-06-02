using System.ComponentModel.DataAnnotations;

namespace Nobels.Models
{
    public class Setting
    {
        public int id { get; set; }
        [Display(Name = "قيمة الضريبة")]
        public float tax { get; set; }
    }
}
