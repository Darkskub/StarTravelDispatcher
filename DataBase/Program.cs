using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using DataBase.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

var key = "this_is_a_super_secure_jwt_secret_key_123!";
var issuer = "StarTravelServer";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };
    });

// Подключаем контекст базы данных
builder.Services.AddDbContext<StarTravelContext>(options =>
    options.UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Darkskub\\Desktop\\University\\Github\\C#\\StarTravelDispatcher\\DataBase\\Database1.mdf;Integrated Security=True"));

// Подключаем JSON-сериализацию (без циклов)
builder.Services.AddControllers().AddJsonOptions(opt =>
    opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.WebHost.UseUrls("http://localhost:5000");

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Endpoint добавления рейса
app.MapPost("/api/flights", [Authorize] async (FlightDto dto, StarTravelContext db) =>
{
    var flight = new Flight
    {
        ShipId = dto.ShipId,
        Date = dto.Date
    };

    db.Flights.Add(flight);
    await db.SaveChangesAsync(); // Сохраняем рейс и получаем его ID

    for (int i = 0; i < dto.StarSystemIds.Count; i++)
    {
        db.FlightPaths.Add(new FlightPath
        {
            FlightId = flight.Id,
            StarSystemId = dto.StarSystemIds[i],
            SequenceIndex = i
        });
    }

    await db.SaveChangesAsync();
    return Results.Ok(flight.Id);
});

app.MapPost("/api/login", async (LoginDto dto, StarTravelContext db) =>
{
    Console.WriteLine($"[DEBUG] Username: {dto.Username}");
    Console.WriteLine($"[DEBUG] Password: {dto.Password}");
    Console.WriteLine($"[DEBUG] Hash: {ComputeHash(dto.Password)}");

    var user = await db.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
    if (user == null) return Results.Unauthorized();

    var inputHash = ComputeHash(dto.Password);
    if (user.PasswordHash != inputHash) return Results.Unauthorized();

    var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
    var tokenKey = Encoding.UTF8.GetBytes(key);

    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, dto.Username)
        }),
        Expires = DateTime.UtcNow.AddHours(1),
        Issuer = issuer,
        SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
    };

    var token = tokenHandler.CreateToken(tokenDescriptor);
    var tokenString = tokenHandler.WriteToken(token);

    return Results.Ok(tokenString);
});

string ComputeHash(string password)
{
    using var sha = SHA256.Create();
    var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
    return Convert.ToBase64String(bytes);
}

app.MapGet("/api/ships", [Authorize] async (StarTravelContext db) =>
    await db.Ships.Select(s => new { s.Id, s.Name }).ToListAsync());

app.MapDelete("/api/flights/{flightId}/passengers", RemovePassengersFromFlight)
    .RequireAuthorization();

static async Task<IResult> RemovePassengersFromFlight(
    int flightId,
    HttpContext context,
    [FromServices] StarTravelContext db)
{
    // ВНИМАНИЕ: passengerIds НЕ в параметрах метода
    var passengerIds = await context.Request.ReadFromJsonAsync<List<int>>();

    if (passengerIds == null || !passengerIds.Any())
        return Results.BadRequest("Список пассажиров не передан.");

    var toDelete = await db.FlightPassengers
        .Where(fp => fp.FlightId == flightId && passengerIds.Contains(fp.PassengerId))
        .ToListAsync();

    db.FlightPassengers.RemoveRange(toDelete);
    await db.SaveChangesAsync();

    return Results.Ok();
}

app.MapGet("/api/flights", [Authorize] async (StarTravelContext db) =>
    await db.Flights
        .Include(f => f.Ship)
        .Include(f => f.FlightPaths)
        .ThenInclude(p => p.StarSystem)
        .Select(f => new FlightViewDto
        {
            Id = f.Id,
            ShipName = f.Ship.Name,
            Date = f.Date.ToShortDateString(),
            StarSystems = f.FlightPaths
                .OrderBy(p => p.SequenceIndex)
                .Select(p => p.StarSystem.Name).ToList()
        }).ToListAsync());

app.MapGet("/api/starsystems", [Authorize] async (StarTravelContext db) =>
    await db.StarSystems.Select(s => new { s.Id, s.Name, s.X, s.Y }).ToListAsync());

app.MapPost("/api/starsystems", [Authorize] async (StarSystemDto dto, StarTravelContext db) =>
{
    db.StarSystems.Add(new StarSystem { Name = dto.Name, X = dto.X, Y = dto.Y });
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.MapDelete("/api/starsystems/{id:int}", [Authorize] async (int id, StarTravelContext db) =>
{
    var s = await db.StarSystems.FindAsync(id);
    if (s == null) return Results.NotFound();

    db.StarSystems.Remove(s);
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.MapGet("/api/passengers", [Authorize] async (StarTravelContext db) =>
    await db.Passengers.Select(p => new { p.Id, p.FullName }).ToListAsync());

app.MapPost("/api/passengers", [Authorize] async (PassengerDto dto, StarTravelContext db) =>
{
    db.Passengers.Add(new Passenger { FullName = dto.FullName });
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.MapDelete("/api/passengers/{id:int}", [Authorize] async (int id, StarTravelContext db) =>
{
    var p = await db.Passengers.FindAsync(id);
    if (p == null) return Results.NotFound();
    db.Passengers.Remove(p);
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.MapPost("/api/flights/{flightId:int}/passengers", [Authorize] async (int flightId, List<int> passengerIds, StarTravelContext db) =>
{
    foreach (var id in passengerIds)
    {
        db.FlightPassengers.Add(new FlightPassenger { FlightId = flightId, PassengerId = id });
    }
    await db.SaveChangesAsync();
    return Results.Ok();
});

/*app.MapDelete("/api/flights/{flightId:int}/passengers", [Authorize] async (int flightId, List<int> passengerIds, StarTravelContext db) =>
{
    var records = await db.FlightPassengers
        .Where(fp => fp.FlightId == flightId && passengerIds.Contains(fp.PassengerId))
        .ToListAsync();

    db.FlightPassengers.RemoveRange(records);
    await db.SaveChangesAsync();
    return Results.Ok();
});*/

app.MapGet("/api/flights/{id:int}/passengers", [Authorize] async (int id, StarTravelContext db) =>
{
    return await db.FlightPassengers
        .Where(fp => fp.FlightId == id)
        .Select(fp => new { fp.Passenger.Id, fp.Passenger.FullName })
        .ToListAsync();
});

app.MapDelete("/api/flights/{id:int}", [Authorize] async (int id, StarTravelContext db) =>
{
    var flight = await db.Flights.FindAsync(id);
    if (flight == null)
        return Results.NotFound();

    db.Flights.Remove(flight);
    await db.SaveChangesAsync();
    return Results.Ok();
});


app.Run();
