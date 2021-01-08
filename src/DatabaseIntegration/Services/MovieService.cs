using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DatabaseIntegration.Models;

namespace DatabaseIntegration.Services {
    public class MovieService {

        private readonly ILogger<MovieService> _logger;
        private readonly MovieContext MovieContext = new MovieContext(null);

        public MovieService(ILogger<MovieService> logger) {
            _logger = logger;
        }

        public async List<Movie> Get([FromQuery(Name = "id")] string id, [FromQuery(Name = "name")] string name, [FromQuery(Name = "category")] string category) {
            List<Movie> listMovies = new List<Movie>();
            var movies = from movie in this.MovieContext.Movie select movie;
            if (!String.IsNullOrEmpty(id)) {
                listMovies = movies.Where(search => search.Id == id).ToList();
            } else if (!String.IsNullOrEmpty(name)) {
                listMovies = movies.Where(search => search.Title == name).ToList();
            } else if (!String.IsNullOrEmpty(category)) {
                listMovies = movies.Where(search => search.Categories[0] == category).ToList();
            }
            return listMovies;
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
