using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarTravelDispatcherApp.Models
{
    public class FlightDto
    {
        public int Id { get; set; }
        public int ShipId { get; set; }
        public DateTime Date { get; set; }
        public List<int> StarSystemIds { get; set; } = new();
    }
}
