using System.Threading;

namespace Blaster.Server.Implementation
{
    public class QueueOptions
    {
        public string BaseLocation { get; set; }
        public CancellationToken CancellationToken { get; set; }
    }
}
