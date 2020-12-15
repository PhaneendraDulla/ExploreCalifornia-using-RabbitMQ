using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ExploreCalifornia.DeadLetters
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqp://deadLetters:deadLettersPassword@localhost:5672");
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.ExchangeDeclare("deadLettersExchange", ExchangeType.Direct, true);
            channel.QueueDeclare("deadLettersQueue", true, false, false);
            channel.QueueBind("deadLettersQueue", "deadLettersExchange", "");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, eventsArgs) =>
            {
                var message = Encoding.UTF8.GetString(eventsArgs.Body.ToArray());
                var deathReasonBytes = eventsArgs.BasicProperties.Headers["x-first-death-reason"] as byte[];
                var deathReason = Encoding.UTF8.GetString(deathReasonBytes);

                Console.WriteLine($"DeadLetter: { message}. Reason: {deathReason}");
            };

            channel.BasicConsume("deadLettersQueue", true, consumer);

            Console.ReadLine();

            channel.Close();
            connection.Close();
        }
    }
}
