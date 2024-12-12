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
                .NotEmpty().WithMessage("The {PropertyName} should not be empty")
                .MaximumLength(150).WithMessage("The {PropertyName} should has maximum {MaxLength} Character")
                .Must(FirstLetterUpperCase).WithMessage("First letter of {PropertyName} should be UpperCase")
                .MustAsync(async (name, _) =>
                {
                    var exists = await genresRepository.Exists(id, name);
                    return !exists;
                }).WithMessage($"This record is already stored and cant be repeat");
            
                
        }
        private bool FirstLetterUpperCase(string value)
        {
            if (string.IsNullOrEmpty(value))
            { 
                return true;
            }
            var firstLetter = value[0].ToString();
            return firstLetter == firstLetter.ToUpper();
        }
    }
}
