using FluentValidation;
using MinimalAPIMoviez.DTOs;
using MinimalAPIMoviez.Repositories;

namespace MinimalAPIMoviez.Validations
{
    public class CreateGenreDTOValidator :AbstractValidator<CreateGenreDTO>
    {
        public CreateGenreDTOValidator(IGenresRepository genresRepository
                                        , IHttpContextAccessor httpContextAccessor)
        {
            var routeValueId =  httpContextAccessor.HttpContext!.Request.RouteValues["id"];
            var id = 0;
            if (routeValueId is string routeValueIdString )
            {
                int.TryParse( routeValueIdString, out id );
            }
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage(ValidationUtilities.NotEmptyMessage)
                .MaximumLength(150).WithMessage(ValidationUtilities.MaxLengthMessage)
                .Must(ValidationUtilities.FirstLetterUpperCase).WithMessage(ValidationUtilities.UpperCaseMessage)
                .MustAsync(async (name, _) =>
                {
                    var exists = await genresRepository.Exists(id, name);
                    return !exists;
                }).WithMessage("This record is already stored and cant be repeat");
            
                
        }

    }
}
