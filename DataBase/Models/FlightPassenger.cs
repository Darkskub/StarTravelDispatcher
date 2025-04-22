using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataBase.Models;

public partial class FlightPassenger
{
    [Key]
    public int Id { get; set; }

    public int FlightId { get; set; }

    public int PassengerId { get; set; }

    [ForeignKey("FlightId")]
    [InverseProperty("FlightPassengers")]
    public virtual Flight Flight { get; set; } = null!;

    [ForeignKey("PassengerId")]
    [InverseProperty("FlightPassengers")]
    public virtual Passenger Passenger { get; set; } = null!;
}
