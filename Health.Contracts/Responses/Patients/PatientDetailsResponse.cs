using Health.Contracts.Requests.MedicalRecords;
using Health.Contracts.Responses.Vitals;

namespace Health.Contracts.Responses.Patients
{
    public class PatientDetailsResponse
    {
        // معلومات المريض الأساسية
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? Gender { get; set; }
        public int? Age { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }

        // آخر Vitals
        public string? LastBloodPressure { get; set; }
        public float? LastBloodSugar { get; set; }
        public int? LastHeartRate { get; set; }
        public int? LastOxygenLevel { get; set; }

        // Blood Pressure Trend — آخر 7 قراءات
        public List<VitalResponse> BloodPressureTrend { get; set; } = new();

        // Medical History من الـ Medical Records
        public List<MedicalRecordResponse> MedicalHistory { get; set; } = new();

        // Medication Adherence
        public int TotalMedications { get; set; }
        public int ActiveMedications { get; set; }
        public double MedicationAdherencePercent { get; set; }
    }
}