using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using Nobels.Models;

namespace Nobels.Models;

public partial class MainItem
{
    public int Id { get; set; }
    [Display(Name = "اسم المنتج الاساسي")]

    public string? MainItemName { get; set; }
    [Display(Name = "Main Item Name")]

    public string? MainItemEnName { get; set; }
    [Display(Name = "الصورة")]

    public string? MainItemPhoto { get; set; } = " "!;

    //public virtual ICollection<ItemType> ItemTypes { get; } = new List<ItemType>();
   // public virtual ICollection<Item> Items { get; } = new List<Item>();
    public virtual ICollection<OrderDetail> OrderDetails  { get; } = new List<OrderDetail>();

}
