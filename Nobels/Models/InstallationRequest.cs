using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Nobels.Models
{
    public partial class InstallationRequest
    {
        [Display(Name = "المعرف")]

        public int Id { get; set; }

        public int? QutationId { get; set; }
        public DateTime? CreationDate { get; set; }
        public int? Progress { get; set; }
        public int? EmployeeId { get; set; }
        [Display(Name = "التاريخ المقترح للتركيب")]

        public DateTime? DesiredDate { get; set; }
    }
}
