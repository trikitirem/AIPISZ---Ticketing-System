using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TicketingSystem.Infrastructure.Swagger;

/// <summary>
/// Schema filter to handle IFormFile type in Swagger.
/// </summary>
public class FileUploadSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(IFormFile))
        {
            schema.Type = "string";
            schema.Format = "binary";
            schema.Description = "File to upload";
        }
    }
}
