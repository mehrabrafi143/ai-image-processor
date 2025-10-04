using ai_image_processor.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ImageProcessor.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProcessController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProcessController> _logger;

        public ProcessController(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<ProcessController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> ProcessImage()
        {
            try
            {
                var file = Request.Form.Files["image"];
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No image file provided");
                }

                // Validate file type
                if (!IsValidImageFile(file))
                {
                    return BadRequest("Invalid file type. Please upload an image file.");
                }

                // Validate file size (max 10MB)
                if (file.Length > 10 * 1024 * 1024)
                {
                    return BadRequest("File size too large. Maximum size is 10MB.");
                }

                _logger.LogInformation("Processing image: {FileName}", file.FileName);

                // Call AI service
                var result = await CallAIService(file);

                return Ok(result);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "AI service call failed");
                return StatusCode(503, "AI service is temporarily unavailable");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing image");
                return StatusCode(500, "An error occurred while processing the image");
            }
        }

        private bool IsValidImageFile(IFormFile file)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return allowedExtensions.Contains(extension);
        }

        private async Task<ProcessingResult> CallAIService(IFormFile file)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("AIService");
                var aiServiceUrl = _configuration["AIService:Url"];

                // EXTRA DEBUGGING
                _logger.LogInformation("=== DEBUG AI SERVICE CALL ===");
                _logger.LogInformation("Configuration URL: {AiServiceUrl}", aiServiceUrl);
                _logger.LogInformation("Full URL being called: {AiServiceUrl}", aiServiceUrl);

                // Test if we can resolve the URL
                try
                {
                    var testUri = new Uri(aiServiceUrl);
                    _logger.LogInformation("URI parsed successfully: {Uri}", testUri.ToString());
                }
                catch (UriFormatException ex)
                {
                    _logger.LogError(ex, "Invalid URI format: {AiServiceUrl}", aiServiceUrl);
                }

                using var content = new MultipartFormDataContent();
                using var fileStream = file.OpenReadStream();
                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

                content.Add(fileContent, "image", file.FileName);


                _logger.LogInformation("Sending POST request...");

                var response = await client.PostAsync(aiServiceUrl, content);

                _logger.LogInformation("Response Status: {StatusCode}", response.StatusCode);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Error Response: {ErrorContent}", errorContent);
                }

                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Success Response: {Response}", responseString);

                var aiResponse = JsonSerializer.Deserialize<AIResponse>(responseString, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                return new ProcessingResult
                {
                    Classification = aiResponse.Classification,
                    Confidence = aiResponse.Confidence,
                    Objects = aiResponse.Objects?.Select(o => new DetectedObject
                    {
                        Label = o.Label,
                        Confidence = o.Confidence
                    }).ToList(),
                    ProcessingTime = aiResponse.ProcessingTime
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AI service call failed");
                throw;
            }
        }
    }

}