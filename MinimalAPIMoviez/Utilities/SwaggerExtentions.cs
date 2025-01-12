using Microsoft.OpenApi.Any;
using MinimalAPIMoviez.DTOs;

namespace MinimalAPIMoviez.Utilities
{
    public static class SwaggerExtentions
    {
        public static TBuilder AddPaginationParameters<TBuilder> (this TBuilder builder) 
            where TBuilder : IEndpointConventionBuilder
        {
            return builder.WithOpenApi(options =>
            {
                // for         public const int PageInitialValue = 1;
                options.Parameters.Add(new Microsoft.OpenApi.Models.OpenApiParameter
                {
                    Name = "Page",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Query,
                    Schema = new Microsoft.OpenApi.Models.OpenApiSchema
                    {
                        Type = "integer",
                        Default = new OpenApiInteger(PaginationDTO.PageInitialValue)
                    }
                });

                // for         public const int RecordsPerPageInitialValue = 10;
                options.Parameters.Add(new Microsoft.OpenApi.Models.OpenApiParameter
                {
                    Name = "RecordsPerPage",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Query,
                    Schema = new Microsoft.OpenApi.Models.OpenApiSchema
                    {
                        Type = "integer",
                        Default = new OpenApiInteger(PaginationDTO.RecordsPerPageInitialValue)
                    }
                });
                return options;
            });
        }
    }
}
