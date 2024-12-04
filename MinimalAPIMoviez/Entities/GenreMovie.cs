namespace MinimalAPIMoviez.Entities
{
    public class GenreMovie
    {
        public int MovieId { get; set; }
        public int GenreId { get; set; }
        public Genre Genres { get; set; } = null!;
        public Movie Movies { get; set; } = null!;

    }
}
