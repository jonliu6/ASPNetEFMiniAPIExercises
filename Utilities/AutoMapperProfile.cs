using AutoMapper;
using MinimalAPIsWithASPNetEF.DTOs;
using MinimalAPIsWithASPNetEF.Entities;

namespace MinimalAPIsWithASPNetEF.Utilities
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile() {
            CreateMap<Genre, GenreDTO>();
            CreateMap<CreateGenreDTO, Genre>();
            CreateMap<Actor, ActorDTO>();
            CreateMap<CreateActorDTO, Actor>().ForMember(p => p.Picture, options => options.Ignore()); // due to the different type of Picture in CreateActorDTO and Actor, auto map other properties except of Picture
            CreateMap<Movie, MovieDTO>()
                .ForMember(m => m.Actors,
                           entity => entity.MapFrom(p => p.ActorsMovies.Select(am => new ActorMovieDTO
                           {
                               Id = am.ActorId,
                               Name = am.Actor.Name,
                               Character = am.Character
                           })));


            CreateMap<CreateMovieDTO, Movie>().ForMember(p => p.Poster, options => options.Ignore());
            CreateMap<Comment, CommentDTO>();
            CreateMap<CreateCommentDTO, Comment>();
            CreateMap<AssignActorMovieDTO, ActorMovie>();
        }
    }
}
