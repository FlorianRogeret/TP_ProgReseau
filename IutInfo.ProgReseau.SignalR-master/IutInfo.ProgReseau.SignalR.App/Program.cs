using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace IutInfo.ProgReseau.SignalR.App
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:60700/stream")
                .AddMessagePackProtocol()
                .Build();

            await connection.StartAsync();

            var channel = await connection.StreamAsChannelAsync<int>("Counter", 10, 500);

            while (await channel.WaitToReadAsync())
            {
                while (channel.TryRead(out var count))
                {
                    Console.WriteLine(count);
                }
            }

            Console.ReadKey();
        }
    }
}