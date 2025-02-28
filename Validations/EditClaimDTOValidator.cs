using FluentValidation;
using MinimalAPIsWithASPNetEF.DTOs;

namespace MinimalAPIsWithASPNetEF.Validations
{
    public class EditClaimDTOValidator : AbstractValidator<EditClaimDTO>
    {
        public EditClaimDTOValidator()
        {
            RuleFor(x => x.Email).EmailAddress().WithMessage(ValidationUtils.EMAIL_ADDRESS_MESSAGE);
        }
    }
}
