using Microsoft.AspNetCore.Mvc;
using InfluxdbService.Models;
using InfluxdbService.Services;

namespace InfluxdbService.Controllers;

[ApiController]
[Route("[controller]")]
public class VitalSignsController : ControllerBase
{
    private readonly VitalSignService _vitalSignService;

    public VitalSignsController(VitalSignService vitalSignService)
    {
        _vitalSignService = vitalSignService;
    }

    [HttpPost]
    public async Task<IActionResult> RecordVitalSign([FromBody] VitalSignRequest request)
    {
        await _vitalSignService.WriteVitalSignAsync(request);
        return CreatedAtAction(nameof(GetVitalSigns), new { patientId = request.PatientId }, request);
    }

    [HttpGet("{patientId}")]
    public async Task<IActionResult> GetVitalSigns(string patientId, [FromQuery] string? range)
    {
        var vitals = await _vitalSignService.GetVitalSignsAsync(patientId, range ?? "-1h");
        return Ok(vitals);
    }
}
