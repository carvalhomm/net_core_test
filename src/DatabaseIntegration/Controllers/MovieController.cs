using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DatabaseIntegration.Models;

namespace DatabaseIntegration.Controllers {
    public class MovieController {

        private readonly ILogger<MovieController> _logger;
        private readonly MovieContext MovieContext = new MovieContext(null);

        public MovieController(ILogger<MovieController> logger) {
            _logger = logger;
        }

        public async Task<List<Movie>> Get([FromQuery(Name = "id")] string id, [FromQuery(Name = "name")] string name, [FromQuery(Name = "category")] string category) {
            await this.MovieContext.Movie.FindAsync();
            var movies = from movie in this.MovieContext.Movie select movie;
            if (!String.IsNullOrEmpty(id)) {
                movies.Where(search => search.Id == id);
            } else if (!String.IsNullOrEmpty(name)) {

            } else if (!String.IsNullOrEmpty(category)) {

            }
            return movies.ToList();
        }

        public async Task<bool> setNewMovie([FromBody] Movie request) {
            Movie exists = await this.MovieContext.Movie.FindAsync();
            if (exists != null) {
                this.MovieContext.Movie.Update(request);
            } else {
                await this.MovieContext.Movie.AddAsync(request);
            }
            this.MovieContext.SaveChanges();
            return true;
        }
    }
}
