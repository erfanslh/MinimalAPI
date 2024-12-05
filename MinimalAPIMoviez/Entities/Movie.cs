namespace MinimalAPIMoviez.Entities
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public bool InCinema { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string? CoverImage { get; set; }
        public List<Comment> commentsfk { get; set; } = new List<Comment>();

        // Many to Many relationship between "Genre & Movie"
        public List<GenreMovie> GenresMovies { get; set; } = new List<GenreMovie>();

        // Many to Many relationship between "Actor & Movie"
        public List<ActorMovie> ActorsMovies { get; set; } = new List<ActorMovie>();

    }
}
