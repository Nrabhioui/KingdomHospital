using KingdomHospital.Application.DTOs.Consultations;
using KingdomHospital.Application.DTOs.Ordonnances;
using KingdomHospital.Application.DTOs.Patients;
using KingdomHospital.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace KingdomHospital.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly PatientService _service;

    public PatientsController(PatientService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PatientDto>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PatientDto>> GetById(int id)
    {
        var dto = await _service.GetByIdAsync(id);
        if (dto is null)
            return NotFound();

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<PatientDto>> Create([FromBody] PatientCreateDto dto)
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
    public async Task<IActionResult> Update(int id, [FromBody] PatientUpdateDto dto)
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

    // --------------------------
    // RELATIONNEL : PATIENT ↔ CONSULTATIONS / ORDONNANCES
    // --------------------------

    [HttpGet("{patientId:int}/consultations")]
    public async Task<ActionResult<IEnumerable<ConsultationDto>>> GetConsultationsByPatient(int patientId)
    {
        try
        {
            var result = await _service.GetConsultationsByPatientAsync(patientId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("{patientId:int}/ordonnances")]
    public async Task<ActionResult<IEnumerable<OrdonnanceDto>>> GetOrdonnancesByPatient(int patientId)
    {
        try
        {
            var result = await _service.GetOrdonnancesByPatientAsync(patientId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

}
