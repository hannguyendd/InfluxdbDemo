using InfluxDB.Client.Core;

namespace InfluxdbService.Models;

[Measurement("vital_signs")]
public class VitalSign
{
    [Column("patient_id", IsTag = true)]
    public string PatientId { get; set; } = string.Empty;

    [Column("heart_rate")]
    public double? HeartRate { get; set; }

    [Column("blood_pressure_systolic")]
    public double? BloodPressureSystolic { get; set; }

    [Column("blood_pressure_diastolic")]
    public double? BloodPressureDiastolic { get; set; }

    [Column("temperature")]
    public double? Temperature { get; set; }

    [Column("oxygen_saturation")]
    public double? OxygenSaturation { get; set; }

    [Column("respiratory_rate")]
    public double? RespiratoryRate { get; set; }

    [Column(IsTimestamp = true)]
    public DateTime Time { get; set; }
}

public record VitalSignRequest(
    string PatientId,
    double? HeartRate,
    double? BloodPressureSystolic,
    double? BloodPressureDiastolic,
    double? Temperature,
    double? OxygenSaturation,
    double? RespiratoryRate
);
