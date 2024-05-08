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
                HostName = "localhost", 
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
                channel.BasicConsume(queue: "productAddQueue",
                    autoAck: true,
                    consumer: consumer);

                Console.WriteLine("Enter to exit.");
                Console.ReadLine();
            }
        }
    }
}