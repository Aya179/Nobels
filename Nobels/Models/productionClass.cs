namespace Nobels.Models
{
    public class productionClass
    {
        public int quotionNumberId { get; set; }
        public int ?IntallationRequestId { get; set; }
        public int ?IntallationScheduleId { get; set; }
        public int ?ProductionSheduleId { get; set; }
        public string quotionCode { get; set; }
        public string CustomerArabicName { get; set; }
        public string city { get; set; }
        public string branch { get; set; }
        public string neighborhood { get; set; }
        //التاريخ المقترح للتركيب
        public DateTime? ProductionDate { get; set; }
        public DateTime? InstallationDate { get; set; }
        public string teamName { get; set; } = null!;
        public string status { get; set; } = null!;
        public byte? IsConfirmed { get; set; }

        public int? Duration { get; set; }
        public  int? teamid { get; set; }
        public int? Progress { get; set; }
        public DateTime? Statusdate { get; set; }
        //شحن
        public int? WarehouseUpdateId { get; set; }
        public DateTime? PalletsDate { get; set; }
        public DateTime? ShipmentDate { get; set; }
        public DateTime? TeamReceiveDate { get; set; }
       public string Phone { get; set; } = null!;
       public string name { get; set; } = null!;
       public string note { get; set; } = null!;
       public DateTime? QuotationSimpleDate { get; set; }

        public string? quotation_date { get; set; }


        public string? FinancialDate { get; set; }
        public string? TechnicalofficeDate { get; set; }
        public DateTime? AddedDate { get; set; }








    }
}
