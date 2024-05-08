// See https://aka.ms/new-console-template for more information

// Setup Connection to RabbitMQ

using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory()
{
    HostName = "rabbitqueue",
    UserName = "user",
    Password = "userpass"
};

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

// Declare Queue : Add Product
ConsumeAddProductMessage(channel);

Console.ReadLine();
return;

void ConsumeAddProductMessage(IModel channel1)
{
    var consumer = new EventingBasicConsumer(channel1);

    channel1.QueueDeclare(queue: "productAddQueue",
        durable: true,
        exclusive: false,
        autoDelete: false,
        arguments: null);

    Console.WriteLine(" [*] Waiting for messages.");
    
    
    consumer.Received += (model, ea) =>
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        Console.WriteLine(" [x] Received {0}", message);
    
    };
}