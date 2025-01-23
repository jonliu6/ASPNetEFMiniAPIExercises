namespace MinimalAPIsWithASPNetEF.DTOs
{
    public class CreateMovieDTO
    {
        public string Title { get; set; } = null!;
        public bool IsReleased { get; set; }
        public DateTime ReleaseDate { get; set; }
        public IFormFile? Poster { get; set; } = null;
    }
}
