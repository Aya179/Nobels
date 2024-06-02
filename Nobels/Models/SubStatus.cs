using System;
using System.Collections.Generic;

namespace Nobels.Models
{
    public partial class SubStatus
    {
        public SubStatus()
        {
          //  QutationStatuses = new HashSet<QutationStatus>();
        }

        public int Id { get; set; }
       // public int? StatusId { get; set; }
        public int? QutationStatusId { get; set; }
        public string? status { get; set; }

      //  public virtual Status? Status { get; set; }
       // public virtual ICollection<QutationStatus> QutationStatuses { get; set; }
    }
}
