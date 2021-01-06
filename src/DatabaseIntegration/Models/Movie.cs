using System;
namespace DatabaseIntegration.Models {
    public class Movie {

        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime LaunchedAt { get; set; }
        public decimal Budget { get; set; }
    }
}