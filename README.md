# ExploreCalifornia-usingRabbitMQ

It has 4 Applications.

1) ExploreCalifornia.WebApp: Web application to book fictional tours and it's sending messages to RabbitMQ.
2) ExploreCalifornia.EmailService: Console application that can pretend to be the service that will send out confirmation emails for booking status in WebApp.
3) ExploreCalifornia.BackOffice: Console application that can pretend to be the service that has all information about confirmation or cancelation status.
4) ExploreCalifornia.DeadLetters: Console application to deal with Dead Letter messages.



WebApp hase a Headers Exchange to publish messages to RabbitMQ on booking or cancelation of a tour.

Email Service has a queue which receives tour booking confirmations only.

BackOffice has a queue which receives any confirmation or cancellation messages.

DeadLetters is deal with DeadLetter messages.
