using System.ComponentModel.DataAnnotations;

namespace MinimalAPIMoviez.Entities
{
    public class Genre
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Genre"), StringLength(150)]
        public string? Name { get; set; }

    }
}
