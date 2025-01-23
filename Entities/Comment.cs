using System.ComponentModel.DataAnnotations.Schema;

namespace MinimalAPIsWithASPNetEF.Entities
{
    [Table("Comment")]
    public class Comment
    {
        public int Id { get; set; }
        public string Body { get; set; } = null!;
        public int MovieId { get; set; }
    }
}
