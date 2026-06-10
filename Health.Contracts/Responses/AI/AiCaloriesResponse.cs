using System.Text.Json.Serialization;

namespace Health.Contracts.Responses.AI
{
    public class AiCaloriesResponse
    {
        [JsonPropertyName("data")]
        public AiCaloriesData Data { get; set; }

        [JsonPropertyName("reply")]
        public string Reply { get; set; }
    }

    public class AiCaloriesData
    {
        [JsonPropertyName("calculation_method")]
        public string CalculationMethod { get; set; }

        [JsonPropertyName("calories_per_100g")]
        public double CaloriesPer100g { get; set; }

        [JsonPropertyName("confidence")]
        public double Confidence { get; set; }

        [JsonPropertyName("estimated_weight_g")]
        public double EstimatedWeightG { get; set; }

        [JsonPropertyName("food_ar")]
        public string FoodAr { get; set; }

        [JsonPropertyName("food_en")]
        public string FoodEn { get; set; }

        [JsonPropertyName("food_id")]
        public int FoodId { get; set; }

        [JsonPropertyName("requested_quantity")]
        public string RequestedQuantity { get; set; }

        [JsonPropertyName("total_calories")]
        public double TotalCalories { get; set; }
    }
}