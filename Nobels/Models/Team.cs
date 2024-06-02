namespace Nobels.Models
{
    public partial class Team
    {

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int? CityId { get; set; }


    }
}
