using FluentValidation;
using MinimalAPIsWithASPNetEF.DTOs;

namespace MinimalAPIsWithASPNetEF.Validations
{
    public class CreateActorDTOValidator : AbstractValidator<CreateActorDTO>
    {
        public CreateActorDTOValidator()
        { 
            RuleFor(a => a.Name).NotEmpty().WithMessage(ValidationUtils.NOT_EMPTY_MESSAGE);

            var minDate = new DateTime(1900, 1, 1);
            RuleFor(a => a.DateOfBirth).GreaterThanOrEqualTo(minDate).WithMessage("The field {PropertyName} must be greater than " + minDate.ToString("yyyy-MM-dd"));
        }
    }
}
