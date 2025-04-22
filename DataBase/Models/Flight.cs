using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataBase.Models;

public partial class Flight
{
    [Key]
    public int Id { get; set; }

    public int ShipId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime Date { get; set; }

    [InverseProperty("Flight")]
    public virtual ICollection<FlightPassenger> FlightPassengers { get; set; } = new List<FlightPassenger>();

    [InverseProperty("Flight")]
    public virtual ICollection<FlightPath> FlightPaths { get; set; } = new List<FlightPath>();

    [ForeignKey("ShipId")]
    [InverseProperty("Flights")]
    public virtual Ship Ship { get; set; } = null!;
}
