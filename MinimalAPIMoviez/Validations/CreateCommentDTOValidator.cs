using FluentValidation;
using MinimalAPIMoviez.DTOs;

namespace MinimalAPIMoviez.Validations
{
    public class CreateCommentDTOValidator :AbstractValidator<CreateCommentDTO>
    {
        public CreateCommentDTOValidator()
        {
            RuleFor(c => c.Body)
             .NotEmpty().WithMessage(ValidationUtilities.NotEmptyMessage)
             .MaximumLength(1000).WithMessage(ValidationUtilities.MaxLengthMessage);

        }
    }
}
