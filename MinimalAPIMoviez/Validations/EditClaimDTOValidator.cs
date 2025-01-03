using FluentValidation;
using MinimalAPIMoviez.DTOs;

namespace MinimalAPIMoviez.Validations
{
    public class EditClaimDTOValidator : AbstractValidator<EditClaimDTO>
    {
        public EditClaimDTOValidator()
        {
              RuleFor(x=> x.Email).NotEmpty().WithMessage(ValidationUtilities.EmailAddressMessage);
        }
    }
}
