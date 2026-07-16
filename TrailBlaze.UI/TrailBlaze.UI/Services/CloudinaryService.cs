using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Headers;

namespace TrailBlaze.UI.Services
{
    public class CloudinaryService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public CloudinaryService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string?> UploadImageAsync(IBrowserFile file)
        {
            try
            {
                var cloudName = _configuration["Cloudinary:CloudName"];
                var uploadPreset = _configuration["Cloudinary:UploadPreset"];

                Console.WriteLine($"Uploading to Cloudinary. CloudName: {cloudName}, Preset: {uploadPreset}");

                var content = new MultipartFormDataContent();
                var fileContent = new StreamContent(file.OpenReadStream(maxAllowedSize: 10485760));
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                content.Add(fileContent, "file", file.Name);
                content.Add(new StringContent(uploadPreset!), "upload_preset");

                var url = $"https://api.cloudinary.com/v1_1/{cloudName}/image/upload";
                Console.WriteLine($"Posting to: {url}");

                var response = await _httpClient.PostAsync(url, content);

                Console.WriteLine($"Response status: {response.StatusCode}");
                var responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response body: {responseBody}");

                if (response.IsSuccessStatusCode)
                {
                    var result = System.Text.Json.JsonSerializer.Deserialize<CloudinaryUploadResult>(
                        responseBody,
                        new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return result?.SecureUrl;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Upload error: {ex.Message}");
                return null;
            }
        }

        public async Task<string?> UploadBase64ImageAsync(string base64Data, string fileName)
        {
            try
            {
                var cloudName = _configuration["Cloudinary:CloudName"];
                var uploadPreset = _configuration["Cloudinary:UploadPreset"];

                Console.WriteLine($"Uploading base64 to Cloudinary. CloudName: {cloudName}, Preset: {uploadPreset}");

                var content = new MultipartFormDataContent();
                content.Add(new StringContent($"data:image/jpeg;base64,{base64Data}"), "file");
                content.Add(new StringContent(uploadPreset!), "upload_preset");

                var url = $"https://api.cloudinary.com/v1_1/{cloudName}/image/upload";
                Console.WriteLine($"Posting to: {url}");

                var response = await _httpClient.PostAsync(url, content);

                Console.WriteLine($"Response status: {response.StatusCode}");
                var responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Cloudinary response: {responseBody}");

                if (response.IsSuccessStatusCode)
                {
                    var result = System.Text.Json.JsonSerializer.Deserialize<CloudinaryUploadResult>(
                        responseBody,
                        new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return result?.SecureUrl;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Upload error: {ex.Message}");
                return null;
            }
        }
    }

    public class CloudinaryUploadResult
    {
        public string? SecureUrl { get; set; }
    }
}