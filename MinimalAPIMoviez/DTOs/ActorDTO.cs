namespace MinimalAPIMoviez.DTOs
{
    //Get
    public class ActorDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime BirthDate { get; set; }
        //we dont use IFormfile here, because we will return the URL of Image
        public string? Imagename { get; set; }
    }
}
