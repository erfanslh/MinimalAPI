using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinimalAPIMoviez.Entities
{
    public class Actor
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(150)]
        public string Name { get; set; } = null!;
        public DateTime BirthDate { get; set; }
        [Display(Name ="Picture")]
        public string? Imagename  { get; set; }
    }
}
