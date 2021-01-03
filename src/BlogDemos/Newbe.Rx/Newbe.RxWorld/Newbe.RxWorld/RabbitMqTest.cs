using System;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using Newbe.RxWorld.DatabaseRepository;
using Newbe.RxWorld.DatabaseRepository.Impl;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Xunit;

namespace Newbe.RxWorld
{
    public class RabbitMqTest
    {
        [Fact(Skip = "require mq")]
        public void Publish()
        {
            var factory = new ConnectionFactory() {HostName = "localhost"};
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue: "hello",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            foreach (var i in Enumerable.Range(0, 1_000_000))
            {
                var body = Encoding.UTF8.GetBytes(i.ToString());

                channel.BasicPublish(exchange: "",
                    routingKey: "hello",
                    basicProperties: null,
                    body: body);
            }
        }

        [Fact(Skip = "require mq")]
        public void ConsumeOneByOne()
        {
            var factory = new ConnectionFactory() {HostName = "localhost"};
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue: "hello",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            var finalDatabaseRepository = new ReactiveBatchRepository(new SQLiteDatabase(nameof(ConsumeOneByOne)));
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body.ToArray());
                finalDatabaseRepository.InsertData(int.Parse(message)).Wait();
                channel.BasicAck(ea.DeliveryTag, false);
            };
            channel.BasicConsume(queue: "hello",
                autoAck: false,
                consumer: consumer);

            Thread.Sleep(TimeSpan.FromSeconds(10));
        }

        [Fact(Skip = "require mq")]
        public void ConsumePreFetchWithFinalRepository()
        {
            var factory = new ConnectionFactory() {HostName = "localhost"};
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue: "hello",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            channel.BasicQos(0, 10000, false);
            var consumer = new EventingBasicConsumer(channel);

            var receiver = Observable.FromEventPattern<EventHandler<BasicDeliverEventArgs>, BasicDeliverEventArgs>(
                h => consumer.Received += h,
                h => consumer.Received -= h);
            var finalDatabaseRepository =
                new ReactiveBatchRepository(new SQLiteDatabase(nameof(ConsumePreFetchWithFinalRepository)));
            using var eventHandlingFlow = receiver
                .Select(@event => Observable.FromAsync(async () =>
                {
                    var item = int.Parse(Encoding.UTF8.GetString(@event.EventArgs.Body.ToArray()));
                    await finalDatabaseRepository.InsertData(item).ConfigureAwait(false);
                    channel.BasicAck(@event.EventArgs.DeliveryTag, false);
                }))
                .Merge()
                .Subscribe();
            channel.BasicConsume(queue: "hello",
                autoAck: false,
                consumer: consumer);

            Thread.Sleep(TimeSpan.FromSeconds(10));
        }

        [Fact(Skip = "require mq")]
        public void ConsumePreFetch()
        {
            var factory = new ConnectionFactory() {HostName = "localhost"};
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue: "hello",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            channel.BasicQos(0, 10000, false);
            var consumer = new EventingBasicConsumer(channel);

            var receiver = Observable
                .FromEventPattern<BasicDeliverEventArgs>(
                    h => consumer.Received += h,
                    h => consumer.Received -= h);
            var database = new SQLiteDatabase(nameof(ConsumePreFetch));
            using var eventHandlingFlow = receiver
                .Buffer(TimeSpan.FromMilliseconds(50), 100)
                .Where(x => x.Count > 0)
                .Select(events => Observable.FromAsync(async () =>
                {
                    var items = events
                        .Select(x => Encoding.UTF8.GetString(x.EventArgs.Body.ToArray()))
                        .Select(int.Parse)
                        .ToArray();
                    await database.InsertMany(items).ConfigureAwait(false);
                    foreach (var @event in events)
                    {
                        channel.BasicAck(@event.EventArgs.DeliveryTag, false);
                    }
                }))
                .Merge()
                .Subscribe();
            channel.BasicConsume(queue: "hello",
                autoAck: false,
                consumer: consumer);

            Thread.Sleep(TimeSpan.FromSeconds(10));
        }
    }
}