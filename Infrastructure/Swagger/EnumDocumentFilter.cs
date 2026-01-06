using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using TicketingSystem.Domain.Enums;

namespace TicketingSystem.Infrastructure.Swagger;

/// <summary>
/// Document filter to add enum values to string properties in request DTOs.
/// </summary>
public class EnumDocumentFilter : IDocumentFilter
{
    private static readonly Dictionary<string, Type> PropertyEnumMap = new()
    {
        { "Category", typeof(TicketCategory) },
        { "Priority", typeof(PriorityLevel) },
        { "Status", typeof(TicketStatus) },
        { "ResolutionType", typeof(ResolutionType) },
        { "EscalationType", typeof(EscalationType) },
        { "UserType", typeof(UserType) },
        { "AccountStatus", typeof(AccountStatusEnum) }
    };

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        // Iterate through all schemas
        foreach (var schema in swaggerDoc.Components.Schemas)
        {
            if (schema.Value.Properties == null)
                continue;

            // Check each property in the schema
            foreach (var property in schema.Value.Properties)
            {
                if (PropertyEnumMap.TryGetValue(property.Key, out var enumType))
                {
                    // Get enum values
                    var enumNames = Enum.GetNames(enumType);
                    
                    // Update the property schema to include enum values
                    property.Value.Type = "string";
                    property.Value.Enum = enumNames
                        .Select(name => new OpenApiString(name) as IOpenApiAny)
                        .ToList();
                    
                    // Add description with available values
                    var enumValues = string.Join(", ", enumNames);
                    property.Value.Description = $"Available values: {enumValues}";
                }
            }
        }
    }
}
