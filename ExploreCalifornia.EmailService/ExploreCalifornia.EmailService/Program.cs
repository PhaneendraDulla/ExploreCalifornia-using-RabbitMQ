
using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Generic;

namespace ExploreCalifornia.EmailService
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqp://emailService:emailServicePassword@localhost:5672");
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare("emailServiceQueue", true, false, false);
            var headers = new Dictionary<string, object>
            {
                { "subject","tour"},
                { "action","booked"},
                { "x-match","all"}
            };
            channel.QueueBind("emailServiceQueue", "webAppExchange", "", headers);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, eventArgs) =>
            {
                var msg = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                var subject = Encoding.UTF8.GetString(eventArgs.BasicProperties.Headers["subject"] as byte[]);
                var action = Encoding.UTF8.GetString(eventArgs.BasicProperties.Headers["action"] as byte[]);
                Console.WriteLine($"{subject} {action} : {msg}");
            };

            channel.BasicConsume("emailServiceQueue", true, consumer);

            Console.ReadLine();

            channel.Close();
            connection.Close();
        }
    }
}
