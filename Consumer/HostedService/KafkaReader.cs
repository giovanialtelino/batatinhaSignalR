using batatinha;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Channels;
using BatatinhaSignalR.Model;

namespace BatatinhaSignalR.HostedService
{
    public class KafkaReader : BackgroundService
    {
        private readonly ChannelWriter<ValueControllerModelOne> _writerOne;
        private readonly ChannelWriter<ValueControllerModelTwo> _writerTwo;

        public KafkaReader(ChannelWriter<ValueControllerModelTwo> writerTwo, ChannelWriter<ValueControllerModelOne> writerOne)
        {
            _writerOne = writerOne;
            _writerTwo = writerTwo;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task.Run(async () =>
            {
                var config = new ConsumerConfig
                {
                    BootstrapServers = "localhost:9092",
                    ClientId = "batima2",
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

                    while (!stoppingToken.IsCancellationRequested)
                    {
                        var message = consumer.Consume(cts.Token);

                        if (message.Message.Value.tipo == "frita")
                        {
                            _writerOne.TryWrite(new ValueControllerModelOne { Value = message.Message.Value.quantidade });
                        }
                        else if (message.Message.Value.tipo == "assada")
                        {
                            _writerTwo.TryWrite(new ValueControllerModelTwo { Value = message.Message.Value.quantidade });
                        }

                        consumer.Commit(message);

                        Console.WriteLine($"{message.Message.Value.tipo}:{message.Message.Value.quantidade}");
                    }
                }

            }, stoppingToken);

            return Task.CompletedTask;
        }
    }
}
