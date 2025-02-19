using System.ComponentModel.DataAnnotations.Schema;

namespace MinimalAPIsWithASPNetEF.Entities
{
    [Table("Actor")]
    public class Actor
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]
        public string Name { get; set; } = null!;
        [Column("dateofbirth")]
        public DateTime DateOfBirth { get; set; }
        [Column("picture")]
        public string? Picture { get; set; }

        public List<ActorMovie> ActorsMovies { get; set; } = new List<ActorMovie>();

    }
}
