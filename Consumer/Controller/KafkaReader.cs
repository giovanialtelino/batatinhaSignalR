using batatinha;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BatatinhaSignalR.Controller
{
    public class KafkaReader : IHostedService
    {
        public KafkaReader()
        {

        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(() =>
            {
                var config = new ConsumerConfig
                {
                    BootstrapServers = "localhost:9092",
                    ClientId = "batima",
                    GroupId = "marvel"
                };

                var schemaRegistryConfig = new SchemaRegistryConfig
                {
                    Url = "localhost:8081"
                };

                using var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);

                using (var consumer = new ConsumerBuilder<Null, BatatinhaDisponivel>(config)
                .SetValueDeserializer(new AvroDeserializer<BatatinhaDisponivel>(schemaRegistry)
                .AsSyncOverAsync())
                .Build())
                {

                    consumer.Subscribe("batata-delivery");

                    var cts = new CancellationTokenSource();

                    while (true)
                    {
                        var message = consumer.Consume(cts.Token);

                        Console.WriteLine($"{message.Value.tipo}:{message.Value.quantidade}");
                    }

                }

            });

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
