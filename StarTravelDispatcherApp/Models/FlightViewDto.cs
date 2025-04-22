using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarTravelDispatcherApp.Models
{
    public class FlightViewDto
    {
        public int Id { get; set; }
        public string ShipName { get; set; }
        public string Date { get; set; }
        public List<string> StarSystems { get; set; } = new();
    }
}
