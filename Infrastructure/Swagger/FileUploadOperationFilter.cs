using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
using System.Reflection;

namespace TicketingSystem.Infrastructure.Swagger;

/// <summary>
/// Operation filter to handle IFormFile in Swagger documentation.
/// Handles both direct IFormFile parameters and DTOs containing IFormFile properties.
/// </summary>
public class FileUploadOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Check for direct IFormFile parameters
        var fileParameters = context.ApiDescription.ParameterDescriptions
            .Where(p => p.ModelMetadata?.ModelType == typeof(IFormFile))
            .ToList();

        // Check for DTOs with IFormFile properties
        var formParameters = context.ApiDescription.ParameterDescriptions
            .Where(p => p.Source == Microsoft.AspNetCore.Mvc.ModelBinding.BindingSource.Form)
            .ToList();

        var hasFileUpload = false;
        var properties = new Dictionary<string, OpenApiSchema>();
        var required = new HashSet<string>();

        // Handle direct IFormFile parameters
        foreach (var fileParam in fileParameters)
        {
            hasFileUpload = true;
            properties[fileParam.Name] = new OpenApiSchema
            {
                Type = "string",
                Format = "binary",
                Description = "File to upload"
            };
            required.Add(fileParam.Name);
        }

        // Handle DTOs with IFormFile properties
        foreach (var formParam in formParameters)
        {
            if (formParam.ModelMetadata?.ModelType != null)
            {
                var dtoType = formParam.ModelMetadata.ModelType;
                var fileProperties = dtoType.GetProperties()
                    .Where(p => p.PropertyType == typeof(IFormFile))
                    .ToList();

                if (fileProperties.Any())
                {
                    hasFileUpload = true;
                    // Get the schema for the DTO
                    var schema = context.SchemaRepository.Schemas.GetValueOrDefault(
                        context.SchemaGenerator.GenerateSchema(dtoType, context.SchemaRepository).Reference?.Id ?? "");

                    if (schema != null)
                    {
                        // Update file properties to be binary
                        foreach (var prop in fileProperties)
                        {
                            if (schema.Properties.ContainsKey(prop.Name))
                            {
                                schema.Properties[prop.Name] = new OpenApiSchema
                                {
                                    Type = "string",
                                    Format = "binary",
                                    Description = "File to upload"
                                };
                            }
                        }
                    }
                }
            }
        }

        if (hasFileUpload && fileParameters.Any())
        {
            // Remove direct IFormFile parameters from the parameters list
            var parametersToRemove = operation.Parameters
                .Where(p => fileParameters.Any(fp => fp.Name == p.Name))
                .ToList();

            foreach (var param in parametersToRemove)
            {
                operation.Parameters.Remove(param);
            }

            // Create request body for direct IFormFile parameters
            if (properties.Any())
            {
                operation.RequestBody = new OpenApiRequestBody
                {
                    Required = true,
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["multipart/form-data"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "object",
                                Properties = properties,
                                Required = required
                            }
                        }
                    }
                };
            }
        }
    }
}
