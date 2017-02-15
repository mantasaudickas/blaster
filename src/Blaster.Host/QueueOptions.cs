using System.Threading;

namespace Vefaqu.Host
{
    public class QueueOptions
    {
        public string BaseLocation { get; set; }
        public CancellationToken CancellationToken { get; set; }
    }
}
