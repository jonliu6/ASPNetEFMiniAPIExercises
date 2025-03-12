namespace MinimalAPIsWithASPNetEF.DTOs
{
    public class MoviesFilterDTO
    {
        public int Page { get; set; }
        public int RecordsPerPage { get; set; }

        public PaginationDTO PaginationDTO {
            get {
                return new PaginationDTO { Page = Page, RecordsPerPage = RecordsPerPage};
            }
        }

        public string? Title { get; set; }
        public string? GenreId { get; set; }
        public bool InTheater { get; set; }
        public bool FutureRelease { get; set; }
        public string? OrderbyField { get; set; }
        public bool OrderByAscending { get; set; } = true;

    }
}