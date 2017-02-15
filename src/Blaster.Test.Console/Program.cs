using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Blaster.Test.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        public static async Task MainAsync(string[] args)
        {
            var enc = new UTF8Encoding();
            var data = "data";

            try
            {
                var count = 1000;
                var timer = Stopwatch.StartNew();
                for (int i = 0; i < count; i++)
                {

                    using (var s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP))
                    {
                        s.Connect("localhost", 51334);
                        s.Send(enc.GetBytes(data));
                        s.Dispose();
                    }
/*
                    var request = WebRequest.CreateHttp(new Uri("http://localhost:51334/"));
                    request.Method = "POST";
                    request.ContentType = "application/text";
                    using (var dataStream = await request.GetRequestStreamAsync())
                    {
                        dataStream.Write(enc.GetBytes(data), 0, data.Length);
                        using (var response = await request.GetResponseAsync())
                        {
                            using (var receiveStream = response.GetResponseStream())
                            {
                                var reader = new StreamReader(receiveStream, Encoding.UTF8);
                                var content = reader.ReadToEnd();
                                //System.Console.WriteLine(content);
                            }
                        }
                    }
*/
                }
                timer.Stop();
                System.Console.WriteLine($"Sent {count} requests.. Spent: {timer.ElapsedMilliseconds} ms. Rate {count / (timer.ElapsedMilliseconds / 1000.0)}");
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
            }
            System.Console.ReadLine();
        }
    }
}