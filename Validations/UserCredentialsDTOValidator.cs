using FluentValidation;
using MinimalAPIsWithASPNetEF.DTOs;

namespace MinimalAPIsWithASPNetEF.Validations
{
    public class UserCredentialsDTOValidator : AbstractValidator<UserCredentialsDTO>
    {
        public UserCredentialsDTOValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage(ValidationUtils.NOT_EMPTY_MESSAGE)
                .MaximumLength(256).WithMessage(ValidationUtils.MAX_LENGTH_MESSAGE)
                .EmailAddress().WithMessage(ValidationUtils.EMAIL_ADDRESS_MESSAGE);
            RuleFor(x => x.Password).NotEmpty().WithMessage(ValidationUtils.NOT_EMPTY_MESSAGE);
        }
    }
}
