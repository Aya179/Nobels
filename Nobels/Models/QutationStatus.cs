using System;
using System.Collections.Generic;

namespace Nobels.Models
{
    public partial class QutationStatus
    {
        public int Id { get; set; }
        public int QutationId { get; set; }
       //public int SubStatusId { get; set; }
       public string status { get; set; }
        public int? EmployeeId { get; set; }
        public DateTime? Statusdate { get; set; }

     //   public virtual Quotation Qutation { get; set; } = null!;
       // public virtual SubStatus SubStatus { get; set; } = null!;
    }
}
