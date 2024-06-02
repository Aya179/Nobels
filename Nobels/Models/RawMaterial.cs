using System;
using System.Collections.Generic;

namespace Nobels.Models
{
    public partial class RawMaterial
    {
        public RawMaterial()
        {
            RoeMaterialColors = new HashSet<RoeMaterialColor>();
            RowMaterialItems = new HashSet<RowMaterialItem>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public double? Price { get; set; }
        public double? Cost { get; set; }
        public string? MeasurUnit { get; set; }
        public double? DamagedPercent { get; set; }

        public virtual ICollection<RoeMaterialColor> RoeMaterialColors { get; set; }
        public virtual ICollection<RowMaterialItem> RowMaterialItems { get; set; }
    }
}
