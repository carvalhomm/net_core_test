using System.Collections.Generic;
using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using DatabaseIntegration.Models;
namespace DatabaseIntegration.Services {
    public class RabbitMQService {
        private IModel Channel;
        private MovieService Movie;
        public RabbitMQService() {
            this.Movie = new MovieService(null);
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

        public void listenToChannels() {
            var consumerPost = new EventingBasicConsumer(this.Channel);
            var consumerGet = new EventingBasicConsumer(this.Channel);
            consumerPost.Received += (model, ea) => {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Movie movie = (Movie)JsonConvert.DeserializeObject(message);
                List<Movie> list = this.Movie.Get(movie.Id, movie.Title, movie.Categories[0]);
            };
            Channel.BasicConsume(queue: "post",
                                 autoAck: true,
                                 consumer: consumerPost);
            consumerGet.Received += (model, ea) => {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Movie movie = (Movie)JsonConvert.DeserializeObject(message);
                this.Movie.setNewMovie(movie);
            };
            Channel.BasicConsume(queue: "get",
                                 autoAck: true,
                                 consumer: consumerGet);
        }
    }
}