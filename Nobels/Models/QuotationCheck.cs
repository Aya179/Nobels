using System;
using System.Collections.Generic;
using Nobels.Models;

namespace Nobels.Models;

public partial class QuotationCheck
{
    public int Id { get; set; }

    public int? Quotationid { get; set; }

    public int? Employeeid { get; set; }

    public DateTime? Checkdate { get; set; }

    public bool? Isok { get; set; }

    public string? Notes { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual Quotation? Quotation { get; set; }
}
