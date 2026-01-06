using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using TicketingSystem.Domain.Enums;

namespace TicketingSystem.Infrastructure.Swagger;

/// <summary>
/// Schema filter to add enum values when the type itself is an enum.
/// </summary>
public class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == null || !context.Type.IsEnum)
            return;

        // If the type itself is an enum, add enum values
        var enumNames = Enum.GetNames(context.Type);
        schema.Type = "string";
        schema.Enum = enumNames
            .Select(name => new OpenApiString(name) as IOpenApiAny)
            .ToList();
        
        // Add description with available values
        var enumValues = string.Join(", ", enumNames);
        schema.Description = string.IsNullOrEmpty(schema.Description) 
            ? $"Available values: {enumValues}" 
            : $"{schema.Description}. Available values: {enumValues}";
    }
}
