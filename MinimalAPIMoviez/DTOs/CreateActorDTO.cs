namespace MinimalAPIMoviez.DTOs
{
    //Post
    public class CreateActorDTO
    {
        public string Name { get; set; } = null!;
        public DateTime BirthDate { get; set; }
        //in .net Core we use IFormFile to receive files from User
        public IFormFile? Imagename { get; set; }
    }
}
