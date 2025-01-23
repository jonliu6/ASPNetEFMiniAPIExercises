using System.ComponentModel.DataAnnotations;

namespace MinimalAPIsWithASPNetEF.DTOs
{
    public class GenreDTO
    {
        public int Id { get; set; }
        
        public string Name { get; set; } = null!;
    }
}
