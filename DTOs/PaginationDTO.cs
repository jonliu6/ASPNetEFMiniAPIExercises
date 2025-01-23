namespace MinimalAPIsWithASPNetEF.DTOs
{
    /// <summary>
    /// separate DTO for generic pagination can be shared by multiple entities
    /// </summary>
    public class PaginationDTO
    {
        public int Page { get; set; } = 1;
        private int recordsPerPage = 10;
        private readonly int recordsPerPageMax = 50;

        public int RecordsPerPage
        {
            get { return recordsPerPage; }
            set
            {
                // recordsPerPage can only be less or equals to recordsPerPageMax
                if (value > recordsPerPageMax)
                {
                    recordsPerPage = recordsPerPageMax;
                }
                else
                {
                    recordsPerPage = value;
                }
            }
        }
    }
}
