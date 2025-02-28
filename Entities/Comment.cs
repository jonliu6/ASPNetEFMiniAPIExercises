using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinimalAPIsWithASPNetEF.Entities
{
    [Table("Comment")]
    public class Comment
    {
        public int Id { get; set; }
        public string Body { get; set; } = null!;
        public int MovieId { get; set; }

        // add the following for security control
        public string UserId { get; set; } = null!;
        public IdentityUser User { get; set; } = null!;
    }
}
