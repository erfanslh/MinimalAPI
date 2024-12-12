using FluentValidation;
using MinimalAPIMoviez.DTOs;

namespace MinimalAPIMoviez.Validations
{
    public class CreateActorDTOValidator : AbstractValidator<CreateActorDTO>
    {
        public CreateActorDTOValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("The {PropertyName} should not be empty")
                .MaximumLength(150).WithMessage("The {PropertyName} should has maximum {MaxLength} Character");

            var minimumDate = new DateTime(1900, 01, 01);
            RuleFor(p => p.BirthDate)
                .GreaterThanOrEqualTo(minimumDate).WithMessage("The Birthday of the Actor should be greater than"+minimumDate.ToString("yyyy-MMMM-dd"));

        }
    }
}
