using System;
using System.Collections.Generic;

namespace Nobels.Models
{
    public partial class WarehouseUpdate
    {

        public int Id { get; set; }
        public int? RequestId { get; set; }
        public int? WarehouseId { get; set; }
        public DateTime? PalletsDate { get; set; }
        public DateTime? ShipmentDate { get; set; }
        public DateTime? TeamReceiveDate { get; set; }
    }
}
