namespace MinimalAPIMoviez.DTOs
{
    public class MovieDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public bool InCinema { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string? CoverImage { get; set; }
        public List<CommentDTO> commentsfk { get; set; } = new List<CommentDTO>();
    }
}
