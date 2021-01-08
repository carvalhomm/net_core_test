using System.Collections.Generic;
using System;
namespace Movies.Models {
    public class Movie {

        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime LaunchedAt { get; set; }
        public decimal Budget { get; set; }

        public IList<string> Categories { get; set; }
    }
}