
using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Generic;
using System.Text;

namespace ExploreCalifornia.BackOffice
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqp://backOffice:backOfficePassword@localhost:5672");
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            var arguments = new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", "deadLettersExchange" }
            };
            channel.QueueDeclare("backOfficeQueue", true, false, false, arguments);
            var headers = new Dictionary<string, object>
            {
                { "subject","tour"},
                { "action","booked"},
                { "x-match","any"}
            };
            channel.QueueBind("backOfficeQueue", "webAppExchange", "", headers);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, eventArgs) =>
            {
                var msg = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                var subject = Encoding.UTF8.GetString(eventArgs.BasicProperties.Headers["subject"] as byte[]);
                var action = Encoding.UTF8.GetString(eventArgs.BasicProperties.Headers["action"] as byte[]);

                Console.WriteLine($"{eventArgs.BasicProperties.UserId} => {subject} {action}: {msg}");
                channel.BasicReject(eventArgs.DeliveryTag, false);
            };

            channel.BasicConsume("backOfficeQueue", false, consumer);

            Console.ReadLine();

            channel.Close();
            connection.Close();
        }
    }
}
