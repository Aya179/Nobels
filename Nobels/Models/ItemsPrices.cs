namespace Nobels.Models
{
    public class ItemsPrices
    {


        public int id { get; set; }
        public int branchId { get; set; }
        public int ItemId { get; set; }
        public float price { get; set; }
        public string? ItemName { get; set; }
        public string? BranchName { get; set; }

    }
}
