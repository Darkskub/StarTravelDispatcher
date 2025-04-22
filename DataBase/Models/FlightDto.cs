namespace DataBase.Models
{
    public class FlightDto
    {
        public int ShipId { get; set; }
        public DateTime Date { get; set; }
        public List<int> StarSystemIds { get; set; } = new();
    }

}
