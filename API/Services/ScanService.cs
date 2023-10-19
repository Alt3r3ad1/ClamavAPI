using nClam;
using System.Net;
using clamavAPI.ViewModels;
using clamavAPI.Repository;
using clamavAPI.Middlewares;


namespace clamavAPI.Services;

public class ScanService
{
    private readonly ClamAVRepository _clamAVRepository = new ClamAVRepository();
    private readonly RateLimitService _rateLimiter = new RateLimitService();
    private readonly ExtensionAnalysisService _extensionAnalysis = new ExtensionAnalysisService();

    public async Task<ScanViewModel> ScanFile(UploadViewModel uploadViewModel, HttpRequest request)
    {
        if (_rateLimiter.TryAcquireConnection(IPAddress.Parse(request.HttpContext.Connection.RemoteIpAddress!.ToString())))
        {
            var file = uploadViewModel.file;

            if (file is not null && file!.Length > 0)
            {
                if ((file.Length / (1024 * 1024)) <= 15)
                {
                    bool compatibleFileExtension = false;
                    string expectedFileExtension = string.Empty;
                    string fileExtension = string.Empty;
                    string contentType = string.Empty;

                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream);
                        memoryStream.Seek(0, SeekOrigin.Begin);

                        fileExtension = Path.GetExtension(file.FileName).Replace(".", "").ToLower();
                        uploadViewModel.expectedFileExtension = string.IsNullOrWhiteSpace(uploadViewModel.expectedFileExtension) ? fileExtension : uploadViewModel.expectedFileExtension.Replace(".", "").ToLower();

                        if (!string.IsNullOrWhiteSpace(uploadViewModel.expectedFileExtension))
                        {
                            await _extensionAnalysis.Verify(uploadViewModel, memoryStream);

                            compatibleFileExtension =  _extensionAnalysis.compatibleFileExtension;
                            expectedFileExtension = _extensionAnalysis.expectedFileExtension;
                            contentType = _extensionAnalysis.contentType;
                        }

                        var scanResult = await _clamAVRepository.ScanFile(memoryStream);

                        if (scanResult.Result == ClamScanResults.Clean)
                        {
                            return new ScanViewModel()
                            {
                                infected = false,
                                fileExtension = new FileExtensionViewModel()
                                {
                                    compatibleFileExtension = compatibleFileExtension,
                                    expectedFileExtension = expectedFileExtension,
                                    fileExtension = fileExtension,
                                    contentType = contentType
                                }
                            };
                        }
                        return new ScanViewModel()
                        {
                            infected = true,
                            viruses = scanResult.InfectedFiles!.Select(virus => virus.VirusName.Trim()).ToList(),
                            fileExtension = new FileExtensionViewModel()
                            {
                                compatibleFileExtension = compatibleFileExtension,
                                expectedFileExtension = expectedFileExtension,
                                fileExtension = fileExtension,
                                contentType = contentType
                            }
                        };
                    }
                }

                throw new HttpException(HttpStatusCode.BadRequest, "The maximum file size of 15MB was exceeded.");
            }

            throw new HttpException(HttpStatusCode.BadRequest, "No file sent.");
        }

        throw new HttpException(HttpStatusCode.TooManyRequests, "Waiting for a new charge, try again after 15 seconds.");
    }
}

