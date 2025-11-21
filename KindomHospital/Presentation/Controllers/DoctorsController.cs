using KingdomHospital.Application.DTOs.Consultations;
using KingdomHospital.Application.DTOs.Doctors;
using KingdomHospital.Application.DTOs.Ordonnances;
using KingdomHospital.Application.DTOs.Patients;
using KingdomHospital.Application.DTOs.Specialties;
using KingdomHospital.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace KingdomHospital.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DoctorsController : ControllerBase
{
    private readonly DoctorService _service;

    public DoctorsController(DoctorService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DoctorDto>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DoctorDto>> GetById(int id)
    {
        var dto = await _service.GetByIdAsync(id);
        if (dto is null)
            return NotFound();

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<DoctorDto>> Create([FromBody] DoctorCreateDto dto)
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
            // Specialty inexistante ou doublon
            return Conflict(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] DoctorUpdateDto dto)
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
            // Médecin avec consultations/ordonnances
            return Conflict(ex.Message);
        }
    }

    // ----------------------------------------------
    // RELATIONNEL : Specialty ↔ Doctor
    // ----------------------------------------------

    [HttpGet("{id:int}/specialty")]
    public async Task<ActionResult<SpecialtyDto>> GetDoctorSpecialty(int id)
    {
        var specialty = await _service.GetDoctorSpecialtyAsync(id);
        if (specialty is null)
            return NotFound();

        return Ok(specialty);
    }

    [HttpPut("{doctorId:int}/specialty/{specialtyId:int}")]
    public async Task<IActionResult> ChangeDoctorSpecialty(int doctorId, int specialtyId)
    {
        try
        {
            var ok = await _service.ChangeDoctorSpecialtyAsync(doctorId, specialtyId);
            if (!ok)
                return NotFound();

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("~/api/specialties/{specialtyId:int}/doctors")]
    public async Task<ActionResult<IEnumerable<DoctorDto>>> GetDoctorsBySpecialty(int specialtyId)
    {
        try
        {
            var result = await _service.GetBySpecialtyAsync(specialtyId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    //Consultations d'un docteur
    [HttpGet("{doctorId:int}/consultations")]
    public async Task<ActionResult<IEnumerable<ConsultationDto>>> GetConsultationsByDoctor(
    int doctorId,
    [FromQuery] DateOnly? from,
    [FromQuery] DateOnly? to,
    [FromQuery] int? patientId)
    {
        try
        {
            var result = await _service.GetConsultationsByDoctorAsync(doctorId, from, to, patientId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    //Patients d'un docteur
    [HttpGet("{doctorId:int}/patients")]
    public async Task<ActionResult<IEnumerable<PatientDto>>> GetPatientsByDoctor(int doctorId)
    {
        try
        {
            var result = await _service.GetPatientsByDoctorAsync(doctorId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    //Ordonnances d'un docteur
    [HttpGet("{doctorId:int}/ordonnances")]
    public async Task<ActionResult<IEnumerable<OrdonnanceDto>>> GetOrdonnancesByDoctor(
    int doctorId,
    [FromQuery] DateOnly? from,
    [FromQuery] DateOnly? to,
    [FromQuery] int? patientId)
    {
        try
        {
            var result = await _service.GetOrdonnancesByDoctorAsync(doctorId, from, to, patientId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }


}
