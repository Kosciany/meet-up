﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication.Application.AIs
{
    public class RpcClient : IRPCClient
    {
        private IConnection connection;
        private IModel channel;
        private EventingBasicConsumer consumer;
        private ConcurrentDictionary<string,
            TaskCompletionSource<string>> pendingMessages;

        private const string requestQueueName = "requestqueue";
        private const string responseQueueName = "responsequeue";
        private const string exchangeName = ""; // default exchange

        public RpcClient()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            this.connection = factory.CreateConnection();
            this.channel = connection.CreateModel();

            this.channel.QueueDeclare(requestQueueName, true, false, false, null);
            this.channel.QueueDeclare(responseQueueName, true, false, false, null);

            this.consumer = new EventingBasicConsumer(this.channel);
            this.consumer.Received += Consumer_Received;
            this.channel.BasicConsume(responseQueueName, true, consumer);

            this.pendingMessages = new ConcurrentDictionary<string,
                TaskCompletionSource<string>>();
        }
        public Task<string> SendAsync(string message)
        {
            var tcs = new TaskCompletionSource<string>();
            var correlationId = Guid.NewGuid().ToString();

            this.pendingMessages[correlationId] = tcs;

            this.Publish(message, correlationId);

            return tcs.Task;
        }

        private void Publish(string message, string correlationId)
        {
            var props = this.channel.CreateBasicProperties();
            props.CorrelationId = correlationId;
            props.ReplyTo = responseQueueName;

            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            this.channel.BasicPublish(exchangeName, requestQueueName, props, messageBytes);
        }

        private void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            var correlationId = e.BasicProperties.CorrelationId;
            var message = Encoding.UTF8.GetString(e.Body.Span);

            this.pendingMessages.TryRemove(correlationId, out var tcs);
            if (tcs != null)
                tcs.SetResult(message);
        }

    }
}


