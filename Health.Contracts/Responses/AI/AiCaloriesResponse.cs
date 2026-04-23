using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Contracts.Responses.AI
{
    public class AiCaloriesResponse
    {
        public int Calories { get; set; }
        public string Food_Ar { get; set; }
        public string Food_En { get; set; }
        public string Source { get; set; }
    }
}
