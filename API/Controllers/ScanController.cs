using Microsoft.AspNetCore.Mvc;
using clamavAPI.ViewModels;
using clamavAPI.Services;

namespace clamavAPI.Controllers;

[ApiController]
[Route("/")]
public class ScanController : ControllerBase
{
    [HttpPost("scan")]
    public async Task<IActionResult> ScanFile([FromForm] UploadViewModel uploadViewModel, 
                                              [FromServices] ScanService _scanService)
    => Ok(await _scanService.ScanFile(uploadViewModel, Request));
}
