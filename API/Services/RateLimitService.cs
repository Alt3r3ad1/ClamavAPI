using System.Net;

namespace clamavAPI.Services;

public class RateLimitService
{
    private static readonly Dictionary<IPAddress, List<DateTime>> requestHistory = new Dictionary<IPAddress, List<DateTime>>();
    private static readonly TimeSpan rateLimitInterval = TimeSpan.FromSeconds(15);

    public bool TryAcquireConnection(IPAddress clientIp)
    {
        lock (requestHistory)
        {

            if (!requestHistory.TryGetValue(clientIp, out var clientRequests))
            {
                clientRequests = new List<DateTime>();
                requestHistory[clientIp] = clientRequests;
            }

            clientRequests.RemoveAll(requestTime => DateTime.Now - requestTime >= rateLimitInterval);
            string path = string.Empty;
            if (!AppDomain.CurrentDomain.BaseDirectory.Contains("Debug"))
            {
                if (!File.Exists(@"/home/log/clamavapi/access.txt"))
                {
                   using (File.Create(@"/home/log/clamavapi/access.txt")) { };
                }
                path = @"/home/log/clamavapi/access.txt";
            }
            else
            {
                if (!File.Exists(@"log/clamavapi/access.txt"))
                {
                   using (File.Create(@"log/clamavapi/access.txt")) { };
                }
                path = @"log/clamavapi/access.txt";
            }
            string dataWrite = clientIp.ToString() + Environment.NewLine;
            File.AppendAllText(path, dataWrite);

            if (clientRequests.Count >= 3)
            {
                return false;
            }

            clientRequests.Add(DateTime.Now);
            return true;
        }
    }
}