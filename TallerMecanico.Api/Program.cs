using Microsoft.EntityFrameworkCore;
using TallerMecanico.Api.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi(options =>
    options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0);

// Cadena de conexión:
//   local  -> appsettings.Development.json (MySQL en Docker, puerto 3307)
//   nube   -> variable de entorno ConnectionStrings__TallerDB (MonsterASP.NET)
var cadenaConexion = builder.Configuration.GetConnectionString("TallerDB");
if (string.IsNullOrWhiteSpace(cadenaConexion))
    throw new InvalidOperationException(
        "Falta la cadena de conexión 'TallerDB'. Configúrala en appsettings.Development.json o en la variable de entorno ConnectionStrings__TallerDB.");

builder.Services.AddDbContext<TallerContext>(options => options.UseMySQL(cadenaConexion));

// CORS con orígenes concretos (nunca AllowAnyOrigin).
var origenesPermitidos = builder.Configuration.GetSection("Cors:Origenes").Get<string[]>()
    ?? ["http://localhost:3000"];

builder.Services.AddCors(options =>
    options.AddPolicy("Front", policy => policy
        .WithOrigins(origenesPermitidos)
        .AllowAnyHeader()
        .AllowAnyMethod()));

var app = builder.Build();


// Swagger visible también en la API publicada.
app.MapOpenApi();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "Taller Mecánico API v1");
    options.RoutePrefix = "swagger";
    options.DocumentTitle = "Taller Mecánico API";
});

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("Front");
app.UseAuthorization();
app.MapControllers();

app.Run();
