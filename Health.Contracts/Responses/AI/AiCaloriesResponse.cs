using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Health.Contracts.Responses.AI
{

    //public class AiCaloriesResponse
    //{
    //    public int Calories { get; set; }
    //    public string Food_Ar { get; set; }
    //    public string Food_En { get; set; }
    //    public string Source { get; set; }
    //}
    public class AiCaloriesResponse
    {
        public AiCaloriesData Data { get; set; }

        public string Reply { get; set; }
    }

    public class AiCaloriesData
    {
        public string CalculationMethod { get; set; }

        public double CaloriesPer100g { get; set; }

        public double Confidence { get; set; }

        public double EstimatedWeightG { get; set; }

        public string FoodAr { get; set; }

        public string FoodEn { get; set; }

        public int FoodId { get; set; }

        public string RequestedQuantity { get; set; }
        public double TotalCalories { get; set; }
    }
}