using System;
using System.Collections.Generic;

namespace Nobels.Models
{
    public partial class InstallationSchedule
    {
        public int Id { get; set; }
        public DateTime? InstallationDate { get; set; }
        public byte? IsConfirmed { get; set; }
        public int? Duration { get; set; }
        public int? InstallationRequestId { get; set; }
        public DateTime? AddedDate { get; set; }
        public int? EmployeeId { get; set; }
        public int? TeamId { get; set; }
    }
}
