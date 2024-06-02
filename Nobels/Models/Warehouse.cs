using System;
using System.Collections.Generic;

namespace Nobels.Models
{
    public partial class Warehouse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? CityId { get; set; }
    }
}
