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
                ClientId = "Alfred",
                BootstrapServers = "localhost:9092"

            };

            var schemaRegistryConfig = new SchemaRegistryConfig
            {
                Url = "localhost:8081"
            };

            using var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);

            var random = new Random();

            using (var producer = new ProducerBuilder<Null, BatatinhaDisponivel>(config)
                .SetValueSerializer(new AvroSerializer<BatatinhaDisponivel>(schemaRegistry))
                .Build())
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                    try
                    {
                        var quantidade = random.Next(1, 13);

                        var tipo = (quantidade % 2) == 0 ? "assada" : "frita";

                        await producer.ProduceAsync("batata-delivery", new Message<Null, BatatinhaDisponivel> { Value = new BatatinhaDisponivel { quantidade = quantidade, tipo = tipo } });

                        await Task.Delay(1000, stoppingToken);
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
