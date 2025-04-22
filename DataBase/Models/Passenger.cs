using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataBase.Models;

public partial class Passenger
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string FullName { get; set; } = null!;

    [InverseProperty("Passenger")]
    public virtual ICollection<FlightPassenger> FlightPassengers { get; set; } = new List<FlightPassenger>();
}
