namespace MinimalAPIsWithASPNetEF.DTOs
{
    public class CreateActorDTO
    {
        public string Name { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public IFormFile? Picture { get; set; } = null; // type IFormFile because the picture file will be saved
    }
}
