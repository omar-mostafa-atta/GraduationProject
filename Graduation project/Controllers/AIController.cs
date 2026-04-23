using Health.Contracts.Responses.AI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Graduation_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AIController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public AIController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }


        [HttpGet("GetAICalories")]
        public async Task<IActionResult> Get(string food)
        {
            var url = "https://mennaelzyat-wateen-nutrition-api.hf.space/ask/text";
            var response = await _httpClient.PostAsJsonAsync(url, new { 
                message = $"how much calories of {food}" 
            });
            if (!response.IsSuccessStatusCode)
            {
                return BadRequest(await response.Content.ReadAsStringAsync());
            }
       
               
            return Ok(await response.Content.ReadFromJsonAsync<AiCaloriesResponse>());
        }

        [HttpGet("GetAiDiagnose")]
        public async Task<IActionResult> GetAiDiagnose(string symptoms)
        {
            var url = "https://mennaelzyat-wateen-diseasepredication.hf.space/diagnose";
            var response = await _httpClient.PostAsJsonAsync(url, new {
                text = $"{symptoms}" 
            });
            if (!response.IsSuccessStatusCode)
            {
                return BadRequest(await response.Content.ReadAsStringAsync());
            }
       
               
            return Ok(await response.Content.ReadFromJsonAsync<AiDiagnoseResponse>());
        }
    }
}
