using KingdomHospital.Application.DTOs.Medicaments;
using KingdomHospital.Application.DTOs.Ordonnances;
using KingdomHospital.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace KingdomHospital.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicamentsController : ControllerBase
{
    private readonly MedicamentService _service;

    public MedicamentsController(MedicamentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MedicamentDto>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MedicamentDto>> GetById(int id)
    {
        var dto = await _service.GetByIdAsync(id);
        if (dto is null)
            return NotFound();

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<MedicamentDto>> Create([FromBody] MedicamentCreateDto dto)
    {
        try
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }
    // ----------------------------
    // RELATIONNEL : Medicament - Ordonnances
    // ----------------------------
    [HttpGet("{id:int}/ordonnances")]
    public async Task<ActionResult<IEnumerable<OrdonnanceDto>>> GetOrdonnancesByMedicament(int id)
    {
        try
        {
            var ordos = await _service.GetOrdonnancesByMedicamentAsync(id);
            return Ok(ordos);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

}
