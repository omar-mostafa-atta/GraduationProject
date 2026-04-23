using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Health.Contracts.Responses.AI
{
    public class AiDiagnoseResponse
    {

        [JsonPropertyName("success")]
        public bool? SuccessEn { get; set; }

        [JsonPropertyName("نجاح")]
        public bool? SuccessAr { get; set; }
        [JsonIgnore]
        public bool Success => SuccessEn ?? SuccessAr ?? false;

        [JsonPropertyName("input_text")]
        public string InputTextEn { get; set; }

        [JsonPropertyName("النص_المدخل")]
        public string InputTextAr { get; set; }
        [JsonIgnore]
        public string InputText => InputTextEn ?? InputTextAr;

        [JsonPropertyName("results")]
        public List<DiagnoseResult> ResultsEn { get; set; }

        [JsonPropertyName("النتائج")]
        public List<DiagnoseResultAr> ResultsAr { get; set; }
        [JsonIgnore]
        public object Results => (object)ResultsEn ?? ResultsAr;


        [JsonPropertyName("red_flags")]
        public List<string> RedFlagsEn { get; set; }

        [JsonPropertyName("علامات_تحذيرية")]
        public List<string> RedFlagsAr { get; set; }
        [JsonIgnore]
        public List<string> RedFlags => RedFlagsEn ?? RedFlagsAr;

        [JsonPropertyName("note")]
        public string NoteEn { get; set; }

        [JsonPropertyName("ملاحظة")]
        public string NoteAr { get; set; }
        [JsonIgnore]
        public string Note => NoteEn ?? NoteAr;
    }
    public class DiagnoseResult
    {
        public string? predicted_disease { get; set; }

        public string? doctor { get; set; }

        public string? overview { get; set; }
        public string? treatment { get; set; }

        public string? when_to_see_doctor { get; set; }
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public double? score { get; set; }
    }
    public class DiagnoseResultAr
    {
        [JsonPropertyName("المرض_المتوقع")]
        public string? Disease { get; set; }

        [JsonPropertyName("التخصص_المناسب")]
        public string? Doctor { get; set; }

        [JsonPropertyName("نبذة")]
        public string? Overview { get; set; }

        [JsonPropertyName("العلاج")]
        public string? Treatment { get; set; }

        [JsonPropertyName("متى_تزور_الطبيب")]
        public string? WhenToSeeDoctor { get; set; }

        [JsonPropertyName("الدرجة")]
        public double? Score { get; set; }
    }
}
