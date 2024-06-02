using System;
using System.Collections.Generic;

namespace Nobels.Models
{
    public partial class ProductionSchedule
    {
        public int Id { get; set; }
        public int? IntallationRequestId { get; set; }
        public DateTime? ProductionDate { get; set; }
        public byte? IsConfirmed { get; set; }
        public int? EmployeeId { get; set; }
        public DateTime? AddedDate { get; set; }
    }
}
