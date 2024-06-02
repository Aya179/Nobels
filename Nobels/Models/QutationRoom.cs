using System.ComponentModel.DataAnnotations;

namespace Nobels.Models
{
    public class QutationRoom
    {

        [Key]
        public int id { get; set; }
        [Display(Name = "الغرفة")]
        public string roomName { get; set; }
        [Display(Name = "ملاحظات")]
        public string? notes { get; set; }
        [Display(Name = "الخصم")]
        public int? discount { get; set; }
        [Display(Name = "نوع الخصم")]
        public string? discountType { get; set; }
          
        public int qutationId { get; set; }
    }
}
