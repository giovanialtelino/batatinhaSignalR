using batatinha;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Avro;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;

namespace BatatinhaProducer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ProducerConfig
            {
                ClientId = "batima",
                BootstrapServers = "localhost:9092"

            };

            var schemaRegistryConfig = new SchemaRegistryConfig
            {
                Url = "localhost:8081"
            };

            using var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);


            using (var producer = new ProducerBuilder<Null, BatatinhaDisponivel>(config)
                .SetValueSerializer(new AvroSerializer<BatatinhaDisponivel>(schemaRegistry))
                .Build())
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                 
                    try
                    {
                        await producer.ProduceAsync("batata-delivery", new Message<Null, BatatinhaDisponivel> { Value = new BatatinhaDisponivel { quantidade = 10, tipo = "assada" } });

                        await Task.Delay(50, stoppingToken);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
            };


        }
    }
}
