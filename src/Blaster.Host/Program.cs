using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Vefaqu.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        // storage format:
        // [#1 block size] [#1 create date] [#1 expire date] [#1 block] [#2 block size] [#2 create date] [#2 expire date] [#2 block] ... [#Nth block size] [#Nth create date] [#Nth expire date] [#Nth block]
        // checksum is needed? before and after to ensure integrity?
        public static Task MainAsync(string[] args)
        {
            var count = 100000;
            var random = new Random();

            Console.WriteLine("Generating...");
            var messages = new List<byte[]>();
            for (int i = 0; i < count; i++)
            {
                var content = new byte[1024];
                random.NextBytes(content);
                messages.Add(content);
            }

            using (var source = new CancellationTokenSource())
            {
                var server = new VefaquServer(new QueueOptions
                {
                    BaseLocation = @"C:\test",
                    CancellationToken = source.Token
                });

                while (true)
                {
                    Console.WriteLine("Writing...");

                    var timer = Stopwatch.StartNew();

                    var queues = new[] {"q1", "q2", "q3", "q4", "q5"};

                    for (int i = 0; i < messages.Count; ++i)
                    {
                        var content = messages[i];
                        var queue = queues[i % queues.Length];
                        server.Enqueue(queue, new Message(content));
                    }

                    timer.Stop();

                    var elapsedMiliseconds = timer.ElapsedMilliseconds;
                    if (elapsedMiliseconds == 0)
                        elapsedMiliseconds = 1;

                    Console.WriteLine(
                        $"Finished in {timer.Elapsed}. Wrote {count} files. Rate: {count / (elapsedMiliseconds / 1000.0)}/s");

                    Console.WriteLine("Press ESC to stop.. ANY key to repeat..");

                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Escape)
                        break;
                }

                source.Cancel();
                server.Stop();
            }

            Console.ReadKey();
            return Task.FromResult(0);
        }
    }
}