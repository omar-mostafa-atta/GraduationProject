namespace Health.Contracts.Requests.Vitals
{
    public class RecordVitalRequest
    {
        public string? BloodPressure { get; set; }
        public float? BloodSugarLevel { get; set; }
        public int? HeartRate { get; set; }
        public float? Temperature { get; set; }
        public float? Weight { get; set; }
        public int? OxygenLevel { get; set; }
    }
}