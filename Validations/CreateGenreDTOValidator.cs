using FluentValidation;
using MinimalAPIsWithASPNetEF.DTOs;
using MinimalAPIsWithASPNetEF.Repositories;

namespace MinimalAPIsWithASPNetEF.Validations
{
    public class CreateGenreDTOValidator : AbstractValidator<CreateGenreDTO>
    {
        // validation for not empty
        public CreateGenreDTOValidator(IGenresRepository repo, 
            IHttpContextAccessor httpCtxAccessor)
        { // use HttpContextAccessor to get route parameters
            var routeValueId = httpCtxAccessor.HttpContext!.Request.RouteValues["id"];
            var id = 0;
            if (routeValueId != null && routeValueId is string routeValueIdString)
            {
                int.TryParse(routeValueIdString, out id); // safely convert routeValueId to id
            }
            RuleFor(r => r.Name)
                .NotEmpty().WithMessage(ValidationUtils.NOT_EMPTY_MESSAGE)
                .MaximumLength(100).WithMessage("The field {PropertyName} must be less than {MaxLength} characters!")
                .Must(ValidationUtils.IsFirstLetterUppercase).WithMessage("The field {PropertyName} must be capitalized!")
                .MustAsync(async(name, _) => {
                    var exists = await repo.Exists(id, name);
                    return !exists;
                }).WithMessage(g => $"A genre with the name {g.Name} already exists!");
        }

    }
}
