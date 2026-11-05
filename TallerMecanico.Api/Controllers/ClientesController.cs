using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TallerMecanico.Api.Models;

namespace TallerMecanico.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly TallerContext _context;

    public ClientesController(TallerContext context) => _context = context;

    // GET api/clientes?buscar=carlos&activo=true
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cliente>>> Get(string? buscar, bool? activo)
    {
        var consulta = _context.Clientes.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var texto = buscar.Trim();
            consulta = consulta.Where(c =>
                c.Nombre.Contains(texto) ||
                c.Apellido.Contains(texto) ||
                c.Documento.Contains(texto));
        }

        if (activo.HasValue)
            consulta = consulta.Where(c => c.Activo == activo.Value);

        var clientes = await consulta
            .OrderBy(c => c.Apellido)
            .ThenBy(c => c.Nombre)
            .ToListAsync();

        return Ok(clientes);
    }

    // GET api/clientes/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Cliente>> Get(int id)
    {
        var cliente = await _context.Clientes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.ClienteId == id);

        if (cliente is null) return NotFound(new { mensaje = $"No existe el cliente con id {id}." });

        return Ok(cliente);
    }

    // GET api/clientes/5/vehiculos  (ruta anidada)
    [HttpGet("{id}/vehiculos")]
    public async Task<ActionResult<IEnumerable<Vehiculo>>> GetVehiculos(int id)
    {
        if (!await _context.Clientes.AnyAsync(c => c.ClienteId == id))
            return NotFound(new { mensaje = $"No existe el cliente con id {id}." });

        var vehiculos = await _context.Vehiculos
            .AsNoTracking()
            .Where(v => v.ClienteId == id)
            .OrderBy(v => v.Placa)
            .ToListAsync();

        return Ok(vehiculos);
    }

    // POST api/clientes
    [HttpPost]
    public async Task<ActionResult<Cliente>> Post(Cliente cliente)
    {
        // Regla de negocio 1: documento único.
        if (await _context.Clientes.AnyAsync(c => c.Documento == cliente.Documento))
            return Conflict(new { mensaje = $"Ya existe un cliente con el documento {cliente.Documento}." });

        // Regla de negocio 2: correo único (solo si se envía).
        if (!string.IsNullOrWhiteSpace(cliente.Email) &&
            await _context.Clientes.AnyAsync(c => c.Email == cliente.Email))
            return Conflict(new { mensaje = $"Ya existe un cliente con el correo {cliente.Email}." });

        cliente.Activo ??= true;
        cliente.FechaRegistro = DateTime.Now;

        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = cliente.ClienteId }, cliente);
    }

    // PUT api/clientes/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, Cliente cliente)
    {
        if (id != cliente.ClienteId)
            return BadRequest(new { mensaje = "El id de la ruta no coincide con el id del cuerpo." });

        if (!await _context.Clientes.AnyAsync(c => c.ClienteId == id))
            return NotFound(new { mensaje = $"No existe el cliente con id {id}." });

        if (await _context.Clientes.AnyAsync(c => c.Documento == cliente.Documento && c.ClienteId != id))
            return Conflict(new { mensaje = $"Ya existe otro cliente con el documento {cliente.Documento}." });

        if (!string.IsNullOrWhiteSpace(cliente.Email) &&
            await _context.Clientes.AnyAsync(c => c.Email == cliente.Email && c.ClienteId != id))
            return Conflict(new { mensaje = $"Ya existe otro cliente con el correo {cliente.Email}." });

        // No se permite modificar la fecha de registro original.
        var fechaOriginal = await _context.Clientes
            .Where(c => c.ClienteId == id)
            .Select(c => c.FechaRegistro)
            .FirstAsync();
        cliente.FechaRegistro = fechaOriginal;
        cliente.Activo ??= true;

        _context.Entry(cliente).State = EntityState.Modified;
        _context.Entry(cliente).Property(c => c.FechaRegistro).IsModified = false;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE api/clientes/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente is null)
            return NotFound(new { mensaje = $"No existe el cliente con id {id}." });

        // Regla de negocio 3: no eliminar clientes con vehículos registrados.
        if (await _context.Vehiculos.AnyAsync(v => v.ClienteId == id))
            return Conflict(new { mensaje = "No se puede eliminar el cliente porque tiene vehículos registrados." });

        _context.Clientes.Remove(cliente);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
