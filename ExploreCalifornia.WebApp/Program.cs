using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using RabbitMQ.Client;

namespace ExploreCalifornia.WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqp://webApp:webAppPassword@localhost:5672");

            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.ExchangeDeclare("webAppExchange", ExchangeType.Headers, true);

            channel.Close();
            connection.Close();

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
