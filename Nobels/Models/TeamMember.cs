using System;
using System.Collections.Generic;

namespace Nobels.Models
{
    public partial class TeamMember
    {
        public int Id { get; set; }
        public int? EmployeeId { get; set; }
        public int? TeamId { get; set; }
    }
}
