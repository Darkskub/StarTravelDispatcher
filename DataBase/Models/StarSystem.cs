using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataBase.Models;

public partial class StarSystem
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    public double X { get; set; }

    public double Y { get; set; }

    [InverseProperty("StarSystem")]
    public virtual ICollection<FlightPath> FlightPaths { get; set; } = new List<FlightPath>();
}
