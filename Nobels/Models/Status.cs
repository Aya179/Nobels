using System;
using System.Collections.Generic;

namespace Nobels.Models
{
    public partial class Status
    {
        public Status()
        {
           // SubStatuses = new HashSet<SubStatus>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Note { get; set; }

       // public virtual ICollection<SubStatus> SubStatuses { get; set; }
    }
}
