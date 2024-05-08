using System.Text;
using dummy_index.Models;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Newtonsoft.Json;
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
                    
                    // Add the product to the index
                    var product = JsonConvert.DeserializeObject<Product>(message);
                    AddProductToIndex(product);
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
        public static async void AddProductToIndex(Product product)
        {
            // Setup Elasticsearch
            const string userName = "elastic";
            const string password = "changeme";
            
            var nodes = new Uri[]
            {
                new Uri("http://localhost:9200"),
            };
        
            var pool = new StaticNodePool(nodes);


            var settings = new ElasticsearchClientSettings(pool)
                // Default Mapping
                .DefaultMappingFor<Product>(m => m
                    .IndexName("product-logs")
                )
                // Setup
                .Authentication(new BasicAuthentication(userName, password))
                .EnableDebugMode() // Optional: Enables detailed logging for debugging
                .PrettyJson() // Optional: Formats JSON output to be more readable
                .RequestTimeout(TimeSpan.FromMinutes(2));
            

            var client = new ElasticsearchClient(settings);
            
            var response = await client.IndexAsync(product);
            
            if (!response.IsValidResponse)
            {
                Console.WriteLine("Failed to add product");
                Console.WriteLine(response.DebugInformation);
            }
            else
            {
                Console.WriteLine("Product added to index");
            }
        
        }
    }
}