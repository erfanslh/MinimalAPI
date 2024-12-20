using FluentValidation;
using MinimalAPIMoviez.DTOs;

namespace MinimalAPIMoviez.Validations
{
    public class UserCredentialsDTOValidator: AbstractValidator<UserCredentialsDTO>
    {
        public UserCredentialsDTOValidator()
        {
            RuleFor(u => u.Email).MaximumLength(256).WithMessage(ValidationUtilities.MaxLengthMessage)
               .NotEmpty().WithMessage(ValidationUtilities.NotEmptyMessage)
               .EmailAddress().WithMessage(ValidationUtilities.EmailAddressMessage);

            RuleFor(u => u.Password).NotEmpty().WithMessage(ValidationUtilities.NotEmptyMessage)
                .MinimumLength(8).WithMessage(ValidationUtilities.MinLengthMessage);
        }
    }
}
