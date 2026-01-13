using Microsoft.AspNetCore.Mvc;
using InfluxdbService.Models;
using InfluxdbService.Services;

namespace InfluxdbService.Controllers;

[ApiController]
[Route("[controller]")]
public class VitalSignsController(VitalSignService vitalSignService) : ControllerBase
{
    private readonly VitalSignService _vitalSignService = vitalSignService;

    [HttpPost]
    public async Task<IActionResult> RecordVitalSign([FromBody] VitalSignRequest request)
    {
        await _vitalSignService.WriteVitalSignAsync(request);
        return CreatedAtAction(nameof(GetVitalSigns), new { patientId = request.PatientId }, request);
    }

    [HttpPost("bulk")]
    public async Task<IActionResult> RecordVitalSignsBulk([FromBody] List<VitalSignRequest> requests)
    {
        if (requests == null || requests.Count == 0)
        {
            return BadRequest("At least one vital sign record is required.");
        }

        await _vitalSignService.WriteVitalSignsAsync(requests);
        return Ok(new { message = $"Successfully recorded {requests.Count} vital sign(s)." });
    }

    [HttpGet("{patientId}")]
    public async Task<IActionResult> GetVitalSigns(string patientId, [FromQuery] string? range)
    {
        var vitals = await _vitalSignService.GetVitalSignsAsync(patientId, range ?? "-1h");
        return Ok(vitals);
    }
}
