using System.Text;
using clamavAPI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace Blazor.Pages;

public class IndexModel : PageModel
{
    [BindProperty]
    public UploadViewModel upload { get; set; } = new UploadViewModel();

    public async Task OnPostAsync()
    {
        if (upload!.file != null)
        {
            if ((upload!.file.Length / (1024 * 1024)) <= 15)
            {
                string baseUrl = string.Empty;

                if (!AppDomain.CurrentDomain.BaseDirectory.Contains("Debug"))
                {
                    baseUrl = "http://127.0.0.1:5000";
                }
                else
                {
                    baseUrl = $"{Request.Scheme}://{Request.Host}";
                }

                string scanEndPoint = "scan";

                Uri baseUri = new Uri(baseUrl);
                Uri combinedUri = new Uri(baseUri, scanEndPoint);

                string apiUrl = combinedUri.ToString();

                using (var httpClientHandler = new HttpClientHandler())
                {
                    httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                    string clientIPAddress = HttpContext.Connection.RemoteIpAddress!.ToString();

                    using (var client = new HttpClient(httpClientHandler))
                    {
                        client.DefaultRequestHeaders.Add("X-Forwarded-For", clientIPAddress);

                        using (var formData = new MultipartFormDataContent())
                        {
                            if (upload.expectedFileExtension != null)
                            {
                                HttpContent expectedFileExtension = new StringContent(upload.expectedFileExtension, Encoding.UTF8);
                                formData.Add(expectedFileExtension, "expectedFileExtension");
                            }

                            using (HttpContent fileStreamContent = new StreamContent(upload.file.OpenReadStream()))
                            {
                                formData.Add(fileStreamContent, "file", upload.file.FileName);

                                var response = await client.PostAsync(apiUrl, formData);
                                var responseData = await response.Content.ReadAsStringAsync();

                                if (response.IsSuccessStatusCode)
                                {
                                    var scanResult = JsonConvert.DeserializeObject<ScanViewModel>(responseData);

                                    TempData["ScanResult"] = scanResult;
                                }
                                else
                                {

                                    var exception = JsonConvert.DeserializeObject<ExceptionViewModel>(responseData);

                                    TempData["Exception"] = exception;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                TempData["Exception"] = new ExceptionViewModel()
                {
                    message = "The maximum file size of 15MB was exceeded."
                };
            }
        }
        else
        {
            TempData["Exception"] = new ExceptionViewModel()
            {
                message = "No file sent."
            };
        }
    }
}
