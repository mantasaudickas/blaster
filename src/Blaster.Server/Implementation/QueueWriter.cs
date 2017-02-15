using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Blaster.Server.Implementation
{
    public class QueueWriter
    {
        const int MaxSize = 10 * 1024 * 1024;

        private readonly ConcurrentQueue<MessageContainer> _messages = new ConcurrentQueue<MessageContainer>();
        private readonly string _queueLocation;
        private readonly CancellationToken _cancellationToken;
        private readonly Task _process;

        public QueueWriter(string queueLocation, CancellationToken cancellationToken)
        {
            _queueLocation = queueLocation;
            _cancellationToken = cancellationToken;

            if (!Directory.Exists(queueLocation))
                Directory.CreateDirectory(queueLocation);

            for (int i = 0; i < 10; i++)
            {
                var stream = File.OpenWrite($"{_queueLocation}\\{i}.dat");
                stream.Write(new byte[] {0}, 0, 1);
                stream.Dispose();
            }

            _process = Task.Factory.StartNew(Run, TaskCreationOptions.LongRunning);
        }

        public void Add(params Message []content)
        {
            foreach (var message in content)
            {
                _messages.Enqueue(new MessageContainer(message));
            }
        }

        public void Stop()
        {
            _process.Wait();
        }

        private void Run()
        {
            var messageCount = 0;
            var streamSize = 0;
            var queueCounter = 0;
            var elapsed = 0L;

            var stream = File.OpenWrite($"{_queueLocation}\\{queueCounter}.dat");
            try
            {
                while (!_cancellationToken.IsCancellationRequested || !_messages.IsEmpty)
                {
                    Task.Delay(100).Wait();
                    if (_messages.Count == 0)
                    {
                        continue;
                    }

                    var iterationMessageCount = 0;
                    var timer = Stopwatch.StartNew();

                    MessageContainer message;
                    while (_messages.TryDequeue(out message))
                    {
                        var content = message.Content;
                        var createDate = message.Created;
                        var expireDate = message.Expires;
                        var length = message.Length;

                        messageCount += 1;
                        iterationMessageCount += 1;

                        stream.Write(length, 0, length.Length);
                        stream.Write(createDate, 0, createDate.Length);
                        stream.Write(expireDate, 0, expireDate.Length);
                        stream.Write(content, 0, content.Length);
                        stream.Write(length, 0, length.Length);             // serves as checksum

                        streamSize += content.Length;
                        if (streamSize >= MaxSize)
                        {
                            stream.Dispose();

                            streamSize = 0;
                            queueCounter += 1;

                            stream = File.OpenWrite($"{_queueLocation}\\{queueCounter}.dat");
                        }
                    }

                    timer.Stop();

                    if (iterationMessageCount > 0)
                    {
                        elapsed += timer.ElapsedMilliseconds;

                        var elapsedMiliseconds = elapsed == 0 ? 1 : elapsed;
                        Console.WriteLine($"Wrote {iterationMessageCount}. Rate {iterationMessageCount / (timer.ElapsedMilliseconds / 1000.0)}. Total: {messageCount}. Rate {messageCount / (elapsedMiliseconds / 1000.0)}");
                    }
                }
            }
            finally
            {
                stream.Dispose();
                Console.WriteLine($"Stopping {_queueLocation}. Wrote messages: {messageCount}.");
            }
        }
    }
}
