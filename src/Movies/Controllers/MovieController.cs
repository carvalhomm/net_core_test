using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Movies.Models;
using Movies.Services;

namespace DatabaseIntegration.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class MovieController : ControllerBase {

        private readonly ILogger<MovieController> _logger;
        private readonly RabbitMQService Messager;

        public MovieController(ILogger<MovieController> logger) {
            _logger = logger;
            this.Messager = new RabbitMQService();
        }

        [HttpGet("getMovies")]
        public async Task<List<Movie>> Get([FromQuery(Name = "id")] string id, [FromQuery(Name = "name")] string name, [FromQuery(Name = "category")] string category) {
            Movie movie = new Movie();
            bool canSend = false;
            if (!String.IsNullOrEmpty(id)) {
                movie.Id = id;
                canSend = true;
            } else if (!String.IsNullOrEmpty(name)) {
                movie.Title = name;
                canSend = true;
            } else if (!String.IsNullOrEmpty(category)) {
                movie.Categories.Add(category);
                canSend = true;
            }
            if (canSend) {
                this.Messager.sendMessage("get", movie);
                return new List<Movie>();
            } else {
                return new List<Movie>();
            }
        }

        [HttpPost("insertMovie")]
        public async Task<IDisposable> setNewMovie([FromBody] Movie request) {
            if (request != null) {
                this.Messager.sendMessage("post", request);
                IDisposable movieSubs = this.Messager.Observable.Subscribe(movie => {
                    Console.WriteLine("chegou fdp --> ", movie);
                });
                Console.WriteLine("movie subs --> ", movieSubs);
                return movieSubs;
            }
            return null;
        }
    }
}
