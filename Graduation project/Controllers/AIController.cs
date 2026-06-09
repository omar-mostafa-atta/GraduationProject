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
            var url = "https://mennaelzyat-ai-nutiration-wateen.hf.space/ask/text";
            var response = await _httpClient.PostAsJsonAsync(url, new { 
                message = food

        });
            if (!response.IsSuccessStatusCode)
            {
                return BadRequest(await response.Content.ReadAsStringAsync());
            }


            var result = await response.Content.ReadFromJsonAsync<AiCaloriesResponse>();
            return Ok(result);

        }
        [HttpPost("GetAICaloriesByImage")]
        public async Task<IActionResult> GetAICaloriesByImage(IFormFile image, [FromForm] string? message)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest("Please upload the meal image.");
            }

            var url = "https://mennaelzyat-ai-nutiration-wateen.hf.space/ask/image";

            using var content = new MultipartFormDataContent();

            using var stream = image.OpenReadStream();
            var fileContent = new StreamContent(stream);

            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(image.ContentType);

            content.Add(fileContent, "image", image.FileName);

            if (!string.IsNullOrEmpty(message))
            {
                
                content.Add(new StringContent(message), "message");
            }

            var response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest(await response.Content.ReadAsStringAsync());
            }

            var result = await response.Content.ReadFromJsonAsync<AiCaloriesResponse>();
            return Ok(result);
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
