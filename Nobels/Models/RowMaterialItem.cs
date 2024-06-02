using System;
using System.Collections.Generic;

namespace Nobels.Models
{
    public partial class RowMaterialItem
    {
        public int Id { get; set; }
        public int? ItemId { get; set; }
        public int? RowMatererialId { get; set; }

        public virtual Item? Item { get; set; }
        public virtual RawMaterial? RowMatererial { get; set; }
    }
}
