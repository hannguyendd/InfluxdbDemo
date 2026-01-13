using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxdbService.Models;

namespace InfluxdbService.Services;

public class VitalSignService : IDisposable
{
    private readonly InfluxDBClient _client;
    private readonly HttpClient _httpClient;
    private readonly string _bucket;
    private readonly string _org;
    private readonly string _url;

    public VitalSignService(IConfiguration configuration)
    {
        _url = configuration["InfluxDB:Url"] ?? "http://localhost:8181";
        var token = configuration["InfluxDB:Token"] ?? "";
        _org = configuration["InfluxDB:Org"] ?? "";
        _bucket = configuration["InfluxDB:Bucket"] ?? "vitals";

        _client = new InfluxDBClient(_url, token);
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task WriteVitalSignAsync(VitalSignRequest request)
    {
        var vitalSign = new VitalSign
        {
            PatientId = request.PatientId,
            HeartRate = request.HeartRate,
            BloodPressureSystolic = request.BloodPressureSystolic,
            BloodPressureDiastolic = request.BloodPressureDiastolic,
            Temperature = request.Temperature,
            OxygenSaturation = request.OxygenSaturation,
            RespiratoryRate = request.RespiratoryRate,
            Time = DateTime.UtcNow
        };

        var writeApi = _client.GetWriteApiAsync();
        await writeApi.WriteMeasurementAsync(vitalSign, WritePrecision.Ns, _bucket, _org);
    }

    public async Task WriteVitalSignsAsync(IEnumerable<VitalSignRequest> requests)
    {
        var vitalSigns = requests.Select(request => new VitalSign
        {
            PatientId = request.PatientId,
            HeartRate = request.HeartRate,
            BloodPressureSystolic = request.BloodPressureSystolic,
            BloodPressureDiastolic = request.BloodPressureDiastolic,
            Temperature = request.Temperature,
            OxygenSaturation = request.OxygenSaturation,
            RespiratoryRate = request.RespiratoryRate,
            Time = DateTime.UtcNow
        }).ToList();

        var writeApi = _client.GetWriteApiAsync();
        await writeApi.WriteMeasurementsAsync(vitalSigns, WritePrecision.Ns, _bucket, _org);
    }

    public async Task<List<VitalSign>> GetVitalSignsAsync(string patientId, string range = "-1h")
    {
        var hours = ParseRangeToHours(range);
        var query = $"""
            SELECT time, patient_id, heart_rate, blood_pressure_systolic, blood_pressure_diastolic,
                   temperature, oxygen_saturation, respiratory_rate
            FROM vital_signs
            WHERE patient_id = '{patientId}'
              AND time >= now() - INTERVAL '{hours} hours'
            ORDER BY time DESC
            """;

        var requestBody = new { db = _bucket, q = query, format = "json" };
        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{_url}/api/v3/query_sql", content);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return new List<VitalSign>();
        }

        return ParseQueryResponse(responseContent);
    }

    private static int ParseRangeToHours(string range)
    {
        if (string.IsNullOrEmpty(range)) return 1;

        var value = range.TrimStart('-').TrimEnd('h', 'd', 'm');
        if (!int.TryParse(value, out var num)) return 1;

        if (range.EndsWith('d')) return num * 24;
        if (range.EndsWith('m')) return num / 60;
        return num;
    }

    private static List<VitalSign> ParseQueryResponse(string json)
    {
        var results = new List<VitalSign>();

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        if (root.ValueKind != JsonValueKind.Array) return results;

        foreach (var row in root.EnumerateArray())
        {
            var vitalSign = new VitalSign();

            if (row.TryGetProperty("time", out var time))
                vitalSign.Time = DateTime.Parse(time.GetString() ?? DateTime.UtcNow.ToString());
            if (row.TryGetProperty("patient_id", out var pid))
                vitalSign.PatientId = pid.GetString() ?? "";
            if (row.TryGetProperty("heart_rate", out var hr) && hr.ValueKind == JsonValueKind.Number)
                vitalSign.HeartRate = hr.GetDouble();
            if (row.TryGetProperty("blood_pressure_systolic", out var bps) && bps.ValueKind == JsonValueKind.Number)
                vitalSign.BloodPressureSystolic = bps.GetDouble();
            if (row.TryGetProperty("blood_pressure_diastolic", out var bpd) && bpd.ValueKind == JsonValueKind.Number)
                vitalSign.BloodPressureDiastolic = bpd.GetDouble();
            if (row.TryGetProperty("temperature", out var temp) && temp.ValueKind == JsonValueKind.Number)
                vitalSign.Temperature = temp.GetDouble();
            if (row.TryGetProperty("oxygen_saturation", out var os) && os.ValueKind == JsonValueKind.Number)
                vitalSign.OxygenSaturation = os.GetDouble();
            if (row.TryGetProperty("respiratory_rate", out var rr) && rr.ValueKind == JsonValueKind.Number)
                vitalSign.RespiratoryRate = rr.GetDouble();

            results.Add(vitalSign);
        }

        return results;
    }

    public void Dispose()
    {
        _client.Dispose();
        _httpClient.Dispose();
    }
}
