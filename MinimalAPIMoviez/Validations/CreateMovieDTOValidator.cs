using FluentValidation;
using MinimalAPIMoviez.DTOs;

namespace MinimalAPIMoviez.Validations
{
    public class CreateMovieDTOValidator : AbstractValidator<CreateMovieDTO>
    {
        public CreateMovieDTOValidator()
        {
            RuleFor(m => m.Title).NotEmpty().WithMessage(ValidationUtilities.NotEmptyMessage)
                .MaximumLength(250).WithMessage(ValidationUtilities.MaxLengthMessage);
                
        }
    }
}
