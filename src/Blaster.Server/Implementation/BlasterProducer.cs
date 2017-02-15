using System;
using System.Collections.Concurrent;
using System.IO;

namespace Blaster.Server.Implementation
{
    public class BlasterProducer
    {
        private readonly ConcurrentDictionary<string, Lazy<QueueWriter>> _writers = new ConcurrentDictionary<string, Lazy<QueueWriter>>();

        public BlasterProducer(QueueOptions options)
        {
            Options = options;
        }

        public QueueOptions Options { get; }

        public void Enqueue(string queue, params Message[] messages)
        {
            if (messages == null || messages.Length == 0)
                return;

            var queueName = queue.ToUpper();
            var options = Options;

            var hub = _writers.GetOrAdd(queueName, new Lazy<QueueWriter>(() =>
            {
                var location = Path.Combine(options.BaseLocation, queueName);
                return new QueueWriter(location, options.CancellationToken);
            }));

            var queueWriter = hub.Value;
            queueWriter.Add(messages);
        }

        public void Stop()
        {
            foreach (var hub in _writers.Values)
            {
                if (hub.IsValueCreated)
                {
                    var writer = hub.Value;
                    writer.Stop();
                }
            }
        }
    }
}
