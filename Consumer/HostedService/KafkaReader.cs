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
            var activeConsumer = ListConsumerGroupsOpen("localhost:9092", "marvel2");

            Task.Run(async () =>
            {
                var config = new ConsumerConfig
                {
                    BootstrapServers = "localhost:9092",
                    ClientId = "batima5",
                    GroupId = "marvel2",
                    AutoOffsetReset = AutoOffsetReset.Earliest
                };

                var schemaRegistryConfig = new SchemaRegistryConfig
                {
                    Url = "localhost:8081",
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

        private static string ListConsumerGroups(string bootstrapServers)
        {
            string s = ($"Consumer Groups:");
            using (var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = bootstrapServers }).Build())
            {
                var groups = adminClient.ListGroups(TimeSpan.FromSeconds(10));
                foreach (var g in groups)
                {
                    s += "\r\n" + ($"  Group: {g.Group} {g.Error} {g.State}");
                    s += "\r\n" + ($"  Broker: {g.Broker.BrokerId} {g.Broker.Host}:{g.Broker.Port}");
                    s += "\r\n" + ($"  Protocol: {g.ProtocolType} {g.Protocol}");
                    s += "\r\n" + ($"  Members:");
                    foreach (var m in g.Members)
                    {
                        s += "\r\n" + ($"    {m.MemberId} {m.ClientId} {m.ClientHost}");
                        s += "\r\n" + ($"    Metadata: {m.MemberMetadata.Length} bytes");
                        s += "\r\n" + ($"    Assignment: {m.MemberAssignment.Length} bytes");
                    }
                }
            }
            return s;
        }

        private static bool ListConsumerGroupsOpen(string bootstrapServers, string connectingGroup)
        {
            string s = ($"Consumer Groups:");
            using (var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = bootstrapServers }).Build())
            {
                var groups = adminClient.ListGroups(TimeSpan.FromSeconds(10));
                foreach (var g in groups)
                {
                    if (g.Group == connectingGroup)
                    {
                        foreach (var m in g.Members)
                        {
                            if (m.MemberId != null) return false;
                        }
                    }
                }
            }
            return true;
        }

    }
}
