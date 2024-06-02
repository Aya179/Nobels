using System;
using System.Collections.Generic;

namespace Nobels.Models
{
    public partial class InstallationUpdate
    {
        public int Id { get; set; }
        public int? RequestId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdateString { get; set; }
        public int? EmployeeId { get; set; }
    }
}
