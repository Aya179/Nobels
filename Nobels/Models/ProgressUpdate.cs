using System;
using System.Collections.Generic;

namespace Nobels.Models
{
    public partial class ProgressUpdate
    {
        public int Id { get; set; }
        public int? RequestId { get; set; }
        public int? CurrentProgress { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int? EmployeeId { get; set; }
        public string? Notes { get; set; }
    }
}
