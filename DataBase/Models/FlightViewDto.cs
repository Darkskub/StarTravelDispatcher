namespace DataBase.Models
{
    public class FlightViewDto
    {
        public int Id { get; set; }
        public string ShipName { get; set; }
        public string Date { get; set; }
        public List<string> StarSystems { get; set; } = new();
        public string Route => string.Join(" → ", StarSystems);
    }
}
