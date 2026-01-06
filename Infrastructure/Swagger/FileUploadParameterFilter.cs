using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TicketingSystem.Infrastructure.Swagger;

/// <summary>
/// Parameter filter to handle IFormFile parameters by converting them to a proper schema.
/// </summary>
public class FileUploadParameterFilter : IParameterFilter
{
    public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
    {
        if (context.ApiParameterDescription.Type == typeof(IFormFile) ||
            context.ApiParameterDescription.ModelMetadata?.ModelType == typeof(IFormFile))
        {
            // Replace the parameter schema with a file schema
            parameter.Schema = new OpenApiSchema
            {
                Type = "string",
                Format = "binary"
            };
            parameter.In = ParameterLocation.Query; // This will be overridden by operation filter
        }
    }
}
