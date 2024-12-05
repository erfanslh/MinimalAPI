using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinimalAPIMoviez.Entities
{
    public class Actor
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime BirthDate { get; set; }
        public string? Imagename { get; set; }

        //Many to Many relationship between "Actor & Movie"
        public List<ActorMovie> ActorsMovies { get; set; } = new List<ActorMovie>();
    }
}
