using FluentValidation;
using MinimalAPIMoviez.DTOs;
using MinimalAPIMoviez.Repositories;

namespace MinimalAPIMoviez.Validations
{
    public class CreateActorDTOValidator : AbstractValidator<CreateActorDTO>
    {
        private async Task<bool> ActorDoesNotExist(CreateActorDTO dto, IActorRepository actorRepository, CancellationToken cancellationToken)
        {
            return !await actorRepository.ActorExists(dto.Name, dto.BirthDate);
        }

        public CreateActorDTOValidator( IActorRepository actorRepository)
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage(ValidationUtilities.NotEmptyMessage)
                .MaximumLength(150).WithMessage(ValidationUtilities.MaxLengthMessage);

            var minimumDate = new DateTime(1900, 01, 01);
            RuleFor(p => p.BirthDate)
                .GreaterThanOrEqualTo(minimumDate).WithMessage(ValidationUtilities.GreaterThanDate(minimumDate));

            RuleFor(p => p)
                .MustAsync((dto, cancellationToken) => ActorDoesNotExist(dto, actorRepository, cancellationToken))
                .WithMessage(dto=> ValidationUtilities.ExistsActor(dto.Name,dto.BirthDate));

        }
    }
}
