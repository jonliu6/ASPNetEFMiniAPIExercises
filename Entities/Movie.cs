using System.ComponentModel.DataAnnotations.Schema;

namespace MinimalAPIsWithASPNetEF.Entities
{
    [Table("Movie")] // without specifying, default table name is Movies
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public bool IsReleased { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string? Poster {  get; set; }

        public List<ActorMovie> ActorsMovies { get; set; } = new List<ActorMovie>();
    }
}
