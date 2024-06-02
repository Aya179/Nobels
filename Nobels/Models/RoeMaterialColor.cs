using System;
using System.Collections.Generic;

namespace Nobels.Models
{
    public partial class RoeMaterialColor
    {
        public int Id { get; set; }
        public int? ColorId { get; set; }
        public int? RowMaterialId { get; set; }

        public virtual Color? Color { get; set; }
        public virtual RawMaterial? RowMaterial { get; set; }
    }
}
