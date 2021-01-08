using System.Collections.Generic;
using System;
using System.Text;
using System.Reactive.Subjects;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Movies.Models;
namespace Movies.Services {
    public class RabbitMQService {
        private IModel Channel;
        public Subject<Movie> Observable;
        public RabbitMQService() {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var connection = factory.CreateConnection();
            this.Channel = connection.CreateModel();
            Channel.QueueDeclare(queue: "get",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
            Channel.QueueDeclare(queue: "get-response",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
            Channel.QueueDeclare(queue: "post",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
            Channel.QueueDeclare(queue: "post-response",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
            this.listenToChannelPost();
        }

        public bool sendMessage(string channelName, Movie parameters) {
            try {
                var json = JsonConvert.SerializeObject(parameters);
                var body = Encoding.UTF8.GetBytes(json);
                Channel.BasicPublish(exchange: "",
                    routingKey: channelName,
                    basicProperties: null,
                    body: body
                );
                return true;
            } catch (Exception error) {
                Console.WriteLine("error on try catch sendMessage --> ", error);
                return false;
            }
        }

        public void listenToChannelPost() {
            var consumer = new EventingBasicConsumer(this.Channel);
            consumer.Received += (model, ea) => {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Movie movie = (Movie)JsonConvert.DeserializeObject(message);
                Console.WriteLine("movie inside callback --> ", message);
                this.Observable.OnNext(movie);
                this.Observable.OnCompleted();
            };

            Channel.BasicConsume(queue: "post-response",
                                 autoAck: true,
                                 consumer: consumer);
        }
    }
}