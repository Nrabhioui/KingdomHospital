using KingdomHospital.Application.DTOs.Ordonnances;
using KingdomHospital.Application.DTOs.OrdonnanceLignes;
using KingdomHospital.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace KingdomHospital.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdonnancesController : ControllerBase
{
    private readonly OrdonnanceService _ordonnanceService;
    private readonly OrdonnanceLigneService _ligneService;

    public OrdonnancesController(OrdonnanceService ordonnanceService, OrdonnanceLigneService ligneService)
    {
        _ordonnanceService = ordonnanceService;
        _ligneService = ligneService;
    }

    // CRUD Ordonnance

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrdonnanceDto>>> GetAll(
        [FromQuery] int? doctorId,
        [FromQuery] int? patientId,
        [FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to)
    {
        // Aucun filtre → toutes les ordonnances
        if (doctorId is null && patientId is null && from is null && to is null)
        {
            var all = await _ordonnanceService.GetAllAsync();
            return Ok(all);
        }

        // Règle endpoint utilitaire : au moins un des deux Id doit être fourni
        if (doctorId is null && patientId is null)
            return BadRequest("Au moins doctorId ou patientId doit être fourni.");

        var filtered = await _ordonnanceService.GetFilteredAsync(doctorId, patientId, from, to);
        return Ok(filtered);
    }


    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrdonnanceDto>> GetById(int id)
    {
        var dto = await _ordonnanceService.GetByIdAsync(id);
        if (dto is null)
            return NotFound();

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<OrdonnanceDto>> Create([FromBody] OrdonnanceCreateDto dto)
    {
        try
        {
            var created = await _ordonnanceService.CreateAsync(dto);
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
    public async Task<IActionResult> Update(int id, [FromBody] OrdonnanceUpdateDto dto)
    {
        try
        {
            var ok = await _ordonnanceService.UpdateAsync(id, dto);
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
            var ok = await _ordonnanceService.DeleteAsync(id);
            if (!ok)
                return NotFound();

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    // Lignes d’ordonnance (endpoints relationnels section 10) :contentReference[oaicite:8]{index=8}

    [HttpGet("{id:int}/lignes")]
    public async Task<ActionResult<IEnumerable<OrdonnanceLigneDto>>> GetLignes(int id)
    {
        var lignes = await _ligneService.GetForOrdonnanceAsync(id);
        return Ok(lignes);
    }

    [HttpPost("{id:int}/lignes")]
    public async Task<ActionResult<OrdonnanceLigneDto>> CreateLigne(
        int id,
        [FromBody] OrdonnanceLigneCreateDto dto)
    {
        try
        {
            var created = await _ligneService.CreateAsync(id, dto);
            return CreatedAtAction(nameof(GetLigneById),
                new { id, ligneId = created.Id },
                created);
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

    [HttpGet("{id:int}/lignes/{ligneId:int}")]
    public async Task<ActionResult<OrdonnanceLigneDto>> GetLigneById(int id, int ligneId)
    {
        var dto = await _ligneService.GetByIdAsync(id, ligneId);
        if (dto is null)
            return NotFound();

        return Ok(dto);
    }

    [HttpPut("{id:int}/lignes/{ligneId:int}")]
    public async Task<IActionResult> UpdateLigne(
        int id,
        int ligneId,
        [FromBody] OrdonnanceLigneUpdateDto dto)
    {
        try
        {
            var ok = await _ligneService.UpdateAsync(id, ligneId, dto);
            if (!ok)
                return NotFound();

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:int}/lignes/{ligneId:int}")]
    public async Task<IActionResult> DeleteLigne(int id, int ligneId)
    {
        var ok = await _ligneService.DeleteAsync(id, ligneId);
        if (!ok)
            return NotFound();

        return NoContent();
    }


    // ----------------------------
    // RELATIONNEL : Consultation - Ordonnance
    // ----------------------------
    [HttpPut("{id:int}/consultation/{consultationId:int}")]
    public async Task<IActionResult> SetConsultation(int id, int consultationId)
    {
        try
        {
            var ok = await _ordonnanceService.SetOrdonnanceConsultationAsync(id, consultationId);
            if (!ok)
                return NotFound();

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{id:int}/consultation")]
    public async Task<IActionResult> RemoveConsultation(int id)
    {
        var ok = await _ordonnanceService.RemoveOrdonnanceConsultationAsync(id);
        if (!ok)
            return NotFound();

        return NoContent();
    }

}
