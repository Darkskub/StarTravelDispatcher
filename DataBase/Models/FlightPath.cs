using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataBase.Models;

public partial class FlightPath
{
    [Key]
    public int Id { get; set; }

    public int FlightId { get; set; }

    public int StarSystemId { get; set; }

    public int SequenceIndex { get; set; }

    [ForeignKey("FlightId")]
    [InverseProperty("FlightPaths")]
    public virtual Flight Flight { get; set; } = null!;

    [ForeignKey("StarSystemId")]
    [InverseProperty("FlightPaths")]
    public virtual StarSystem StarSystem { get; set; } = null!;
}
