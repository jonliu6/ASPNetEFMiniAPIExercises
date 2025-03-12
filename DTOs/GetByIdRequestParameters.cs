using AutoMapper;
using MinimalAPIsWithASPNetEF.Repositories;

namespace MinimalAPIsWithASPNetEF.DTOs
{
    public class GetByIdRequestParameters
    {
        public IGenresRepository Repository { get; set; } = null!;
        public int Id { get; set; } = 0;

        public IMapper Mapper { get; set; } = null!;
    }
}
