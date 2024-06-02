using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nobels.Models;

namespace Nobels.Models
{
    public partial class Quotation
    {
        public Quotation()
        {
            Bills = new HashSet<Bill>();
            OrderDetails = new HashSet<OrderDetail>();
            QoutationUpdates = new HashSet<QoutationUpdate>();
        }
        [Display(Name = "رقم العرض")]

        public int QuotationId { get; set; }
        [Display(Name = "الزبون")]
        public int? CustomerId { get; set; }
        [Display(Name = "حالة العرض")]
        public string? QuotationStatus { get; set; }

        [Display(Name = "الكود")]
        public string? Code { get; set; }
        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }
        [Display(Name = "الفرع")]
        public int? BranchId { get; set; }
        [Display(Name = "تاريخ العرض")]
        public DateTime? QuotationSimpleDate { get; set; }

        public DateTime? QuotationDate { get; set; }

        [Display(Name = "البائع")]
        public int employeeId { get; set; }
        public int? discount { get; set; }
        public string? discountType { get; set; }
        public DateTime? enddate { get; set; }
        public int? addressId { get; set; }


        public virtual Customer? Customer { get; set; }
        public virtual ICollection<Bill> Bills { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<QoutationUpdate> QoutationUpdates { get; set; }

        public virtual Branch? Branch { get; set; }
        //   public virtual ICollection<QutationStatus> QutationStatuses { get; set; }

    }
}