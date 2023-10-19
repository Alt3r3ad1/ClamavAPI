using System.Net;
using clamavAPI.Middlewares;
using nClam;

namespace clamavAPI.Repository;

public class ClamAVRepository
{
    private readonly IClamClient _clamClient;

    public ClamAVRepository(){
        _clamClient = new ClamClient("localhost", 3310);

        if (_clamClient.TryPingAsync().Result == false)
        {
            throw new HttpException(HttpStatusCode.BadRequest, "Scan service inaccessible.");
        }
    }

    public async Task<ClamScanResult> ScanFile(MemoryStream memoryStream)
    {
        return await _clamClient.SendAndScanFileAsync(memoryStream);
    }

}
