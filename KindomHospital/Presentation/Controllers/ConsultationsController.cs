using KingdomHospital.Application.DTOs.Consultations;
using KingdomHospital.Application.DTOs.Ordonnances;
using KingdomHospital.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace KingdomHospital.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConsultationsController : ControllerBase
{
    private readonly ConsultationService _service;

    public ConsultationsController(ConsultationService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ConsultationDto>>> GetAll(
        [FromQuery] int? doctorId,
        [FromQuery] int? patientId,
        [FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to)
    {
        // Si aucun filtre → même comportement qu'avant : tout renvoyer
        if (doctorId is null && patientId is null && from is null && to is null)
        {
            var all = await _service.GetAllAsync();
            return Ok(all);
        }

        // Règle du Endpoints utilitaires  : au moins un des deux Id doit être non nul
        if (doctorId is null && patientId is null)
            return BadRequest("Au moins doctorId ou patientId doit être fourni.");

        var filtered = await _service.GetFilteredAsync(doctorId, patientId, from, to);
        return Ok(filtered);
    }


    [HttpGet("{id:int}")]
    public async Task<ActionResult<ConsultationDto>> GetById(int id)
    {
        var dto = await _service.GetByIdAsync(id);
        if (dto is null)
            return NotFound();

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<ConsultationDto>> Create([FromBody] ConsultationCreateDto dto)
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

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ConsultationUpdateDto dto)
    {
        try
        {
            var ok = await _service.UpdateAsync(id, dto);
            if (!ok)
                return NotFound();

            return NoContent();
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

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok)
                return NotFound();

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }


    // ----------------------------
    // RELATIONNEL : Consultation - Ordonnance
    // ----------------------------
    [HttpGet("{id:int}/ordonnances")]
    public async Task<ActionResult<IEnumerable<OrdonnanceDto>>> GetOrdonnancesByConsultation(int id)
    {
        try
        {
            var result = await _service.GetOrdonnancesByConsultationAsync(id);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost("{id:int}/ordonnances")]
    public async Task<ActionResult<OrdonnanceDto>> CreateOrdonnanceForConsultation(
        int id, [FromBody] OrdonnanceCreateDto dto)
    {
        try
        {
            var created = await _service.CreateOrdonnanceForConsultationAsync(id, dto);
            return CreatedAtAction(nameof(GetOrdonnancesByConsultation), new { id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }


}
