using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace dummy_index
{
    class Program
    {
        private static void Main(string[] args)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "rabbitqueue", 
                UserName = "user", 
                Password = "userpass"
            };


            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "productQueue",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Received {0}", message);
                };
                channel.BasicConsume(queue: "productQueue",
                    autoAck: true,
                    consumer: consumer);

                // Keep the application running
                while (true)
                {
                    // You can add any additional processing or sleeping logic here if needed
                }
            }
        }
    }
}