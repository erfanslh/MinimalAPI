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
        public List<GenreMovie> GenresMovies { get; set; } = new List<GenreMovie>();
    }
}
