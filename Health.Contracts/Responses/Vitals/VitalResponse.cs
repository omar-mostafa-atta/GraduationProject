namespace Health.Contracts.Responses.Vitals
{
    public class VitalResponse
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string? BloodPressure { get; set; }
        public float? BloodSugarLevel { get; set; }
        public int? HeartRate { get; set; }
        public float? Temperature { get; set; }
        public float? Weight { get; set; }
        public int? OxygenLevel { get; set; }
        public string RecordedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}